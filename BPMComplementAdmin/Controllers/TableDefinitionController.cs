using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ultimus.ComponentManager.Models;

namespace Ultimus.AuditManager.Admin.Controllers
{
    public class TableDefinitionController : Controller
    {
        private ComponentManagerContext db = new ComponentManagerContext(@"Data Source=EHERNANDEZPC\MSSQLSERVER_EIH;Initial Catalog=Ultimus.AuditManager;User ID=sa;Password=12345678a;MultipleActiveResultSets=True");

        //
        // GET: /TableDefinition/

        public ActionResult Index()
        {
            var tablesdefinition = db.AuditTablesDefinition.ToList();
            return View(tablesdefinition.ToList());
        }


        //
        // GET: /TableDefinition/Details/5

        public ActionResult Details(int id = 0, string id2 = "")
        {

            if (id2 != "")
            {
                ViewBag.lastAdded = id2;
            }

            AuditTablesDefinition tablesdefinition = db.AuditTablesDefinition.Find(id);

         
            if (tablesdefinition == null)
            {
                return HttpNotFound();
            }
            return View(model: new Tuple<AuditTablesDefinition, AuditColumnsDefinition>(tablesdefinition, new AuditColumnsDefinition()));
        }

        //
        // GET: /TableDefinition/Create

        public ActionResult Create()
        {
            ViewBag.IdConnectionOrigin = new SelectList(db.CatConnections, "IdConnections", "ConnectionString");
            ViewBag.IdConnectionDestination = new SelectList(db.CatConnections, "IdConnections", "ConnectionString");
            return View();
        }

        //
        // POST: /TableDefinition/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AuditTablesDefinition tablesdefinition)
        {
            if (ModelState.IsValid)
            {
                db.AuditTablesDefinition.Add(tablesdefinition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdConnectionOrigin = new SelectList(db.CatConnections, "IdConnections", "ConnectionString");
            ViewBag.IdConnectionDestination = new SelectList(db.CatConnections, "IdConnections", "ConnectionString");
            return View(tablesdefinition);
        }

        //
        // GET: /TableDefinition/Edit/5

        public ActionResult Edit(int id = 0)
        {
            AuditTablesDefinition tablesdefinition = db.AuditTablesDefinition.Find(id);
            if (tablesdefinition == null)
            {
                return HttpNotFound();
            }
            ViewBag.DDLIdConnectionOrigin = new SelectList(db.CatConnections, "IdConnections", "ConnectionString");
            ViewBag.DDLIdConnectionDestination = new SelectList(db.CatConnections, "IdConnections", "ConnectionString");
            return View(tablesdefinition);
        }

        //
        // POST: /TableDefinition/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AuditTablesDefinition tablesdefinition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tablesdefinition).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DDLIdConnectionOrigin = new SelectList(db.CatConnections, "IdConnections", "ConnectionString");
            ViewBag.DDLIdConnectionDestination = new SelectList(db.CatConnections, "IdConnections", "ConnectionString");
            return View(tablesdefinition);
        }

        //
        // GET: /TableDefinition/Delete/5

        public ActionResult Delete(int id = 0)
        {
            AuditTablesDefinition tablesdefinition = db.AuditTablesDefinition.Find(id);
            if (tablesdefinition == null)
            {
                return HttpNotFound();
            }
            return View(tablesdefinition);
        }

        //
        // POST: /TableDefinition/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AuditTablesDefinition tablesdefinition = db.AuditTablesDefinition.Find(id);
            db.AuditTablesDefinition.Remove(tablesdefinition);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateColumn(AuditColumnsDefinition columnsDefinition)
        {   

            if (ModelState.IsValid)
            {
                db.AuditColumnsDefinition.Add(columnsDefinition);
                db.SaveChanges();

                return RedirectToAction("Details", new { id = columnsDefinition.IdTableDefinition, id2 = columnsDefinition.ColumnName});
            }
            return RedirectToAction("Details", new { id = columnsDefinition.IdTableDefinition, id2 = columnsDefinition.ColumnName });
        }
    }
}