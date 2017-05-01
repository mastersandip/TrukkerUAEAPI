using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
//using System.Web.Http.Cors;

namespace trukkerUAE
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //EnableCrossSiteRequests(config);
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            
            config.Routes.MapHttpRoute(
                name: "OwnerApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

        }

        //private static void EnableCrossSiteRequests(HttpConfiguration config)
        //{
        //    var cors = new EnableCorsAttribute(
        //        origins: "*",
        //        headers: "*",
        //        methods: "*");
        //    config.EnableCors(cors);
        //}
    }
}
