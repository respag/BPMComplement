using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace APILogInBPMComplement
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{dominio}/{user}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi2",
                routeTemplate: "api/{controller}/{dominio}/{user}/{password}"
            );
        }
    }
}
