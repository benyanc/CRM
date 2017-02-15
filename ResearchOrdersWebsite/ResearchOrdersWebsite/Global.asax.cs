using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MongoDB.Driver;

namespace ResearchOrdersWebsite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            #if !DEBUG
            // SECURE: Ensure any request is returned over SSL/TLS in production
            if (!Request.IsLocal && !Context.Request.IsSecureConnection) {
                var redirect = Context.Request.Url.ToString().ToLower(CultureInfo.CurrentCulture).Replace("http:", "https:");
                Response.Redirect(redirect);
            }
            #endif
            Application["mongoClient"] = new MongoClient("mongodb://localhost:27017");
            Application["mongoDB"] = ((MongoClient)Application["mongoClient"]).GetDatabase("research_orders_db");
        }
        protected void Session_Start()
        {
            //Session["dummy"] = 1;
            Console.WriteLine("Session starting");
        }
    }
}
