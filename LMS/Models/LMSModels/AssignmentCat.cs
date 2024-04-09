using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignmentCat
    {
        public AssignmentCat()
        {
            Assignments = new HashSet<Assignment>();
        }

        public uint Weight { get; set; }
        public string Name { get; set; } = null!;
        public uint Class { get; set; }
        public uint AssId { get; set; }

        public virtual Class ClassNavigation { get; set; } = null!;
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
