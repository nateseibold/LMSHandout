﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Student
    {
        public Student()
        {
            Enrolleds = new HashSet<Enrolled>();
            Submissions = new HashSet<Submission>();
        }

        public string UId { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public DateOnly Dob { get; set; }
        public uint SId { get; set; }

        public virtual Department SubjectNavigation { get; set; } = null!;
        public virtual ICollection<Enrolled> Enrolleds { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
