using Raffle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Raffle.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
                return View();

            UserProfile user = Context.UserProfiles.First(u => u.UserName == User.Identity.Name);

            IList<Item> latest = Context.Items.Where(i => i.OwnerId != user.UserId)
                                              .OrderByDescending(i => i.CreatedAt)
                                              .Take(15)
                                              .ToList();

            return View("Latest", latest);
        }

        public ActionResult Ending()
        {
            UserProfile user = Context.UserProfiles.First(u => u.UserName == User.Identity.Name);

            IList<Item> ending = Context.Items.Where(i => i.ClosedAt != null && i.OwnerId != user.UserId)
                                              .OrderBy(i => i.TotalRaffleCount - i.Raffles.Count)
                                              .Take(15)
                                              .ToList();

            return View(ending);
        }

        public ActionResult Search(string query, int skip = 0)
        {
            UserProfile user = Context.UserProfiles.First(u => u.UserName == User.Identity.Name);

            IList<Item> results = Context.Items.Where(i => i.Name.Contains(query) || i.Description.Contains(query) || i.Category.Contains(query))
                                               .OrderByDescending(i => i.CreatedAt)
                                               .Skip(skip)
                                               .Take(18)
                                               .ToList();

            return View(results);
        }
    }
}
