using MVCeTicaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCeTicaret.Controllers
{
    public class ProfileController : Controller
    {
        Context db;
        public ProfileController()
        {
            db = new Context();
        }
        // GET: Profile
        public ActionResult UpdateProfile()
        {
            return View(db.Customers.Find(TemporaryUserData.UserID));
        }

        [HttpPost]
        public ActionResult UpdateProfile(FormCollection frm)
        {
            Customer customer = db.Customers.Find(TemporaryUserData.UserID);

            customer.FirstName = frm["FirstName"];
            customer.LastName = frm["LastName"];
            customer.Password = frm["Password"];
            customer.Gender = frm["Gender"] == "false" ? false : true;
            customer.Birthday = DateTime.Parse(frm["BirthDate"]);
            customer.Address = frm["Address"];
            customer.City = frm["City"];
            customer.Country = frm["Country"];

            db.SaveChanges();
            return RedirectToAction("ProfileUpdated", "Profile");
        }

        public ActionResult ProfileUpdated()
        {
            return View();
        }
    }
}