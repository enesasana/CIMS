using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SISProject.Models.SISDB_Data;

namespace SISProject.Controllers
{
    public class DepartmentController : Controller
    {
        // GET: Department
        public ActionResult Index()
        {
            return View();
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        [HttpGet]
        public ActionResult AddNewDepartment()
        {
            SISEntities db = new SISEntities();
            var model = db.Deparments.Create();

            return View(model);
        }

        [HttpPost]
        public ActionResult AddNewDepartment(string deptName, int facultyID)
        {
            SISEntities db = new SISEntities();
            Deparments deparment = new Deparments
            {
                d_Name = deptName,
                d_Faculties_TableID = facultyID,                
            };

            db.Deparments.Add(deparment);
            db.SaveChanges();

            return View();
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        // List of Departments
        public ActionResult ListingDepartments()
        {
            SISEntities db = new SISEntities();
            var model = db.Deparments.ToList();
            return View(model);
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        public ActionResult DeletingDepartments(int ID)
        {
            SISEntities db = new SISEntities();
            if (db.Deparments.Where(r => r.TableID == ID).Any())
            {
                db.Deparments.Remove(db.Deparments.Where(r => r.TableID == ID).FirstOrDefault());
                db.SaveChanges();
            }
            
            return RedirectToAction("ListingDepartments", "Department");
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //Editing Departments
        [HttpGet]
        public ActionResult EditingDepartments(int id)
        {
            SISEntities db = new SISEntities();
            var model = db.Deparments.Find(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditingDepartments(int id, string deptName, int facultyID)
        {
            SISEntities db = new SISEntities();
            var model = db.Deparments.Where(r => r.TableID == id).FirstOrDefault();

            model.d_Name = deptName;
            model.d_Faculties_TableID = facultyID;

            db.SaveChanges();

            return RedirectToAction("ListingDepartments", "Department");
        }
    }
}