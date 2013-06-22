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

        public ActionResult Index(int? id, int skip = 0)
        {
            ViewBag.User = db.UserProfiles.First(u => u.UserName == User.Identity.Name);

            Item item;

            if (!id.HasValue || (item = db.Items.Find(id)) == null)
                return View("List", db.Items.OrderByDescending(i => i.CreatedAt)
                                            .Skip(skip)
                                            .Take(20)
                                            .AsQueryable());

            ViewBag.Related = db.Items.Where(i => i.Category == item.Category && i.Id != item.Id)
                                      .OrderByDescending(i => i.CreatedAt)
                                      .Take(6)
                                      .AsQueryable();

            return View(item);
        }

        [HttpPost]
        public ActionResult Index(int id)
        {
            Item item = db.Items.Find(id);
            UserProfile user = db.UserProfiles.First(u => u.UserName == User.Identity.Name);

            if(!ModelState.IsValid)
                return RedirectToAction("Index", id);

            var im = new ItemManager(item);

            try
            {
                var raffle = im.BuyRaffle(user);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e);
            }

            return RedirectToAction("Index", id);
        }

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

        //
        // GET: /Item/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        //
        // POST: /Item/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}