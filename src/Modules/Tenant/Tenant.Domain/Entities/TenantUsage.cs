using System.ComponentModel.DataAnnotations;
using TaskFlow.BuildingBlocks.Common;

namespace Tenant.Domain.Entities;

public class TenantUsage : BaseEntity
{
    public TenantUsage(Guid tenantId)
    {
        TenantId = tenantId;
        CurrentUserCount = 0;
        CurrentTaskCount = 0;
        CurrentGroupCount = 0;
        CurrentIndividualTaskCount = 0;
        RowVersion = CreateNextVersion();
    }

    public Guid TenantId { get; private set; }
    public int CurrentUserCount { get; private set; }
    public int CurrentTaskCount { get; private set; }
    public int CurrentGroupCount { get; private set; }
    public int CurrentIndividualTaskCount { get; private set; }

    [Timestamp]
    public byte[] RowVersion { get; private set; }

    public void IncrementUserCount()
    {
        CurrentUserCount++;
        TouchVersion();
    }

    public void DecrementUserCount()
    {
        if (CurrentUserCount <= 0)
        {
            return;
        }

        CurrentUserCount--;
        TouchVersion();
    }

    public void IncrementTaskCount()
    {
        CurrentTaskCount++;
        TouchVersion();
    }

    public void DecrementTaskCount()
    {
        if (CurrentTaskCount <= 0)
        {
            return;
        }

        CurrentTaskCount--;
        TouchVersion();
    }

    public void IncrementGroupCount()
    {
        CurrentGroupCount++;
        TouchVersion();
    }

    public void DecrementGroupCount()
    {
        if (CurrentGroupCount <= 0)
        {
            return;
        }

        CurrentGroupCount--;
        TouchVersion();
    }

    public void IncrementIndividualTaskCount()
    {
        CurrentIndividualTaskCount++;
        TouchVersion();
    }

    public void DecrementIndividualTaskCount()
    {
        if (CurrentIndividualTaskCount <= 0)
        {
            return;
        }

        CurrentIndividualTaskCount--;
        TouchVersion();
    }

    private void TouchVersion()
    {
        RowVersion = CreateNextVersion();
    }

    private static byte[] CreateNextVersion()
    {
        return Guid.NewGuid().ToByteArray();
    }
}
