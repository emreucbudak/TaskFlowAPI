using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Bases;

namespace TaskFlow.Domain.Entities
{
    public class SubTask : BaseEntity
    {
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public int TaskStatusId { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public ICollection<SubTaskAnswer> Answers { get; set; }
    }
}
