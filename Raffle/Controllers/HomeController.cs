using Raffle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Raffle.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
                return View();

            var context = new Context();
            UserProfile user = context.UserProfiles.First(u => u.UserName == User.Identity.Name);

            var latest = context.Items.Where(i => i.OwnerId != user.UserId)
                                      .OrderByDescending(i => i.CreatedAt)
                                      .Take(15)
                                      .AsQueryable();

            return View("Latest", latest);
        }

        public ActionResult Ending()
        {
            var context = new Context();
            UserProfile user = context.UserProfiles.First(u => u.UserName == User.Identity.Name);

            var ending = context.Items.Where(i => i.ClosedAt != null && i.OwnerId != user.UserId)
                                      .OrderBy(i => i.TotalRaffleCount - i.Raffles.Count)
                                      .Take(15)
                                      .AsQueryable();

            return View(ending);
        }

        public ActionResult Search(string query, int skip = 0)
        {
            var context = new Context();
            UserProfile user = context.UserProfiles.First(u => u.UserName == User.Identity.Name);

            var results = context.Items.Where(i => i.Name.Contains(query) || i.Description.Contains(query) || i.Category.Contains(query))
                                       .OrderByDescending(i => i.CreatedAt)
                                       .Skip(skip)
                                       .Take(18)
                                       .AsQueryable();

            return View(results);
        }
    }
}
