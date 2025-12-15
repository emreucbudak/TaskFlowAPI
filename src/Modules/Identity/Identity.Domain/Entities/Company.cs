using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Entities
{
    public class Company : TaskFlow.BuildingBlocks.Common.BaseEntity
    {
        public string CompanyName { get; set; }
        public Guid CompanyPlanId { get; set; }

    }
}
