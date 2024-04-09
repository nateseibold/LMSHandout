using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public DateTime SubmitDate { get; set; }
        public uint Score { get; set; }
        public string Contents { get; set; } = null!;
        public uint Student { get; set; }
        public uint Assigment { get; set; }
        public uint SubId { get; set; }

        public virtual Assignment AssigmentNavigation { get; set; } = null!;
        public virtual Student StudentNavigation { get; set; } = null!;
    }
}
