using System.ComponentModel.DataAnnotations;
using TaskFlow.BuildingBlocks.Common;

namespace Tenant.Domain.Entities
{
    public class TenantUsage : BaseEntity
    {
        public TenantUsage(Guid tenantId)
        {
            TenantId = tenantId;
            CurrentUserCount = 0;
            CurrentTaskCount = 0;
            CurrentGroupCount = 0;
            CurrentIndividualTaskCount = 0;
        }

        public Guid TenantId { get; private set; }
        public int CurrentUserCount { get; private set; }
        public int CurrentTaskCount { get; private set; }
        public int CurrentGroupCount { get; private set; }
        public int CurrentIndividualTaskCount { get; private set; }

        [Timestamp]
        public byte[] RowVersion { get;  private set; }
        public void IncrementUserCount()
        {
            CurrentUserCount++;
        }
        public void DecrementUserCount()
        {
            CurrentUserCount--;
        }
        public void IncrementTaskCount()
        {
            CurrentTaskCount++;
        }
        public void DecrementTaskCount()
        {
            CurrentTaskCount--;
        }
        public void IncrementGroupCount()
        {
            CurrentGroupCount++;
        }
        public void DecrementGroupCount()
        {
            CurrentGroupCount--;
        }
        public void IncrementIndividualTaskCount()
        {
            CurrentIndividualTaskCount++;
        }
        public void DecrementIndividualTaskCount()
        {
            CurrentIndividualTaskCount--;
        }


    }
}
