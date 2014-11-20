using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Ultimus.ComponentManager.Models;

namespace Ultimus.AuditManager.Admin.Controllers
{
    public class ConnectionsController : ApiController
    {
        private ComponentManagerContext db = new ComponentManagerContext(@"Data Source=EHERNANDEZPC\MSSQLSERVER_EIH;Initial Catalog=Ultimus.AuditManager;User ID=sa;Password=12345678a;MultipleActiveResultSets=True");

        // GET api/Connections
        public IEnumerable<CatConnections> GetConnections()
        {
            var connections = db.CatConnections.Include(c => c.CatConnectionType);
            return connections.AsEnumerable();
        }

        // GET api/Connections/5
        public CatConnections GetConnections(int id)
        {
            CatConnections connections = db.CatConnections.Find(id);
            if (connections == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return connections;
        }

        // PUT api/Connections/5
        public HttpResponseMessage PutConnections(int id, CatConnections connections)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != connections.IdConnections)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(connections).State = System.Data.Entity.EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Connections
        public HttpResponseMessage PostConnections(CatConnections connections)
        {
            if (ModelState.IsValid)
            {
                db.CatConnections.Add(connections);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, connections);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = connections.IdConnections }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Connections/5
        public HttpResponseMessage DeleteConnections(int id)
        {
            CatConnections connections = db.CatConnections.Find(id);
            if (connections == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.CatConnections.Remove(connections);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, connections);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}