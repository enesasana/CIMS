using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SISProject.Models.SISDB_Data;

namespace SISProject.Controllers
{
    public class FacultyController : Controller
    {
        // GET: Faculty
        public ActionResult Index()
        {
            return View();
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        [HttpGet]
        public ActionResult AddNewFaculty()
        {
            SISEntities db = new SISEntities();
            var model = db.Faculties.Create();

            return View(model);
        }

        [HttpPost]
        public ActionResult AddNewFaculty(string facultyName)
        {
           
            SISEntities db = new SISEntities();
            Faculties faculty = new Faculties
            {
                f_Name = facultyName,                
            };

            db.Faculties.Add(faculty);
            db.SaveChanges();
                      
            return View();
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        // List of Faculties
        public ActionResult ListingFaculties()
        {
            SISEntities db = new SISEntities();
            var model = db.Faculties.ToList();
            return View(model);
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //DeletingFaculties
        public ActionResult DeletingFaculties(int ID)
        {
            SISEntities db = new SISEntities();
            if(db.Faculties.Where(r => r.TableID == ID).Any())
            {
                db.Faculties.Remove(db.Faculties.Where(r => r.TableID == ID).FirstOrDefault());
                db.SaveChanges();
            }

            return RedirectToAction("ListingFaculties", "Faculty");
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////////////////////////////\\

        //EditingFaculties
        [HttpGet]
        public ActionResult EditingFaculties(int id)
        {
            SISEntities db = new SISEntities();
            var model = db.Faculties.Find(id);
            
            return View(model);
        }

        [HttpPost]
        public ActionResult EditingFaculties(int id, string facultyName)
        {
            SISEntities db = new SISEntities();
            var model = db.Faculties.Where(r => r.TableID == id).FirstOrDefault();

            model.f_Name = facultyName;

            db.SaveChanges();

            return RedirectToAction("ListingFaculties", "Faculty");
        }
    }
}