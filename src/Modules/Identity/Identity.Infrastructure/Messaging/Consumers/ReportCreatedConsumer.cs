using DotNetCore.CAP;
using FlashMediator; 
using Identity.Application.Features.CQRS.Department.Query.GetDepartmentLeader;
using Microsoft.Extensions.Logging;
using Report.Application.IntegrationEvents;

namespace Identity.Infrastructure.Messaging.Consumers
{
    public class ReportCreatedConsumer : ICapSubscribe
    {
        private readonly IMediator _mediator;
        private readonly ICapPublisher _capPublisher; 
        private readonly ILogger<ReportCreatedConsumer> _logger;

        public ReportCreatedConsumer(
            IMediator mediator,
            ICapPublisher capPublisher,
            ILogger<ReportCreatedConsumer> logger)
        {
            _mediator = mediator;
            _capPublisher = capPublisher;
            _logger = logger;
        }

        [CapSubscribe("report.created", Group = "module.identity")]
        public async Task ConsumeAsync(ReportCreatedIntegrationEvent eventData)
        {
            try
            {
  
                var result = await _mediator.Send(new GetDepartmentLeaderQueryRequest(eventData.NotifiedDepartmentId));

                if (result is not null)
                {
 
                    await _capPublisher.PublishAsync("notification.send", new NotifyUserIntegrationEvent(
                        result.LeaderId,
                        $"Yeni Rapor: {eventData.Content}" 
                    ));

                    _logger.LogInformation($"Rapor {eventData.ReportId} için lider {result.LeaderId} kullanıcısına bildirim tetiklendi.");
                }
                else
                {
                    _logger.LogWarning($"Departman {eventData.NotifiedDepartmentId} için lider bulunamadı.");
                }
            }
            catch (Exception ex)
            {
     
                _logger.LogError(ex, "Identity modülü rapor eventini işlerken hata oluştu.");
                throw; 
            }
        }
    }
}