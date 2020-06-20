using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SISProject.Models.SISDB_Data;

namespace SISProject.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\FUNCTIONS////////////////////////////////////////////////\\

        //Adding New Users
        [HttpGet]
        public ActionResult AddNewUser()
        {
            SISEntities db = new SISEntities();
            var model = db.Users.Create();
            
            return View(model);
        }

        [HttpPost]
        public ActionResult AddNewUser(string citNum, string logNum, string fulName, string password, string email, string phnNum, string address, 
                                        string title, int groupID, string pic, int depID, HttpPostedFileBase uploadImages)
        {
            SISEntities db = new SISEntities();
            Users user = new Users();

            user.u_Citizen_Number = citNum;
            user.u_Login_Number = logNum;
            user.u_Name = fulName;
            user.u_Password = password;
            user.u_eMail = email;
            user.u_Phone_Number = phnNum;
            user.u_Address = address;
            user.u_Registration_Date = DateTime.Now;
            user.u_Title = title;
            user.u_Groups_TableID = groupID;
            //Picture section
            if (uploadImages != null || uploadImages.ContentLength > 0)
            {
                var name = Path.GetExtension(uploadImages.FileName);
                var fullname = logNum + name;
                var path = Path.Combine(Server.MapPath("~/images/pp"), fullname);
                uploadImages.SaveAs(path);
                user.u_Picture = fullname;
            }else if (uploadImages == null)
            {
                user.u_Picture = fulName;
            }
            else
            {
                user.u_Picture = "user.png";
            }
            //Department Selection
            if (groupID == 1)
            {
                //admin
            }
            else if(groupID == 2 || groupID == 3)
            {
                user.u_Departments_TableID = depID;    
            }
            
            db.Users.Add(user);
            db.SaveChanges();

            return RedirectToAction("AddNewUser", "User");
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        // List of Users
        public ActionResult ListingUsers()
        {
            SISEntities db = new SISEntities();
            var model = db.Users.ToList();
            return View(model);
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //Deleting Users
        public ActionResult DeletingUsers(int ID)
        {
            SISEntities db = new SISEntities();

            if (db.Users.Where(r => r.TableID == ID).Any()) //LINQ usage
            {
                db.Users.Remove(db.Users.Where(r => r.TableID == ID).FirstOrDefault());
                db.SaveChanges();
            }

            return RedirectToAction("ListingUsers","User");
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //Editing Users
        [HttpGet]
        public ActionResult EditingUsers(int id)
        {
            SISEntities db = new SISEntities();
            var model = db.Users.Find(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditingUsers(int id, string citNum, string logNum, string fulName, string password, string email, string phnNum, string address, 
                                                    string title, int groupID, int depID)
        {
            SISEntities db = new SISEntities();
            var model = db.Users.Where(r => r.TableID == id).FirstOrDefault();

            model.u_Citizen_Number = citNum;
            model.u_Login_Number = logNum;
            model.u_Name = fulName;
            model.u_Password = password;
            model.u_eMail = email;
            model.u_Phone_Number = phnNum;
            model.u_Address = address;
            model.u_Title = title;
            model.u_Groups_TableID = groupID;
            model.u_Departments_TableID = depID;

            db.SaveChanges();

            return RedirectToAction("ListingUsers","User");
        }


        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\GET DATA FROM SQL////////////////////////////////////////////////\\

        //To access Departments Table
        class Json_Department
        {
            public int DepID { get; set; }
            public string DepName { get; set; }
        }

        [HttpPost]
        public JsonResult GetDepartments(int ID)
        {
            SISEntities db = new SISEntities();
            List<Json_Department> dep_list = new List<Json_Department>();
            foreach (var dep in db.Deparments.Where(row => row.d_Faculties_TableID == ID).ToList())
            {
                Json_Department new_dep = new Json_Department
                {
                    DepID = dep.TableID,
                    DepName = dep.d_Name
                };
                dep_list.Add(new_dep);
            }
            return Json(dep_list, JsonRequestBehavior.AllowGet);
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //To access Courses Table
        class Json_Courses
        {
            public int CourseID { get; set; }
            public string CourseName { get; set; }
            public string CourseTerm { get; set; }
            public int? CourseECTS { get; set; }
            public string DepName { get; set; }
        }

        [HttpPost]
        public JsonResult GetCourses(int ID)
        {
            SISEntities db = new SISEntities();
            List<Json_Courses> course_list = new List<Json_Courses>();
            foreach (var dep_ID in db.Deparments.Where(row => row.d_Faculties_TableID == ID).ToList())
            {
                foreach (var course in db.Courses.Where(row => row.c_Departments_TableID == dep_ID.TableID).ToList())
                {
                    var dep_name = db.Deparments.Where(row => row.TableID == course.c_Departments_TableID).FirstOrDefault().d_Name;
                    Json_Courses new_course = new Json_Courses
                    {
                        CourseID = course.TableID,
                        CourseName = course.c_Name,
                        CourseTerm = course.c_Term,
                        CourseECTS = course.c_ECTS,
                        DepName = dep_name
                    };
                    course_list.Add(new_course);
                }
            }
            return Json(course_list, JsonRequestBehavior.AllowGet);
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\
    }
}