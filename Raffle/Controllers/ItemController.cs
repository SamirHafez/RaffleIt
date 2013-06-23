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

namespace Raffle.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private Context db = new Context();

        //
        // GET: /Item/

        public ActionResult Index(int id)
        {
            ViewBag.User = db.UserProfiles.First(u => u.UserName == User.Identity.Name);

            Item item = db.Items.Find(id);

            ViewBag.Related = db.Items.Where(i => i.Category == item.Category && i.Id != item.Id)
                                      .OrderByDescending(i => i.CreatedAt)
                                      .Take(6)
                                      .AsQueryable();

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

        //
        // GET: /Item/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Item/Create

        [HttpPost]
        public ActionResult Create(Item item)
        {
            if (ModelState.IsValid)
            {
                var user = db.UserProfiles.First(u => u.UserName == User.Identity.Name);

                item.CreatedAt = DateTime.Now;
                user.Items.Add(item);
                db.SaveChanges();

                var buyButton = PayPal.ButtonManager.BuyNowButton.Create(new PayPal.ButtonManager.HtmlButtonVariables 
                {
                    Business = PaypalConfig.BusinessEmail,
                    Quantity = "1",
                    Amount = item.RafflePrice.ToString().Replace(",", "."),
                    CurrencyCode = "EUR",
                    ItemName = string.Format("Raffle_{0}", item.Id)
                });

                item.PaypalCode = buyButton.WebSiteCode;
                db.SaveChanges();

                return RedirectToAction("Index", new { id = item.Id });
            }

            return View(item);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}