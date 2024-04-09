using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrolled
    {
        public string Grade { get; set; } = null!;
        public uint Student { get; set; }
        public uint Class { get; set; }
        public uint EnrolledId { get; set; }

        public virtual Class ClassNavigation { get; set; } = null!;
        public virtual Student StudentNavigation { get; set; } = null!;
    }
}
