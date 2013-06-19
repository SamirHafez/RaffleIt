using Raffle.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Raffle
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            PayPal.Profile.Initialize("rhymecheatEbay-facilitator_api1.gmail.com", "1371677373", "A99sWksTL1d4MqQV9XAcXgxm3nh2Aq6F79k2yMvITGUairYn2EnPxu.s", "sandbox");

            Database.SetInitializer<Context>(new DropCreateDatabaseIfModelChanges<Context>());
        }
    }
}