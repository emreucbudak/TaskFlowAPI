using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Domain.Entities
{
    public class TaskPriorityCategory
    {
        public Guid TaskPriorityCategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
