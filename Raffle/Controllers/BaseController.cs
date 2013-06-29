using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raffle.Models;

namespace Raffle.Controllers
{
    public abstract class BaseController : Controller
    {
        protected Context Context { get; private set; }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            Context = new Context();
            base.Initialize(requestContext);
        }

        protected override void Dispose(bool disposing)
        {
            using (Context)
                Context.SaveChanges();
            base.Dispose(disposing);
        }
    }
}
