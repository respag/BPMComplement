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
    public class FormConfigurationController : Controller
    {
        ComponentManagerContext db = Utilities.GetComponentManagerContext();
        CacheManager objCache = new CacheManager();

        #region Help form configuration

        public ActionResult HelpProcessConfiguration(int id)
        {
            ViewBag.HelpProcessId = -1;
            ViewBag.HelpProcessToUpdateId = -1;

            HelpProcess Model = new HelpProcess();

            ViewBag.HelpProcessList = (from d in db.HelpProcess where d.IdForm == id select d);
            Model.IdForm = id;

            Model.CatForms = db.CatForms.Find(id);

            return View(Model);
        }

        [HttpPost]
        public ActionResult CreateUpdateHelpProcess(HelpProcess modelHelpProcess)
        {
            HelpProcess Model = new HelpProcess();

            ViewBag.HelpProcessId = -1;
            ViewBag.HelpProcessToUpdateId = -1;

            ViewBag.HelpProcessList = (from d in db.HelpProcess where d.IdForm == modelHelpProcess.IdForm select d);
            Model.IdForm = modelHelpProcess.IdForm;

            Model.CatForms = db.CatForms.Find(modelHelpProcess.IdForm);

            if (ModelState.IsValid)
            {
                if (db.HelpProcess.Any(o => o.IdHelpProcess == modelHelpProcess.IdHelpProcess))
                {
                    db.Entry(modelHelpProcess).State = System.Data.Entity.EntityState.Modified;
                    ViewBag.IsUpdated = true;
                }
                else
                {
                    db.HelpProcess.Add(modelHelpProcess);
                    ViewBag.IsCreated = true;
                }

                db.SaveChanges();
                ModelState.Clear();
                ViewBag.HelpProcessId = modelHelpProcess.IdHelpProcess;
            }

            return View("HelpProcessConfiguration", Model);
        }

        [HttpGet]
        public ActionResult UpdateHelpProcess(int id)
        {
            HelpProcess Model = new HelpProcess();

            ViewBag.HelpProcessId = -1;
            ViewBag.HelpProcessToUpdateId = -1;

            Model = db.HelpProcess.Find(id);
            ViewBag.HelpProcessList = (from d in db.HelpProcess where d.IdForm == Model.IdForm select d);
            ViewBag.HelpProcessToUpdateId = id;

            Model.CatForms = db.CatForms.Find(Model.IdForm);

            return View("HelpProcessConfiguration", Model);
        }

        [HttpGet]
        public ActionResult DeleteHelpProcess(int id)
        {
            HelpProcess Model = new HelpProcess();
            int IdForm = 0;

            ViewBag.HelpProcessId = -1;
            ViewBag.HelpProcessToUpdateId = -1;

            Model = db.HelpProcess.Find(id);
            ViewBag.HelpProcessList = (from d in db.HelpProcess where d.IdForm == Model.IdForm select d);
            IdForm = Model.IdForm;

            db.HelpProcess.Remove(Model);
            db.SaveChanges();


            Model = new HelpProcess();
            Model.IdForm = IdForm;

            Model.CatForms = db.CatForms.Find(Model.IdForm);

            ViewBag.IsDeleted = true;
            ModelState.Clear();

            return View("HelpProcessConfiguration", Model);
        }

        #endregion

        #region Form Maintenance

        public ActionResult FormMaintenance()
        {
            #region Local Variables

            CatForms Model = new CatForms();
            int SelectedCatWebAplicationId = 0;
            int UpdateFormMaintenanceId = -1;

            #endregion

            #region Get Process list

            ViewBag.WebAplicationsList = new SelectList(db.CatWebAplications.OrderBy(w=>w.WebAplicationName), "IdWebAplication", "WebAplicationName");
            ViewBag.FormToUpdateId = -1;

            #endregion

            #region Validate if a form is updated

            if (objCache.GetMyCachedItem("UpdateFormMaintenanceId") != null)
            {
                UpdateFormMaintenanceId = int.Parse(objCache.GetMyCachedItem("UpdateFormMaintenanceId").ToString());
                objCache.RemoveMyCachedItem("UpdateFormMaintenanceId");
                Model = db.CatForms.Find(UpdateFormMaintenanceId);

                ViewBag.WebAplicationsList = new SelectList(db.CatWebAplications, "IdWebAplication", "WebAplicationName", Model.IdWebAplication);
                ViewBag.CatFormList = new List<CatForms>(from d in db.CatForms where d.IdWebAplication == Model.IdWebAplication select d);

                ViewBag.FormToUpdateId = UpdateFormMaintenanceId;
            }

            #endregion

            #region Validate if a form is deleted

            if (objCache.GetMyCachedItem("DeleteFormStatus") != null)
            {
                ViewBag.DeleteFormStatus = objCache.GetMyCachedItem("DeleteFormStatus").ToString();
                objCache.RemoveMyCachedItem("DeleteFormStatus");
            }
            else
                ViewBag.DeleteFormStatus = string.Empty;

            #endregion

            #region Get Form by List

            if (objCache.GetMyCachedItem("SelectedCatProcessesId") != null)
            {
                SelectedCatWebAplicationId = int.Parse(objCache.GetMyCachedItem("SelectedCatProcessesId").ToString());
                objCache.RemoveMyCachedItem("SelectedCatProcessesId");

                ViewBag.CatFormList = new List<CatForms>(from d in db.CatForms where d.IdWebAplication == SelectedCatWebAplicationId select d);
                ViewBag.WebAplicationsList = new SelectList(db.CatWebAplications, "IdWebAplication", "WebAplicationName", SelectedCatWebAplicationId);
                Model.IdWebAplication = SelectedCatWebAplicationId;

            }

            #endregion

            #region Validate if a form is added

            if (objCache.GetMyCachedItem("CatForms.IdForm") != null)
            {
                ViewBag.FormAddedId = int.Parse(objCache.GetMyCachedItem("CatForms.IdForm").ToString());
                objCache.RemoveMyCachedItem("CatForms.IdForm");
            }
            else
                ViewBag.FormAddedId = -1;

            #endregion

            return View(Model);
        }

        [HttpPost]
        public ActionResult FormMaintenance(string IdWebAplication)
        {
            #region Local Variables

            ViewBag.FormToUpdateId = -1;

            CatForms Model = new CatForms();
            int SelectedCatWebAplicationId = 0;
            ViewBag.FormAddedId = -1;

            bool isNumeric = int.TryParse(IdWebAplication, out SelectedCatWebAplicationId);

            #endregion

            #region Get form list by Process

            if (SelectedCatWebAplicationId != 0)
            {
                ViewBag.CatFormList = new List<CatForms>(from d in db.CatForms where d.IdWebAplication == SelectedCatWebAplicationId select d);
                ViewBag.WebAplicationsList = new SelectList(db.CatWebAplications, "IdWebAplication", "WebAplicationName", SelectedCatWebAplicationId);
                Model.IdWebAplication = SelectedCatWebAplicationId;
            }
            else
            {
                ViewBag.WebAplicationsList = new SelectList(db.CatWebAplications.OrderBy(w => w.WebAplicationName), "IdWebAplication", "WebAplicationName");
                ViewBag.CatFormList = null;
            }

            #endregion
            
            return View(Model);
        }

        [HttpPost]
        public ActionResult CreateForm(CatForms newCatForms)
        {
            if (ModelState.IsValid)
            {
                if (db.CatForms.Any(o => o.IdForm == newCatForms.IdForm))
                {
                    db.Entry(newCatForms).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    db.CatForms.Add(newCatForms);
                }

                db.SaveChanges();

                objCache.AddToMyCache("SelectedCatProcessesId", newCatForms.IdWebAplication, MyCachePriority.Default);
                objCache.AddToMyCache("CatForms.IdForm", newCatForms.IdForm, MyCachePriority.Default);
            }
            return RedirectToAction("FormMaintenance");
        }

        [HttpGet]
        public ActionResult DeleteForm(int id)
        {
            CatForms deleteCatSteps = db.CatForms.Find(id);

            objCache.AddToMyCache("SelectedCatProcessesId", deleteCatSteps.IdWebAplication, MyCachePriority.Default);

            if (deleteCatSteps.FormsProcess.Count == 0)
            {
                db.CatForms.Remove(deleteCatSteps);
                db.SaveChanges();

                objCache.AddToMyCache("DeleteFormStatus", "success", MyCachePriority.Default);
            }
            else
                objCache.AddToMyCache("DeleteFormStatus", "error", MyCachePriority.Default);

            return RedirectToAction("FormMaintenance");
        }

        [HttpGet]
        public ActionResult UpdateFormMaintenance(int id)
        {
            CatForms UpdateCatSteps = db.CatForms.Find(id);

            objCache.AddToMyCache("UpdateFormMaintenanceId", id, MyCachePriority.Default);

            return RedirectToAction("FormMaintenance");
        }

        #endregion

        #region Form vs Process (Tab Menu)

        public ActionResult Index()
        {
            #region Local Variables

            FormsProcess Model = new FormsProcess();
            int SelectedCatProcessesId = 0;
            int SelectedCatStepId = 0;
            int Selected_IdWebAplication = 0;

            #endregion

            #region Get process list

            ViewBag.ProcessDLL = new SelectList(db.CatProcesses.OrderBy(p => p.ProcessName), "IdProcess", "ProcessName");

            #endregion

            #region Validate if a form is deleted

            if (objCache.GetMyCachedItem("DeleteFormStatus") != null)
            {
                ViewBag.DeleteFormStatus = objCache.GetMyCachedItem("DeleteFormStatus").ToString();
                objCache.RemoveMyCachedItem("DeleteFormStatus");
            }
            else
                ViewBag.DeleteStepStatus = string.Empty;

            #endregion

            #region Get form, step and Forms configurated by Process


            if (objCache.GetMyCachedItem("SelectedCatProcessesId") != null)
            {
                SelectedCatProcessesId = int.Parse(objCache.GetMyCachedItem("SelectedCatProcessesId").ToString());
                objCache.RemoveMyCachedItem("SelectedCatProcessesId");

                ViewBag.ProcessDLL = new SelectList(db.CatProcesses, "IdProcess", "ProcessName", SelectedCatProcessesId.ToString());

                Model.IdProcess = SelectedCatProcessesId;

   

                List<CatSteps> _CatStepsList = new List<CatSteps>(from d in db.CatSteps
                                                                  where d.IdProcess == SelectedCatProcessesId
                                                                  select d);

                ViewBag.IdStepDLL = new SelectList(_CatStepsList, "IdStep", "StepName");

                #region Web Aplication list

                List<WebAplicationDetails> WebAplicationDetailsList = new List<WebAplicationDetails>(from d in db.WebAplicationDetails
                                                                                                     where d.IdProcess == SelectedCatProcessesId
                                                                                                     select d);

                ViewBag.WebAplicationsList = new SelectList(WebAplicationDetailsList,
                    "CatWebAplications.IdWebAplication", "CatWebAplications.WebAplicationName");

                #endregion

                if (objCache.GetMyCachedItem("SelectedCatStepId") != null)
                {

                    SelectedCatStepId = int.Parse(objCache.GetMyCachedItem("SelectedCatStepId").ToString());
                    objCache.RemoveMyCachedItem("SelectedCatStepId");

                    ViewBag.FormsProcessesList = new List<FormsProcess>(from d in db.FormsProcesses
                                                                        where d.IdStep == SelectedCatStepId
                                                                        orderby d.FormOrder ascending
                                                                        select d);
                    Model.IdStep = SelectedCatStepId;

                }

                if (objCache.GetMyCachedItem("Selected_IdWebAplication") != null)
                {

                    Selected_IdWebAplication = int.Parse(objCache.GetMyCachedItem("Selected_IdWebAplication").ToString());
                    objCache.RemoveMyCachedItem("Selected_IdWebAplication");

                    ViewBag.FormList = new SelectList(new List<CatForms>(from d in db.CatForms
                                                                         where d.IdWebAplication == Selected_IdWebAplication
                                                                         select d), "IdForm", "FormLabel");


                }
            }

            
            #endregion
            
            if (objCache.GetMyCachedItem("FormsProcess.IdFormProcess") != null)
            {
                ViewBag.IdFormsProcessAdded = int.Parse(objCache.GetMyCachedItem("FormsProcess.IdFormProcess").ToString());
                objCache.RemoveMyCachedItem("FormsProcess.IdFormProcess");
            }
            else
                ViewBag.IdFormsProcessAdded = -1;

            return View(Model);
        }

        [HttpPost]
        public ActionResult Index(string IdProcess, string IdStep, string IdWebAplication)
        {
            int _idProcess = 0;
            int _idStep = 0;
            int nIdWebAplication = 0;
            bool isNumeric = false;

            isNumeric = int.TryParse(IdProcess, out _idProcess);
            isNumeric = int.TryParse(IdStep, out _idStep);
            isNumeric = int.TryParse(IdWebAplication, out nIdWebAplication);

            ViewBag.DeleteStepStatus = string.Empty;
            ViewBag.IdFormsProcessAdded = -1;

            FormsProcess Model = new FormsProcess();


            Model.IdProcess = _idProcess;

            List<CatSteps> _CatStepsList = new List<CatSteps>(from d in db.CatSteps
                                                              where d.IdProcess == _idProcess
                                                              orderby d.StepName
                                                              select d);




            ViewBag.FormList = new SelectList(new List<CatForms>(from d in db.CatForms
                                                                 where d.IdWebAplication == nIdWebAplication
                                                                 select d), "IdForm", "FormLabel");

            ViewBag.ProcessDLL = new SelectList(db.CatProcesses, "IdProcess", "ProcessName");
            ViewBag.IdStepDLL = new SelectList(_CatStepsList, "IdStep", "StepName");

            #region Web Aplication list

            List<WebAplicationDetails> WebAplicationDetailsList = new List<WebAplicationDetails>(from d in db.WebAplicationDetails
                                                                                     where d.IdProcess == _idProcess
                                                                                     select d);

            ViewBag.WebAplicationsList = new SelectList(WebAplicationDetailsList, 
                "CatWebAplications.IdWebAplication", "CatWebAplications.WebAplicationName");

            #endregion


            if (_idStep != 0)
            {
                ViewBag.FormsProcessesList = new List<FormsProcess>(from d in db.FormsProcesses
                                                                    where d.IdStep == _idStep
                                                                    orderby d.FormOrder ascending
                                                                    select d);


                Model.IdStep = _idStep;
            }
            else
                ViewBag.FormsProcessesList = null;

            if (_idProcess == 0)
            {
                ViewBag.FormsProcessesList = null;
                Model = new FormsProcess();
                ModelState.Clear();
            }

            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFormProcess(FormsProcess Model)
        {
            var _FormsProcess = db.FormsProcesses;

            if (ModelState.IsValid)
            {
                db.FormsProcesses.Add(Model);
                db.SaveChanges();

                Model.CatSteps = db.CatSteps.Find(Model.IdStep);

                objCache.AddToMyCache("SelectedCatStepId", Model.IdStep, MyCachePriority.Default);
                objCache.AddToMyCache("SelectedCatProcessesId", Model.CatSteps.IdProcess, MyCachePriority.Default);
                objCache.AddToMyCache("FormsProcess.IdFormProcess", Model.IdFormProcess, MyCachePriority.Default);
                objCache.AddToMyCache("Selected_IdWebAplication", Model.IdWebAplication, MyCachePriority.Default);

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DeleteFormProcess(int id)
        {
            FormsProcess Model = db.FormsProcesses.Find(id);

            objCache.AddToMyCache("SelectedCatStepId", Model.IdStep, MyCachePriority.Default);
            objCache.AddToMyCache("SelectedCatProcessesId", Model.CatSteps.IdProcess, MyCachePriority.Default);
            objCache.AddToMyCache("Selected_IdWebAplication", Model.IdWebAplication, MyCachePriority.Default);
       
            db.FormsProcesses.Remove(Model);
            db.SaveChanges();

            objCache.AddToMyCache("DeleteFormStatus", "success", MyCachePriority.Default);

            return RedirectToAction("Index");
        }

        #endregion

        #region StepMaintenance

        public ActionResult StepMaintenance()
        {
            CatSteps _newCatSteps = new CatSteps();
            int id = 0;
            int UpdateStepMaintenanceId = 0;

            ViewBag.StepToUpdateId = -1;
            ViewBag.CatProcessesList = new SelectList(db.CatProcesses.OrderBy(p => p.ProcessName), "IdProcess", "ProcessName");

            if (objCache.GetMyCachedItem("UpdateStepMaintenanceId") != null)
            {
                UpdateStepMaintenanceId = int.Parse(objCache.GetMyCachedItem("UpdateStepMaintenanceId").ToString());
                objCache.RemoveMyCachedItem("UpdateStepMaintenanceId");
                _newCatSteps = db.CatSteps.Find(UpdateStepMaintenanceId);

                ViewBag.CatProcessesList = new SelectList(db.CatProcesses, "IdProcess", "ProcessName", _newCatSteps.IdProcess);
                ViewBag.CatStepsList = new List<CatSteps>(from d in db.CatSteps where d.IdProcess == _newCatSteps.IdProcess select d);

                ViewBag.StepToUpdateId = UpdateStepMaintenanceId;
            }

            if (objCache.GetMyCachedItem("DeleteStepStatus") != null)
            {
                ViewBag.DeleteStepStatus = objCache.GetMyCachedItem("DeleteStepStatus").ToString();
                objCache.RemoveMyCachedItem("DeleteStepStatus");
            }
            else
                ViewBag.DeleteStepStatus = string.Empty;

            if (objCache.GetMyCachedItem("SelectedCatProcessesId") != null)
            {
                id = int.Parse(objCache.GetMyCachedItem("SelectedCatProcessesId").ToString());
                objCache.RemoveMyCachedItem("SelectedCatProcessesId");
            }

            if (objCache.GetMyCachedItem("CatSteps.IdStep") != null)
            {
                ViewBag.IdStepAdded = int.Parse(objCache.GetMyCachedItem("CatSteps.IdStep").ToString());
                objCache.RemoveMyCachedItem("CatSteps.IdStep");
            }
            else
                ViewBag.IdStepAdded = -1;

            ViewBag.SelectedCatProcessesId = id;

            if (id != 0)
            {
                ViewBag.CatStepsList = new List<CatSteps>(from d in db.CatSteps
                                                          where d.IdProcess == id
                                                          orderby d.StepName
                                                          select d);
                ViewBag.CatProcessesList = new SelectList(db.CatProcesses.OrderBy(p => p.ProcessName), "IdProcess", "ProcessName", id);
                _newCatSteps.IdProcess = id;
            }

            return View(_newCatSteps);
        }

        [HttpPost]
        public ActionResult StepMaintenance(string IdProcess)
        {

            CatSteps _newCatSteps = new CatSteps();
            int id = 0;



            bool isNumeric = int.TryParse(IdProcess, out id);
            ViewBag.SelectedCatProcessesId = id;
            ViewBag.IdStepAdded = -1;
            ViewBag.StepToUpdateId = -1;

            if (id != 0)
            {
                ViewBag.CatStepsList = new List<CatSteps>(from s in db.CatSteps
                                                          where s.IdProcess == id
                                                          orderby s.StepName
                                                          select s);
                ViewBag.CatProcessesList = new SelectList(db.CatProcesses.OrderBy(p => p.ProcessName), "IdProcess", "ProcessName", id);

                _newCatSteps.IdProcess = id;
            }
            else
                ViewBag.CatProcessesList = new SelectList(db.CatProcesses.OrderBy(p => p.ProcessName), "IdProcess", "ProcessName");

            return View(_newCatSteps);
        }

        [HttpPost]
        public ActionResult CreateStep(CatSteps _CatSteps)
        {
            if (ModelState.IsValid)
            {
                if (db.CatSteps.Any(o => o.IdStep == _CatSteps.IdStep))
                {
                    db.Entry(_CatSteps).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    db.CatSteps.Add(_CatSteps);
                }
                db.SaveChanges();

                objCache.AddToMyCache("SelectedCatProcessesId", _CatSteps.IdProcess, MyCachePriority.Default);
                objCache.AddToMyCache("CatSteps.IdStep", _CatSteps.IdStep, MyCachePriority.Default);
            }
            return RedirectToAction("StepMaintenance");
        }

        [HttpGet]
        public ActionResult DeleteStep(int id)
        {
            CatSteps _CatSteps = db.CatSteps.Find(id);

            objCache.AddToMyCache("SelectedCatProcessesId", _CatSteps.IdProcess, MyCachePriority.Default);

            if (_CatSteps.FormsProcess.Count == 0)
            {
                db.CatSteps.Remove(_CatSteps);
                db.SaveChanges();

                objCache.AddToMyCache("DeleteStepStatus", "success", MyCachePriority.Default);
            }
            else
                objCache.AddToMyCache("DeleteStepStatus", "error", MyCachePriority.Default);

            return RedirectToAction("StepMaintenance");
        }

        [HttpGet]
        public ActionResult UpdateStepMaintenance(int id)
        {
            objCache.AddToMyCache("UpdateStepMaintenanceId", id, MyCachePriority.Default);
            return RedirectToAction("StepMaintenance");
        }

        #endregion

        #region ProcessMaintenance

        public ActionResult ProcessMaintenance()
        {
            CatProcesses newCatProcesses = new CatProcesses();
            int UpdateProcessMaintenanceId = 0;
            ViewBag.CatProcessesList = db.CatProcesses.OrderBy(p=>p.ProcessName);
            ViewBag.ConnectionOriginList = new SelectList(db.CatConnections, "IdConnections", "ConnectionName");
            ViewBag.ConnectionDestinationList = new SelectList(db.CatConnections, "IdConnections", "ConnectionName");

            ViewBag.ProcessToUpdateId = -1;

            if (objCache.GetMyCachedItem("UpdateProcessMaintenanceId") != null)
            {
                UpdateProcessMaintenanceId = int.Parse(objCache.GetMyCachedItem("UpdateProcessMaintenanceId").ToString());
                objCache.RemoveMyCachedItem("UpdateProcessMaintenanceId");
                newCatProcesses = db.CatProcesses.Find(UpdateProcessMaintenanceId);

                ViewBag.ProcessToUpdateId = UpdateProcessMaintenanceId;

            }

            if (objCache.GetMyCachedItem("DeleteProcessStatus") != null)
            {
                ViewBag.DeleteProcessStatus = objCache.GetMyCachedItem("DeleteProcessStatus").ToString();
                objCache.AddToMyCache("DeleteProcessStatus", string.Empty, MyCachePriority.Default);
            }
            else
                ViewBag.DeleteProcessStatus = string.Empty;


            if (objCache.GetMyCachedItem("ProcessMaintenanceID") != null)
            {
                ViewBag.lastAdded = int.Parse(objCache.GetMyCachedItem("ProcessMaintenanceID").ToString());
                objCache.AddToMyCache("ProcessMaintenanceID", -1, MyCachePriority.Default);
            }
            else
                ViewBag.lastAdded = -1;

            return View(newCatProcesses);
        }

        [HttpPost]
        public ActionResult ProcessMaintenance(CatProcesses _CatProcesses)
        {
            var _CatProcessesList = db.CatProcesses;

            if (ModelState.IsValid)
            {
                if (db.CatProcesses.Any(o => o.IdProcess == _CatProcesses.IdProcess))
                {
                    db.Entry(_CatProcesses).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    db.CatProcesses.Add(_CatProcesses);
                    db.SaveChanges();

                    db.HistoryConfigurations.Add(
                     new HistoryConfiguration
                     {
                         IdProcess = _CatProcesses.IdProcess,
                         CompletedTaskDeleted = true,
                         IncidentAborted = true,
                         IncidentCompleted = true,
                         IncidentInitiated = true,
                         QueueTaskActivated = true,
                         StepAborted = true,
                         TaskActivated = true,
                         TaskAssigned = true,
                         TaskCompleted = true,
                         TaskConferred = true,
                         TaskDelayed = true,
                         TaskDeletedOnMinResponseComplete = true,
                         TaskLate = true,
                         TaskResubmitted = true,
                         TaskReturned = true,
                         TasksPerDayThresholdReached = true,
                         TaskSubmitFailed = true
                     });
                    db.SaveChanges();

                }

                objCache.AddToMyCache("ProcessMaintenanceID", _CatProcesses.IdProcess, MyCachePriority.Default);
            }
            return RedirectToAction("ProcessMaintenance");
        }


        [HttpGet]
        public ActionResult DeleteProcess(int id)
        {
            CatProcesses _CatProcesses = db.CatProcesses.Find(id);

            if (_CatProcesses.CatSteps.Count == 0)
            {
                db.CatProcesses.Remove(_CatProcesses);
                db.SaveChanges();

                objCache.AddToMyCache("DeleteProcessStatus", "success", MyCachePriority.Default);
            }
            else
                objCache.AddToMyCache("DeleteProcessStatus", "error", MyCachePriority.Default);

            return RedirectToAction("ProcessMaintenance");
        }


        [HttpGet]
        public ActionResult UpdateProcessMaintenance(int id)
        {
            objCache.AddToMyCache("UpdateProcessMaintenanceId", id, MyCachePriority.Default);
            return RedirectToAction("ProcessMaintenance");
        }
        #endregion
    }
}
