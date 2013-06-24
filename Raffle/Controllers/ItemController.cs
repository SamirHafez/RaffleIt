using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raffle.Models;
using Raffle.Core;
using Raffle.App_Start;
using PayPal.ButtonManager;

namespace Raffle.Controllers
{
    [Authorize]
    public class ItemController : BaseController
    {
        public ActionResult Index(int id)
        {
            ViewBag.User = Context.UserProfiles.First(u => u.UserName == User.Identity.Name);

            Item item = Context.Items.Find(id);

            ViewBag.Related = Context.Items.Where(i => i.CategoryId == item.CategoryId && i.Id != item.Id)
                                           .OrderByDescending(i => i.CreatedAt)
                                           .Take(6)
                                           .ToList();

            return View(item);
        }

        //[HttpPost]
        //public ActionResult Index(int id)
        //{
        //    Item item = db.Items.Find(id);
        //    UserProfile user = db.UserProfiles.First(u => u.UserName == User.Identity.Name);

        //    if(!ModelState.IsValid)
        //        return RedirectToAction("Index", id);

        //    var im = new ItemManager(item);

        //    try
        //    {
        //        var raffle = im.BuyRaffle(user);
        //        db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        ModelState.AddModelError(string.Empty, e);
        //    }

        //    return RedirectToAction("Index", id);
        //}

        public ActionResult Create()
        {
            ViewBag.Categories = Context.Categories.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Create(Item item)
        {
            if (ModelState.IsValid)
            {
                UserProfile user = Context.UserProfiles.First(u => u.UserName == User.Identity.Name);

                item.CreatedAt = DateTime.Now;
                user.Items.Add(item);
                Context.SaveChanges();

                ButtonManagerResponse buyButton = BuyNowButton.Create(new HtmlButtonVariables 
                {
                    Business = PaypalConfig.BusinessEmail,
                    Quantity = "1",
                    Amount = item.RafflePrice.ToString().Replace(",", "."),
                    CurrencyCode = "EUR",
                    ItemName = string.Format("Raffle_{0}", item.Id)
                });

                item.PaypalCode = buyButton.WebSiteCode;

                return RedirectToAction("Index", new { id = item.Id });
            }

            return View(item);
        }
    }
}