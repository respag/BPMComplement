using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ultimus.AuditManager.Admin.Commons;
using Ultimus.ComponentManager.Models;

namespace Ultimus.AuditManager.Admin.Controllers
{
    public class WebAplicationController : Controller
    {
        ComponentManagerContext db = Utilities.GetComponentManagerContext();
        CacheManager objCache = new CacheManager();

        public ActionResult Index()
        {
            CatWebAplications Model = new CatWebAplications();

            ViewBag.WebAplicationsList = db.CatWebAplications.OrderBy(w=>w.WebAplicationName).ToList();

            return View(Model);
        }

        [HttpPost]
        public ActionResult CreateOrUpdateWebAplication(CatWebAplications Model)
        {
            if (ModelState.IsValid)
            {

                if (db.CatWebAplications.Any(o => o.IdWebAplication == Model.IdWebAplication))
                {
                    db.Entry(Model).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    db.CatWebAplications.Add(Model);
                }

                Model.isUpdatedOrCreated = true;

                db.SaveChanges();
                ModelState.Clear();

                Model = new CatWebAplications();
                Model.isUpdatedOrCreated = true;
            }

            ViewBag.WebAplicationsList = db.CatWebAplications.OrderBy(w => w.WebAplicationName).ToList();

            return View("Index", Model);
        }


        [HttpGet]
        public ActionResult UpdateWebAplication(int id)
        {
            CatWebAplications Model = new CatWebAplications();

            Model = db.CatWebAplications.Find(id);

            ViewBag.WebAplicationsList = db.CatWebAplications.OrderBy(w => w.WebAplicationName).ToList();

            Model.toUpdate = true;

            return View("Index", Model);
        }

        [HttpGet]
        public ActionResult DeleteWebAplication(int id)
        {
            CatWebAplications Model = db.CatWebAplications.Find(id);

            if (Model.WebAplicationDetails.Count == 0)
            {
                db.CatWebAplications.Remove(Model);
                db.SaveChanges();

                ModelState.Clear();
                Model = new CatWebAplications();
                Model.isDeleted = true;
            }
            else
            {
                ModelState.Clear();
                Model = new CatWebAplications();
                Model.inUse = true;
            }

            ViewBag.WebAplicationsList = db.CatWebAplications.OrderBy(w => w.IdWebAplication).ToList();

            return View("Index", Model);
        }
    }
}
