using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SISProject.Models.SISDB_Data;
using System.Web.Helpers;

namespace SISProject.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        { 
            return View();
        }

        [HttpPost]
        public ActionResult LoginMethod(string userLoginNumber, string userPassword)
        {
            SISEntities entity = new SISEntities();
            entity.Configuration.ProxyCreationEnabled = true;

            using (SISEntities db = entity)
            {
                foreach (var user in db.Users)
                {
                    if (user.u_Login_Number == userLoginNumber)
                    {
                        if (user.u_Password == userPassword)
                        {
                            Session["LoggedUser"] = user;
                            return Redirect("/Home/Index");
                        }
                        TempData["alertMessage"] = 0;
                        return View("Index");
                    }
                }
            }
            TempData["alertMessage"] = 1;
            return View("Index");
        }
    }
}
