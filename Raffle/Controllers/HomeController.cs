﻿using Raffle.Models;
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
            var context = new Context();
            UserProfile user = context.UserProfiles.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (user == null)
                return View();

            ViewBag.CloseCall = context.Items.Where(i => i.TotalRaffleCount - i.Raffles.Count != 0 && i.OwnerId != user.UserId)
                                             .OrderBy(i => i.TotalRaffleCount - i.Raffles.Count)
                                             .Take(6)
                                             .AsQueryable();

            ViewBag.Latest = context.Items.Where(i => i.OwnerId != user.UserId)
                                          .OrderByDescending(i => i.CreatedAt)
                                          .Take(6)
                                          .AsQueryable();

            ViewBag.User = context.UserProfiles.First(u => u.UserName == User.Identity.Name);

            return View("Main");
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
