using Raffle.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Raffle.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            if (!Request.IsAuthenticated)
                return View();

            UserProfile user = await Context.UserProfiles.FirstAsync(u => u.UserName == User.Identity.Name);

            IList<Item> latest = await Context.Items.OrderByDescending(i => i.CreatedAt)
                                                    .Take(18)
                                                    .ToListAsync();

            ViewBag.User = user;

            return View("Latest", latest);
        }

        public async Task<ActionResult> Ending()
        {
            UserProfile user = await Context.UserProfiles.FirstAsync(u => u.UserName == User.Identity.Name);

            IList<Item> ending = await Context.Items.Where(i => i.ClosedAt == null)
                                                    .OrderBy(i => i.Price - i.Raffles.Count)
                                                    .Take(18)
                                                    .ToListAsync();

            ViewBag.User = user;

            return View(ending);
        }

        public async Task<ActionResult> Search(string query, int page = 0)
        {
            UserProfile user = await Context.UserProfiles.FirstAsync(u => u.UserName == User.Identity.Name);

            ViewBag.HasMore = await Context.Items.Where(i => i.Name.Contains(query) || i.Description.Contains(query) || i.Category.Name.Contains(query))
                                                 .OrderByDescending(i => i.CreatedAt)
                                                 .Skip((page * 18) + 18)
                                                 .AnyAsync();

            ViewBag.HasLess = page != 0;

            ViewBag.Page = page;

            ViewBag.User = user;

            IList<Item> results = await Context.Items.Where(i => i.Name.Contains(query) || i.Description.Contains(query) || i.Category.Name.Contains(query))
                                                     .OrderByDescending(i => i.CreatedAt)
                                                     .Skip(page * 18)
                                                     .Take(18)
                                                     .ToListAsync();

            return View(results);
        }

        public async Task<ActionResult> Category(int id, int page = 0)
        {
            ViewBag.Category = await Context.Categories.FindAsync(id);

            ViewBag.HasMore = await Context.Items.Where(i => i.ClosedAt == null && i.CategoryId == id)
                                                 .OrderByDescending(i => i.CreatedAt)
                                                 .Skip((page * 18) + 18)
                                                 .AnyAsync();

            ViewBag.HasLess = page != 0;

            ViewBag.Page = page;


            IList<Item> results = await Context.Items.Where(i => i.ClosedAt == null && i.CategoryId == id)
                                                     .OrderByDescending(i => i.CreatedAt)
                                                     .Skip(page * 18)
                                                     .Take(18)
                                                     .ToListAsync();

            return View(results);
        }
    }
}
