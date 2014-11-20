using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace APILogInBPMComplement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IsLoguedController : ApiController
    {
        public string Get(string dominio, string user, string password="")
        {
            if (password == "")
                password = "aa";
            Ultimus.OC.Group grupo = new Ultimus.OC.Group();
            Ultimus.OC.OrgChart obj = new Ultimus.OC.OrgChart();
            bool res = obj.VerifyUser(dominio + '/' + user, password);
            if (res)
            {
                bool encontrado = obj.GetGroup("BPMComplementAdmin", out grupo);
                if (encontrado)
                    return "{\"isLogued\": " + grupo.IsMemberOfGroup(dominio + '/' + user).ToString().ToLower() + "}";
                else
                    return "{\"isLogued\": false}";
            }
            return "{\"isLogued\": false}";
        }
    }
}
