using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ultimus.AuditManager.Admin.Commons;
using Ultimus.ComponentManager.Models;

namespace Ultimus.AuditManager.Admin.Controllers
{
    public class HistoryController : Controller
    {

        ComponentManagerContext db = Utilities.GetComponentManagerContext();
        //
        // GET: /History/Edit/5

        public ActionResult Edit(int id = 0)
        {

            Debug.Write("[Ultimus.AuditManager] HistoryController Edit");

            HistoryConfiguration historyconfiguration;

            if (db.HistoryConfigurations.Where(d => d.IdProcess == id).Count() > 0)
            {

                historyconfiguration = db.HistoryConfigurations.Where(d => d.IdProcess == id).FirstOrDefault();
            }
            else
            {

                db.HistoryConfigurations.Add(
                        new HistoryConfiguration
                        {
                            IdProcess = id,
                            CompletedTaskDeleted = false,
                            IncidentAborted = false,
                            IncidentCompleted = false,
                            IncidentInitiated = false,
                            StepAborted = false,
                            TaskActivated = false,
                            TaskAssigned = false,
                            TaskCompleted = false,
                            TaskConferred = false,
                            TaskDelayed = false,
                            TaskLate = false,
                            TaskResubmitted = false,
                            TaskReturned = false,
                            TaskSubmitFailed = false,
                            TasksPerDayThresholdReached = false,
                            QueueTaskActivated = false,
                            TaskDeletedOnMinResponseComplete = false
                        }
                      );


                db.SaveChanges();


                historyconfiguration = db.HistoryConfigurations.Where(d => d.IdProcess == id).FirstOrDefault();

            }

            return View(historyconfiguration);
        }

        //
        // POST: /History/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HistoryConfiguration historyconfiguration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(historyconfiguration).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ViewBag.SaveChanges = true;

                return RedirectToAction("ProcessMaintenance", "FormConfiguration");
            }
            return View(historyconfiguration);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}