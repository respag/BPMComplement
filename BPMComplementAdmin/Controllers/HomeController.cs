using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ultimus.AuditManager.Admin.Commons;
using Ultimus.AuditManager.Admin.Migrations;
using Ultimus.ComponentManager.Models;

namespace Ultimus.AuditManager.Admin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.AuditInstalled = Utilities.IsAuditInstalled();
            ViewBag.ProcessInstalled = Utilities.IsProcessInstalled();


            using (var dbContext = Utilities.GetComponentManagerContext())
            {
                if (!dbContext.Database.Exists())
                {
                    return RedirectToAction("DataBaseNotExists");
                }
            }

            return View();
        }

        public ActionResult DataBaseNotExists()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDataBase()
        {
            using (var dbContext = Utilities.GetComponentManagerContext())
            {
                if (!dbContext.Database.Exists())
                {
                    dbContext.Database.Create();

                   
                    dbContext.ConnectionTypes.AddOrUpdate(
                      new CatConnectionTypes { IdConnectionType = 1, ConnectionName = "Microsoft SQL Server" },
                      new CatConnectionTypes { IdConnectionType = 2, ConnectionName = "Oracle" }
                    );

                    dbContext.CatAuditTablesStatus.AddOrUpdate(
                      new CatAuditTablesStatus { IdTablesStatus = "P", StatusName = "Publicada" },
                      new CatAuditTablesStatus { IdTablesStatus = "D", StatusName = "Disabled" },   
                      new CatAuditTablesStatus { IdTablesStatus = "N", StatusName = "New" },
                      new CatAuditTablesStatus { IdTablesStatus = "PC", StatusName = "Pendiente" }
                    );

                    dbContext.CatConnections.AddOrUpdate(
                     new CatConnections
                     {
                         ConnectionName = "Northwind",
                         IdConnectionType = 1,
                         ConnectionString = "N+qtkiW4v0DG4lmdPikdxVD6lSKv05JxGAtSCILGo8Jcm8QbMNBNvVGAZoRieaXe2HR0txsShv5nfIYQb3Ju9Sgw5HWNHkZ7EOBhOA8z2M6MLQX04hPa4yG13NEQhOS+rus1yHwA2Trc3g8dBIT4lasomIZKSIXfmPALJw8YZ9o="
                     },
                     new CatConnections
                     {
                         ConnectionName = "AuditTest",
                         IdConnectionType = 1,
                         ConnectionString = "N+qtkiW4v0DG4lmdPikdxVD6lSKv05JxGAtSCILGo8Jcm8QbMNBNvVGAZoRieaXe2HR0txsShv777MT1GwOsMz2vBz46rC5nEOBhOA8z2M6MLQX04hPa4yG13NEQhOS+rus1yHwA2Trc3g8dBIT4lasomIZKSIXfmPALJw8YZ9o=" 
                     }
                   );

                    dbContext.SaveChanges();

                }
            }
            return View();
        }

        public ActionResult DoPendingMigrations()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ExecutePendingMigrations()
        {

            using (var dbContext = Utilities.GetComponentManagerContext())
            {

                var configuration = new Ultimus.AuditManager.Admin.Migrations.Configuration();
                var migrator = new DbMigrator(configuration);

                if (!dbContext.Database.CompatibleWithModel(false))
                {
                    if (migrator.GetPendingMigrations().Any())
                    {
          
                        migrator.Update();
                    }
                }
            }

            return View();
        }

        public ActionResult DataBaseNoPendingMigrations()
        {
            return View();
        }

        public ActionResult Logo()
        {
            var content = System.IO.File.ReadAllBytes(Server.MapPath("~/images/logo.png"));
            return base.File(content, "image/png");
        }
        public ActionResult LogoSmall()
        {
            var content = System.IO.File.ReadAllBytes(Server.MapPath("~/images/logo.png"));
            return base.File(content, "image/png");
        }

        public ActionResult bpmcloud()
        {
            var content = System.IO.File.ReadAllBytes(Server.MapPath("~/images/bpmcloud.png"));
            return base.File(content, "image/png");
        }

        public ActionResult footerwatermark()
        {
            var content = System.IO.File.ReadAllBytes(Server.MapPath("~/images/footerwatermark.png"));
            return base.File(content, "image/png");
        }
    }
}
