using System.Web.Http;
using System.Web.Http.Cors;

namespace AssetAPIs
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable Cross-Origin Resource Sharing (CORS)
            //This enables CORS, allowing other apps(like your React frontend) to make HTTP requests to this API.
            //The* means “allow all origins, all headers, all methods.”
            //In production, you should restrict this(e.g., only allow your frontend’s domain).
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling =
    Newtonsoft.Json.NullValueHandling.Ignore;//When converting objects to JSON, DO NOT include properties whose value is null
            // Enable attribute-based routing
            config.MapHttpAttributeRoutes();
            // Define a default route for the API
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
//this file Defines how routing and CORS are handled
// Install-Package Microsoft.AspNet.WebApi.Cors
// Imports namespaces for Web API configuration and CORS (Cross-Origin Resource Sharing)
//using System.Web.Http;
//using System.Web.Http.Cors;





