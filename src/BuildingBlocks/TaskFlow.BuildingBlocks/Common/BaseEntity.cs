using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.BuildingBlocks.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
    }
}
