using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Bases;

namespace TaskFlow.Domain.Entities
{
    public class SubTaskAnswer : BaseEntity
    {
        public string AnswerText { get; set; }
        public Guid SubTaskId { get; set; }
        public SubTask SubTask { get; set; }

    }
}
