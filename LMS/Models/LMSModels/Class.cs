using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCats = new HashSet<AssignmentCat>();
            Enrolleds = new HashSet<Enrolled>();
        }

        public uint SYear { get; set; }
        public string Season { get; set; } = null!;
        public string Location { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public uint Professor { get; set; }
        public uint Course { get; set; }
        public uint ClassId { get; set; }

        public virtual Course CourseNavigation { get; set; } = null!;
        public virtual Professor ProfessorNavigation { get; set; } = null!;
        public virtual ICollection<AssignmentCat> AssignmentCats { get; set; }
        public virtual ICollection<Enrolled> Enrolleds { get; set; }
    }
}
