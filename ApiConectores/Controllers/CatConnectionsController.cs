using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ApiConectores.Models;
using Ultimus.UtilityLayer;
using System.Web.Http.Cors;

namespace ApiConectores.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CatConnectionsController : ApiController
    {
        private Model db = new Model();

        // GET: api/CatConnections
        public IQueryable<CatConnection> GetCatConnections()
        {
            return db.CatConnections;
        }

        // GET: api/CatConnections/5
        [ResponseType(typeof(CatConnection))]
        public IHttpActionResult GetCatConnection(int id)
        {
            CatConnection catConnection = db.CatConnections.Find(id);
            if (catConnection == null)
            {
                return NotFound();
            }

            return Ok(catConnection);
        }

        // PUT: api/CatConnections/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCatConnection(int id, [FromBody] CatConnection catConnection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != catConnection.IdConnections)
            {
                return BadRequest();
            }

            try
            {
                var objChanged = db.CatConnections.Find(id);
                objChanged.ConnectionName = catConnection.ConnectionName;
                
                Crypt crypt = new Crypt();
                string cs = crypt.EncryptString(catConnection.ConnectionString);
                catConnection.ConnectionString = cs;
                objChanged.ConnectionString = catConnection.ConnectionString;
                objChanged.IdConnectionType = catConnection.IdConnectionType;
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatConnectionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/CatConnections
        [ResponseType(typeof(CatConnection))]
        public IHttpActionResult PostCatConnection([FromBody]CatConnection catConnection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Crypt crypt = new Crypt();
            string cs = crypt.EncryptString(catConnection.ConnectionString);
            catConnection.ConnectionString = cs;
            
            db.Entry(catConnection).State = EntityState.Modified;
            db.CatConnections.Add(catConnection);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = catConnection.IdConnections }, catConnection);
        }

        // DELETE: api/CatConnections/5
        [ResponseType(typeof(CatConnection))]
        public IHttpActionResult DeleteCatConnection(int id)
        {
            CatConnection catConnection = db.CatConnections.Find(id);
            if (catConnection == null)
            {
                return NotFound();
            }

            db.CatConnections.Remove(catConnection);
            db.SaveChanges();

            return Ok(catConnection);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CatConnectionExists(int id)
        {
            return db.CatConnections.Count(e => e.IdConnections == id) > 0;
        }
    }
}