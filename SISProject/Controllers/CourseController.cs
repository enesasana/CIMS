using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SISProject.Models.SISDB_Data;

namespace SISProject.Controllers
{
    public class CourseController : Controller
    {
        // GET: Course
        public ActionResult Index()
        {
            return View();
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\FUNCTIONS////////////////////////////////////////////////\\

        [HttpGet]
        public ActionResult OpenNewCourse()
        {
            SISEntities db = new SISEntities();
            var model = db.Courses.Create();

            return View(model);
        }

        [HttpPost]
        public ActionResult OpenNewCourse(string courseName, int courseEcts, string courseTerm, int deptID)
        {
            SISEntities db = new SISEntities();
            Courses course = new Courses();
            course.c_Name = courseName;
            course.c_ECTS = courseEcts;
            course.c_Term = courseTerm;
            course.c_Departments_TableID = deptID;

            db.Courses.Add(course);
            db.SaveChanges();           

            return View();
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //Listing Courses
        public ActionResult ListingCourses()
        {
            SISEntities db = new SISEntities();
            var model = db.Courses.ToList();
            return View(model);
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //Deleting Courses
        public ActionResult DeletingCourses(int ID)
        {
            SISEntities db = new SISEntities();

            if (db.Courses.Where(r => r.TableID == ID).Any())
            {
                db.Courses.Remove(db.Courses.Where(r => r.TableID == ID).FirstOrDefault());
                db.SaveChanges();
            }

            return RedirectToAction("ListingCourses", "Course");
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //Editing Courses
        [HttpGet]
        public ActionResult EditingCourses(int id)
        {
            SISEntities db = new SISEntities();
            var model = db.Courses.Find(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditingCourses(int id, string courseName, int courseEcts, string courseTerm, int deptID)
        {
            SISEntities db = new SISEntities();
            var model = db.Courses.Where(r => r.TableID == id).FirstOrDefault();

            model.c_Name = courseName;
            model.c_ECTS = courseEcts;
            model.c_Term = courseTerm;
            model.c_Departments_TableID = deptID;

            db.SaveChanges();

            return RedirectToAction("ListingCourses", "Course");
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //Assignin Courses to Lecturers
        [HttpGet]
        public ActionResult AssigningCourse()
        {
            
            return View();
        }

        //Assignin Courses to Lecturers
        [HttpPost]
        public ActionResult AssigningCourse(int id, int CourseID )
        {
            SISEntities db = new SISEntities();
            Lecturers_Courses lecturer = new Lecturers_Courses
            {
                lc_Users_TableID = id,
                lc_Courses_TableID = CourseID
            };

            db.Lecturers_Courses.Add(lecturer);
            db.SaveChanges();
           
            return View();
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //Selecting Courses for Students
        [HttpGet]
        public ActionResult CourseSelection()
        {
            //To list Selected Courses
            var userID = ((Users)Session["LoggedUser"]).TableID;
            SISEntities db = new SISEntities();
            var model = db.Students_Courses.Where(r => r.sc_Users_TableID == userID).ToList();
            return View(model);
        }

        //Course Selection for Students
        [HttpPost]
        public ActionResult CourseSelection(int courseID)
        {
            var userID = ((Users)Session["LoggedUser"]).TableID;
            SISEntities db = new SISEntities();            
            Students_Courses choosenCourse = new Students_Courses
            {
                sc_Users_TableID = userID,
                sc_Courses_TableID = courseID
            };

            db.Students_Courses.Add(choosenCourse); 
            db.SaveChanges();

            return View();
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //Grade Entry
        public ActionResult GradeEntry()
        {
            return View();
        }

        public ActionResult GradeEntry1(int courseID)
        {
            SISEntities db = new SISEntities();
            var model = db.Students_Courses.Where(r => r.sc_Courses_TableID == courseID).ToList();
            return View(model);
        }

        public ActionResult SetNotes(int id, int midterm, int final, int? resit, double? courseGPA = 0.0)
        {
            SISEntities db = new SISEntities();
            
            var std_course = db.Students_Courses.Where(r => r.TableID == id).FirstOrDefault();
            std_course.sc_Midterm_Grade = midterm;
            std_course.sc_Final_Grade = final;
            std_course.sc_Resit_Grade = resit;
            if(resit == null || resit == 0)
            {
                courseGPA = midterm * 0.4 + final * 0.6;
                std_course.sc_Grade = courseGPA;
            }
            else
            {
                courseGPA = midterm * 0.5 + resit * 0.5;
                std_course.sc_Grade = courseGPA;
            }
            db.SaveChanges();

            return RedirectToAction("GradeEntry1", "Course",new { courseID = std_course.sc_Courses_TableID});
        }

        //Showing Grades
        [HttpGet]
        public ActionResult CourseGrades()
        {
            //To list Selected Courses
            var userID = ((Users)Session["LoggedUser"]).TableID;
            SISEntities db = new SISEntities();
            var model = db.Students_Courses.Where(r => r.sc_Users_TableID == userID).ToList();
            return View(model);
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\GET DATA FROM SQL////////////////////////////////////////////////\\

        //To access Courses Table
        class Json_Courses
        {
            public int CourseID { get; set; }
            public string CourseName { get; set; }
            public string DepName { get; set; }
        }

        [HttpPost]
        public JsonResult GetCoursesWithSelectedUser(int ID)
        {
            SISEntities db = new SISEntities();
            List<Json_Courses> course_list = new List<Json_Courses>();

            var dept_id = db.Users.Where(row => row.TableID == ID).FirstOrDefault().u_Departments_TableID;

            foreach (var course in db.Courses.Where(row => row.c_Departments_TableID == dept_id).ToList())
            {
                if (db.Lecturers_Courses.Where(r => r.lc_Users_TableID == ID).Where(r => r.lc_Courses_TableID == course.TableID).Any())
                    continue;
                else
                {
                    Json_Courses new_course = new Json_Courses
                    {
                        CourseID = course.TableID,
                        CourseName = course.c_Name,

                    };
                    course_list.Add(new_course);
                }              
            }
                
            return Json(course_list, JsonRequestBehavior.AllowGet);
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //To access User Table
        class Json_Users
        {
            public string LecturerName { get; set; }
            public string LecturerTitle { get; set; }  
            public int LecturerID { get; set; }
        }

        [HttpPost]
        public JsonResult GetLecturersFromUsers(int ID)
        {
            SISEntities db = new SISEntities();
            List<Json_Users> lecturer_list = new List<Json_Users>();
            
            foreach(var user in db.Users.Where(row => row.u_Groups_TableID == 2).Where(r=>r.u_Departments_TableID==ID).ToList())
            {
                Json_Users new_user = new Json_Users
                {
                    LecturerName = user.u_Name,
                    LecturerTitle = user.u_Title,
                    LecturerID = user.TableID
                };
                lecturer_list.Add(new_user);
            }

            return Json(lecturer_list, JsonRequestBehavior.AllowGet);
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

    }
}