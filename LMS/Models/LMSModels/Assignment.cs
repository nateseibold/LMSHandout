using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public Assignment()
        {
            Submissions = new HashSet<Submission>();
        }

        public string Name { get; set; } = null!;
        public uint MaxPoint { get; set; }
        public string Content { get; set; } = null!;
        public uint AssId { get; set; }
        public DateTime DueDate { get; set; }
        public uint Cat { get; set; }

        public virtual AssignmentCat CatNavigation { get; set; } = null!;
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
