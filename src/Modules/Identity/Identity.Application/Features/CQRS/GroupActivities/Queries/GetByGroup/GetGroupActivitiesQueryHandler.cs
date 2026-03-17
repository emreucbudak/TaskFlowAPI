using FlashMediator;
using Identity.Application.Repositories;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Features.CQRS.GroupActivities.Queries.GetByGroup;

public sealed class GetGroupActivitiesQueryHandler(
    IReadRepository<GroupActivity, Guid> readRepository) : IRequestHandler<GetGroupActivitiesQueryRequest, List<GetGroupActivitiesQueryResponse>>
{
    private static readonly string[] StatusTexts = ["Bekliyor", "Onaylandi", "Reddedildi"];

    public async Task<List<GetGroupActivitiesQueryResponse>> Handle(GetGroupActivitiesQueryRequest request, CancellationToken cancellationToken)
    {
        const int pageSize = 200;
        var page = 1;
        var activities = new List<GroupActivity>();

        while (true)
        {
            var result = await readRepository.GetAllAsync(
                pageSize: pageSize,
                page: page,
                trackChanges: false,
                inc: q => q
                    .Include(a => a.SubmittedByUser)
                    .Include(a => a.ReviewedByUser),
                predicate: a => a.GroupId == request.GroupId);

            activities.AddRange(result.Items);

            if (page * pageSize >= result.TotalCount)
                break;

            page++;
        }

        return activities
            .OrderByDescending(a => a.SubmittedAt)
            .Select(a => new GetGroupActivitiesQueryResponse
            {
                ActivityId = a.Id,
                Title = a.Title,
                Description = a.Description,
                SubmittedByUserId = a.SubmittedByUserId,
                SubmittedByUserName = a.SubmittedByUser?.Name ?? "Bilinmeyen",
                SubmittedAt = a.SubmittedAt,
                Status = (int)a.Status,
                StatusText = StatusTexts[(int)a.Status],
                ReviewedByUserId = a.ReviewedByUserId,
                ReviewedByUserName = a.ReviewedByUser?.Name,
                ReviewedAt = a.ReviewedAt,
                ReviewNote = a.ReviewNote
            })
            .ToList();
    }
}
