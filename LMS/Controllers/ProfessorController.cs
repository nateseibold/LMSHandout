using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var queryCourse = from i in db.Courses
                              where i.Subject == subject && i.Number == num
                              select i.CId;

            var queryClass = from i in db.Classes
                             where i.Season == season && i.SYear == year && i.Course == queryCourse.ToArray()[0]
                             select i.ClassId;

            var query = from i in db.Enrolleds
                        where i.Class == queryClass.ToArray()[0]
                        join j in db.Students
                        on i.Student equals j.SId
                        into full
                        from f in full
                        select new { fname = f.FName, lname = f.LName, uid = f.UId, dob = f.Dob, grade = i.Grade};

            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var queryCourse = from i in db.Courses
                              where i.Subject == subject && i.Number == num
                              select i.CId;

            var queryClass = from i in db.Classes
                             where i.Season == season && i.SYear == year && i.Course == queryCourse.ToArray()[0]
                             select i.ClassId;

	        var queryAssignment = from i in db.AssignmentCats
                                  where i.AssId == queryClass.ToArray()[0]
                                  join j in db.Assignments
                                  on i.AssId equals j.Cat
                                  into full
                                  from f in full
				                  join s in db.Submissions
				                  on f.AssId equals s.Assigment
				                  into newFull
				                  from nF in newFull
				                  select nF.Student;

            var query = from i in db.AssignmentCats
                        where i.AssId == queryClass.ToArray()[0]
                        join j in db.Assignments
                        on i.AssId equals j.Cat
                        into full
                        from f in full
                        select new { aname = f.Name, cname = i.Name, due = f.DueDate, submissions = queryAssignment.ToArray().Length };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var queryCourse = from i in db.Courses
                              where i.Subject == subject && i.Number == num
                              select i.CId;

            var queryClass = from i in db.Classes
                             where i.Season == season && i.SYear == year && i.Course == queryCourse.ToArray()[0]
                             select i.ClassId;

            var query = from i in db.AssignmentCats
                        where i.Class == queryClass.ToArray()[0]
                        select new { name = i.Name, weight = i.Weight };
                       
            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            try
            {
                AssignmentCat cat = new AssignmentCat();

                var queryCourse = from i in db.Courses
                                  where i.Subject == subject && i.Number == num
                                  select i.CId;

                var queryClass = from i in db.Classes
                                 where i.Season == season && i.SYear == year && i.Course == queryCourse.ToArray()[0]
                                 select i.ClassId;

                cat.Weight = (uint)catweight;
                cat.Name = category;
                cat.Class = queryClass.ToArray()[1];

                db.AssignmentCats.Add(cat);
                db.SaveChanges();
            }
            catch
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            try
            {
                Assignment assign = new Assignment();

                var queryCourse = from i in db.Courses
                                  where i.Subject == subject && i.Number == num
                                  select i.CId;

                var queryClass = from i in db.Classes
                                 where i.Season == season && i.SYear == year && i.Course == queryCourse.ToArray()[0]
                                 select i.ClassId;

                var queryCat = from i in db.AssignmentCats
                               where i.Class == queryClass.ToArray()[0] && i.Name == category
                               select i.AssId;

                assign.Name = asgname;
                assign.MaxPoint = (uint)asgpoints;
                assign.Content = asgcontents;
                assign.DueDate = asgdue;
                assign.Cat = queryCat.ToArray()[0];

                db.Assignments.Add(assign);
                db.SaveChanges();

		        var query = from i in db.Enrolleds
			                where i.Class == queryClass.ToArray()[0]
                            select i.Student;

		        foreach(uint student in query)
		        {
		            updateGrade(student, asgname, category, queryClass.ToArray()[0], 0);
		        }               
            }   
            catch
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
	    
            var queryCourse = from i in db.Courses
                              where i.Subject == subject && i.Number == num
                              select i.CId;

            var queryClass = from i in db.Classes
                             where i.Season == season && i.SYear == year && i.Course == queryCourse.ToArray()[0]
                             select i.ClassId;

	    	var queryAssignment = from i in db.AssignmentCats
                                    where i.AssId == queryClass.ToArray()[0]
                                    join j in db.Assignments
                                    on i.AssId equals j.Cat
                                    into full
                                    from f in full
				                    join s in db.Submissions
				                    on f.AssId equals s.Assigment
				                    into newFull
                                    from newfull in newFull
				                    join stud in db.Students
				                  
				                on newfull.Student equals stud.SId
		                        into finaltable
				                from ft in finaltable 
				                select new{fname = ft.FName, lname = ft.LName, uid=ft.UId, time = newfull.SubmitDate, score = newfull.Score};
		    
	
            return Json(queryAssignment.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var queryCourse = from i in db.Courses
                              where i.Subject == subject && i.Number == num
                              select i.CId;

            var queryClass = from i in db.Classes
                             where i.Season == season && i.SYear == year && i.Course == queryCourse.ToArray()[0]
                             select i.ClassId;

            var query = from i in db.Students
                        where i.UId == uid
			            select i.SId;

	        updateGrade(query.ToArray()[0], asgname, category, queryClass.ToArray()[0], score);


            return Json(new { success = true });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
	        var query = from i in db.Professors
			            where i.UId == uid
			            join j in db.Classes
			            on i.SId equals j.Professor
			            into classList
			            from cL in classList
			            join k in db.Courses
			            on cL.Course equals k.CId
			            into fullTable
			            from full in fullTable
			            select new {subject = full.Subject, number = full.Number, name = full.Name, season = cL.Season, year = cL.SYear};

            return Json(query.ToArray());
        }

	    private void updateGrade(uint sid, string asgname, string category, uint classID, int score)
	    {
	        var assignQuery = from i in db.AssignmentCats
			            where i.Name == category
                        join j in db.Assignments
			            on i.AssId equals j.Cat
			            into Assigns
			            from a in Assigns
			            where a.Name == asgname
			            select a.AssId;

	        var query = from i in db.Submissions
			            where i.Student == sid && i.Assigment == assignQuery.ToArray()[0]
			            select i;
			          
	        foreach(Submission sub in query)
		    {
		        sub.Score = (uint)score;
		    }   

	        try
	        {
		        db.SaveChanges();
	        }
	        catch (Exception e)
	        {
		        Console.WriteLine("FAIL");
	        }

	        calculateGrade(query.ToArray()[0].Student, classID);
	    }

	    private void calculateGrade(uint sID, uint classID)
	    {
	        var studQuery = from i in db.Submissions
                where i.Student == sID
		        select i;


	        var assQuery = from i in db.AssignmentCats
			   where i.Class == classID
			   join j in db.Assignments
	            on i.AssId equals j.Cat
	            into newTable
		        from tab in newTable
		        join stud in studQuery
		        on tab.AssId equals stud.Assigment
		        into leftTable
		        from left in leftTable.DefaultIfEmpty()
		        select new{weight = i.Weight,maxPoints=  tab.MaxPoint, points = left.Score};

	        var uniqueCat = from i in db.AssignmentCats
			                where i.Class == classID
			                select i.Weight;

	        uint totalWeight = 0;

	        var Array = assQuery.ToArray();
	        uint totalPoints = 0;
	        for(int i = 0; i < Array.Count(); i++){
		        totalWeight += Array[i].weight;
		        totalPoints += Array[i].weight * Array[i].points/Array[i].maxPoints;
            }

	        float score = totalPoints/totalWeight;
	        string grade = FinalGrade(score);
	    
	        var studentQuery = from i in db.Enrolleds
			                   where i.Student == sID && i.Class == classID
			                   select i;

	        foreach(Enrolled student in studentQuery)
            {
		        student.Grade = grade;
            }

	        try
		    {   
		        db.SaveChanges();
		    } 
	        catch
	        {
		        Console.Write("FAIL");
	        }
        }
	    
        
	    private string FinalGrade(float percentage)
	    {
            if (percentage >= 93)
            {
                return "A";
            }
            else if (percentage >= 90)
            {
                return "A-";
            }
            else if (percentage >= 87)
            {
                return "B+";
            }
            else if (percentage >= 83)
            {
                return "B";
            }
            else if (percentage >= 80)
            {
                return "B-";
            }
            else if (percentage >= 77)
            {
                return "C+";
            }
            else if (percentage >= 73)
            {
                return "C";
            }
            else if (percentage >= 70)
            {
                return "C-";
            }
            else if (percentage >= 67)
            {
                return "D+";
            }
            else if (percentage >= 63)
            {
                return "D";
            }
            else if (percentage >= 60)
            {
                return "D-";
            }
            else
            {
                return "E";
            }
        }
	    /*******End code to modify********/
    }   
}

