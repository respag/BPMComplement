using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ultimus.AuditManager.Admin.Commons;
using Ultimus.ComponentManager.Models;

namespace Ultimus.AuditManager.Admin.Controllers
{
    public class WebAplicationsController : ApiController
    {

        ComponentManagerContext db = Utilities.GetComponentManagerContextForApi();
        CacheManager objCache = new CacheManager();

        public IEnumerable<CatWebAplications> GetAllWebAplications(int id)
        {
            IEnumerable<CatWebAplications> WebAplicationsList
                = db.CatWebAplications.ToList();

            IEnumerable<WebAplicationDetails> waDetailsList
                = db.WebAplicationDetails.Where(d => d.IdProcess == id).ToList();

            foreach (CatWebAplications item in WebAplicationsList)
            {
                if (waDetailsList.Where(d => d.IdWebAplication == item.IdWebAplication).Count() > 0)
                {
                    item.IdProcess = id;

                    if (db.FormsProcesses.Include("CatSteps").Include("CatForms").Where(d => d.CatSteps.IdProcess == item.IdProcess
                        && d.CatForms.IdWebAplication == item.IdWebAplication).Count() > 0)
                    {
                        item.isAddedToProcess = true;
                    }
                    else
                    {
                        item.isAddedToProcess = false; ;
                    }

                }
            }

            return WebAplicationsList;
        }


        public string SaveWebAplicationsRelation([FromBody] SaveWebAplicationsRequest Request)
        {
            try
            {

                WebAplicationDetails Detail = new WebAplicationDetails();
                Detail.IdWebAplication = Request.IdWebAplication;
                Detail.IdProcess = Request.IdProcess;

                db.WebAplicationDetails.Add(Detail);

                db.SaveChanges();

                return "DONE";
            }
            catch (Exception ex) {

                return "FAIL";
            
            }
        }

        public string RemoveWebAplicationsRelation([FromBody] SaveWebAplicationsRequest Request)
        {
            try
            {
                WebAplicationDetails Detail = db.WebAplicationDetails.Where(d => d.IdProcess == Request.IdProcess
                    && d.IdWebAplication == Request.IdWebAplication).FirstOrDefault();

                db.WebAplicationDetails.Remove(Detail);

                db.SaveChanges();

                return "DONE";
            }
            catch (Exception ex)
            {
                return "FAIL";

            }
        }
    }

    public class SaveWebAplicationsRequest {

        public int IdWebAplication { get; set; }
        public int IdProcess { get; set; } 
    }
}
