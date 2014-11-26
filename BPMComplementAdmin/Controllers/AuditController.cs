using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Ultimus.AuditManager.Admin.Commons;
using Ultimus.ComponentManager.Models;

namespace Ultimus.AuditManager.Admin.Controllers
{
    public class AuditController : Controller
    {
       
        ComponentManagerContext db = Utilities.GetComponentManagerContext();
        CacheManager objCache = new CacheManager();

        public ActionResult Prueba()
        {
            return View();
        }

        //Método que verifica  si un conector es válido
        public bool ValidarConexion(string connstring, string tipo)
        {
            if (tipo == "Microsoft SQL Server")
            {
                using (SqlConnection conn = new SqlConnection(connstring))
                {
                    try
                    {

                        conn.Open();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

            }
            else
            {
                try
                {
                    using (OracleConnection conn = new OracleConnection(connstring))
                    {
                        try
                        {
                            conn.Open();
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }


        public ActionResult Index()
        {
            ViewBag.AuditInstalled = Utilities.IsAuditInstalled();
            ViewBag.ProcessInstalled = Utilities.IsProcessInstalled();

            if (ViewBag.AuditInstalled)
                return View();
            else
                return RedirectToAction("LogIn", "UserAuthentication");
        }

        public ActionResult ConnectionConfiguration()
        {
            ViewBag.Id ="-1";

            CatConnections Model = new CatConnections();

            ViewBag.ConnectionsList = db.CatConnections.OrderBy(c => c.ConnectionName).ToList();
            ViewBag.ConnectionTypesList = new SelectList(db.ConnectionTypes, "IdConnectionType", "ConnectionName"); 

            return View(Model);
        }

       
        [HttpPost]
        public ActionResult CreateUpdateConnection(CatConnections Model)
        {

            if (ModelState.IsValid)
            {
                Model.ConnectionString = Model.ConnectionStringEncrypted;

                if (db.CatConnections.Any(o => o.IdConnections == Model.IdConnections))
                {
                    db.Entry(Model).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    db.CatConnections.Add(Model);
                }

                Model.isUpdatedOrCreated = true;

                db.SaveChanges();
                ModelState.Clear();

                Model = new CatConnections();
                Model.isUpdatedOrCreated = true;
            }

            ViewBag.ConnectionsList = db.CatConnections.ToList();
            ViewBag.ConnectionTypesList = new SelectList(db.ConnectionTypes, "IdConnectionType", "ConnectionName");

            return View("ConnectionConfiguration", Model);
        }

        [HttpGet]
        public ActionResult UpdateConnection(int id)
        {
            ViewBag.Id = id;
            CatConnections Model = new CatConnections();

            Model = db.CatConnections.Find(id);

            ViewBag.ConnectionsList = db.CatConnections.OrderBy(c=>c.ConnectionName).ToList();
            ViewBag.ConnectionTypesList = new SelectList(db.ConnectionTypes, "IdConnectionType", "ConnectionName", Model.IdConnectionType);

            Model.toUpdate = true;
            Model.ConnectionString = Model.ConnectionStringDecrypted;

            return View("ConnectionConfiguration", Model);
        }

        [HttpGet]
        public ActionResult DeleteConnection(int id)
        {
            CatConnections Model = db.CatConnections.Find(id);

            if (Model.AuditConnectionOrigin.Count == 0 && Model.AuditConnectionDestination.Count == 0)
            {
                db.CatConnections.Remove(Model);
                db.SaveChanges();

                ModelState.Clear();
                Model = new CatConnections();
                Model.isDeleted = true;
            }
            else {
                ModelState.Clear();
                Model = new CatConnections();
                Model.inUse = true;
            }

            ViewBag.ConnectionsList = db.CatConnections.OrderBy(c=>c.ConnectionName).ToList();
            ViewBag.ConnectionTypesList = new SelectList(db.ConnectionTypes, "IdConnectionType", "ConnectionName");

            return View("ConnectionConfiguration", Model);
        }


        #region ProcessMaintenance


        public ActionResult ProcessMaintenance()
        {
            CatProcesses newCatProcesses = new CatProcesses();
            int UpdateProcessMaintenanceId = 0;
            ViewBag.CatProcessesList = db.CatProcesses.OrderBy(p => p.ProcessName);
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
                }
                else
                {
                    db.CatProcesses.Add(_CatProcesses);
                }
                db.SaveChanges();

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

        #region StepMaintenance

        public ActionResult StepMaintenance()
        {
            CatSteps _newCatSteps = new CatSteps();
            int id = 0;
            int UpdateStepMaintenanceId = 0;

            ViewBag.StepToUpdateId = -1;
            ViewBag.CatProcessesList = new SelectList(db.CatProcesses.OrderBy(p=>p.ProcessName), "IdProcess", "ProcessName");

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
                ViewBag.CatProcessesList = new SelectList(db.CatProcesses.OrderBy(p=>p.ProcessName), "IdProcess", "ProcessName", id);
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
                ViewBag.CatProcessesList = new SelectList(db.CatProcesses.OrderBy(p=>p.ProcessName), "IdProcess", "ProcessName", id);

                _newCatSteps.IdProcess = id;
            }
            else
                ViewBag.CatProcessesList = new SelectList(db.CatProcesses.OrderBy(p=>p.ProcessName), "IdProcess", "ProcessName");

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
    }

}