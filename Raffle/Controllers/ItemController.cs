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
using System.Threading.Tasks;

namespace Raffle.Controllers
{
    [Authorize]
    public class ItemController : BaseController
    {
        public async Task<ActionResult> Index(int id)
        {
            ViewBag.User = Context.UserProfiles.First(u => u.UserName == User.Identity.Name);

            Item item = await Context.Items.FindAsync(id);

            ViewBag.Related = await Context.Items.Where(i => i.CategoryId == item.CategoryId && i.Id != item.Id)
                                                 .OrderByDescending(i => i.CreatedAt)
                                                 .Take(6)
                                                 .ToListAsync();

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

        public async Task<ActionResult> Create()
        {
            ViewBag.Categories = await Context.Categories.ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Item item)
        {
            if (ModelState.IsValid)
            {
                UserProfile user = await Context.UserProfiles.FirstAsync(u => u.UserName == User.Identity.Name);

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