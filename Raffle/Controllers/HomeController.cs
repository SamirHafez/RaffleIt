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

            IList<Item> latest = Context.Items.OrderByDescending(i => i.CreatedAt)
                                              .Take(18)
                                              .ToList();

            return View("Latest", latest);
        }

        public ActionResult Ending()
        {
            UserProfile user = Context.UserProfiles.First(u => u.UserName == User.Identity.Name);

            IList<Item> ending = Context.Items.Where(i => i.ClosedAt == null)
                                              .OrderBy(i => i.TotalRaffleCount - i.Raffles.Count)
                                              .Take(18)
                                              .ToList();

            return View(ending);
        }

        public ActionResult Search(string query, int page = 0)
        {
            UserProfile user = Context.UserProfiles.First(u => u.UserName == User.Identity.Name);

            ViewBag.HasMore = Context.Items.Where(i => i.Name.Contains(query) || i.Description.Contains(query) || i.Category.Name.Contains(query))
                                           .OrderByDescending(i => i.CreatedAt)
                                           .Skip((page * 18) + 18)
                                           .Any();

            ViewBag.HasLess = page != 0;

            ViewBag.Page = page;

            IList<Item> results = Context.Items.Where(i => i.Name.Contains(query) || i.Description.Contains(query) || i.Category.Name.Contains(query))
                                               .OrderByDescending(i => i.CreatedAt)
                                               .Skip(page * 18)
                                               .Take(18)
                                               .ToList();

            return View(results);
        }

        public ActionResult Category(int id, int page = 0)
        {
            ViewBag.Category = Context.Categories.Find(id);

            ViewBag.HasMore = Context.Items.Where(i => i.ClosedAt == null && i.CategoryId == id)
                                           .OrderByDescending(i => i.CreatedAt)
                                           .Skip((page * 18) + 18)
                                           .Any();

            ViewBag.HasLess = page != 0;

            ViewBag.Page = page;


            IList<Item> results = Context.Items.Where(i => i.ClosedAt == null && i.CategoryId == id)
                                               .OrderByDescending(i => i.CreatedAt)
                                               .Skip(page * 18)
                                               .Take(18)
                                               .ToList();

            return View(results);
        }
    }
}
