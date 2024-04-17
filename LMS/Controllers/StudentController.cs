using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private LMSContext db;
        public StudentController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query = from i in db.Students
                        where i.UId == uid
                        join j in db.Enrolleds
                        on i.SId equals j.Student
                        into classList
                        from cL in classList
                        join k in db.Classes
                        on cL.Class equals k.ClassId
			            into classes
			            from c in classes
			            join cla in db.Courses
			            on c.Course equals cla.CId
                        into fullTable
                        from full in fullTable
                        select new { subject = full.Subject, number = full.Number, name = full.Name, season = c.Season, year = c.SYear };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            var queryCourse = from i in db.Courses
                              where i.Subject == subject && i.Number == num
                              select i.CId;

            var queryClass = from i in db.Classes
                             where i.Season == season && i.SYear == year && i.Course == queryCourse.ToArray()[0]
                             select i.ClassId;

            var queryAssign = from i in db.AssignmentCats
                              where i.Class == queryClass.ToArray()[0]
                              join j in db.Assignments
                              on i.AssId equals j.Cat
                              into assignList
                              from a in assignList
                              select new { assign = a, cat = i };

            var queryStudent = from i in db.Students
                               where i.UId == uid
                               select i.SId;

            var query = from q in queryAssign
                        join s in db.Submissions
                        on new { A = q.assign.AssId, B = queryStudent.ToArray()[0] } equals new { A = s.Assigment, B = s.Student }
                        into joined
                        from j in joined.DefaultIfEmpty()
                        select new { aname = q.assign.Name, cname = q.cat.Name, due = q.assign.DueDate, score = j == null ? null : (uint?)j.Score};

            return Json(query.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            try
	        {
                var queryCourse = from i in db.Courses
                                  where i.Subject == subject && i.Number == num
                                  select i.CId;

                var queryClass = from i in db.Classes
                                 where i.Season == season && i.SYear == year && i.Course == queryCourse.ToArray()[0]
                                 select i.ClassId;

                var queryStudent = from i in db.Students
                                   where i.UId == uid
                                   select i.SId;

                var queryCat = from i in db.AssignmentCats
                                  where i.Class == queryClass.ToArray()[0] && i.Name == category
                                  select i.AssId;

                var queryAssign = from i in db.Assignments
                                  where i.Cat == queryCat.ToArray()[0] && i.Name == asgname
                                  select i.AssId;

                var query = from i in db.Submissions
                            where i.Assigment == queryAssign.ToArray()[0] && i.Student == queryStudent.ToArray()[0]
                            select i;
		   
                if(query.ToArray().Count() == 0)
                {
                    Submission sub = new Submission();
                    sub.Student = queryStudent.ToArray()[0];
                    sub.SubmitDate = DateTime.Now;
                    sub.Score = 0;
                    sub.Contents = contents;
                    sub.Assigment = queryAssign.ToArray()[0];

                    db.Submissions.Add(sub);
		        }
                else
                {
                    foreach(Submission sub in query.ToArray())
                    {
                        sub.Contents = contents;
                        sub.SubmitDate = DateTime.Now;
		            }

		        }

                db.SaveChanges();
	        }
            catch
            {
                return Json(new { success = false });
            }    

            return Json(new { success = true });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {    
            try
            {
                var queryCourse = from i in db.Courses
                                  where i.Subject == subject && i.Number == num
                                  select i.CId;

                var queryClass = from i in db.Classes
                                 where i.Season == season && i.SYear == year && i.Course == queryCourse.ToArray()[0]
                                 select i.ClassId;

                var queryStudent = from i in db.Students
                                   where i.UId == uid
                                   select i.SId;

                Enrolled enroll = new Enrolled();
                enroll.Student = queryStudent.ToArray()[0];
                enroll.Class = queryClass.ToArray()[0];
                enroll.Grade = "--";

                db.Enrolleds.Add(enroll);
                db.SaveChanges();

	        }
            catch
            {
                return Json(new { success = false });
            }
            return Json(new { success = true});
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            float gpa = 0.0f;
            int count = 0;

            var queryStudent = from i in db.Students
                               where i.UId == uid
                               select i.SId;

            var query = from i in db.Enrolleds
                        where i.Student == queryStudent.ToArray()[0]
                        select i.Grade;

            foreach(string grade in query.ToArray())
            {
                float gr = CalculateNumber(grade);

                if(gr != -1.0f)
                {
                    count++;
                    gpa += gr;
		        }
	        }

            if (count != 0)
                return Json(gpa / count);


            return Json(gpa);
        }

        private float CalculateNumber(string grade)
        {
            switch (grade)
            {
                case "A":
                    return 4.0f;
                case "A-":
                    return 3.7f;
                case "B+":
                    return 3.3f;
                case "B":
                    return 3.0f;
                case "B-":
                    return 2.7f;
                case "C+":
                    return 2.3f;
                case "C":
                    return 2.0f;
                case "C-":
                    return 1.7f;
                case "D+":
                    return 1.3f;
                case "D":
                    return 1.0f;
                case "D-":
                    return 0.7f;
                case "E":
                    return 0.0f;
                default:
                    return -1.0f;
            }
        }

                
        /*******End code to modify********/

    }
}

