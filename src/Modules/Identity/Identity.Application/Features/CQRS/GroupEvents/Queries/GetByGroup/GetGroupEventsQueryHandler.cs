using FlashMediator;
using Identity.Application.Features.CQRS.Groups.Exceptions;
using Identity.Application.Repositories;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.GroupEvents.Queries.GetByGroup;

public sealed class GetGroupEventsQueryHandler(
    IReadRepository<Domain.Entities.Groups, Guid> groupReadRepository,
    IReadRepository<GroupEvent, Guid> readRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetGroupEventsQueryRequest, List<GetGroupEventsQueryResponse>>
{
    public async Task<List<GetGroupEventsQueryResponse>> Handle(GetGroupEventsQueryRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId
            ?? throw new AuthExceptions("Kullanici dogrulanamadi.");

        var group = await groupReadRepository.GetByIdAsync(
            trackChanges: false,
            id: request.GroupId,
            inc: query => query.Include(g => g.Users));

        if (group is null)
        {
            throw new GroupsNotFoundExceptions();
        }

        var isMember = group.Users.Any(u => u.UserId == currentUserId);
        if (!isMember)
        {
            throw new AuthExceptions("Bu grubun uyesi degilsiniz.");
        }

        const int pageSize = 200;
        var page = 1;
        var events = new List<GroupEvent>();
        var now = DateTime.UtcNow;

        while (true)
        {
            var result = await readRepository.GetAllAsync(
                pageSize: pageSize,
                page: page,
                trackChanges: false,
                inc: query => query.Include(e => e.CreatedByUser),
                predicate: e => e.GroupId == request.GroupId &&
                    (e.StartsAt >= now || (e.EndsAt.HasValue && e.EndsAt.Value >= now)),
                orderBy: query => query
                    .OrderBy(e => e.StartsAt)
                    .ThenBy(e => e.CreatedAt)
                    .ThenBy(e => e.Id));

            events.AddRange(result.Items);

            if (page * pageSize >= result.TotalCount)
            {
                break;
            }

            page++;
        }

        return events
            .OrderBy(e => e.StartsAt)
            .ThenBy(e => e.CreatedAt)
            .ThenBy(e => e.Id)
            .Select(e => new GetGroupEventsQueryResponse
            {
                GroupEventId = e.Id,
                Subject = e.Subject,
                EventType = e.EventType,
                Title = e.Title,
                Description = e.Description,
                StartsAt = e.StartsAt,
                EndsAt = e.EndsAt,
                MeetingLink = e.MeetingLink,
                CreatedByUserId = e.CreatedByUserId,
                CreatedByUserName = e.CreatedByUser?.Name ?? "Bilinmeyen",
                CreatedAt = e.CreatedAt
            })
            .ToList();
    }
}
