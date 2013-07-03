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
            ViewBag.User = await Context.UserProfiles.FirstAsync(u => u.UserName == User.Identity.Name);

            Item item = await Context.Items.FindAsync(id);

            ViewBag.Related = await Context.Items.Where(i => i.CategoryId == item.CategoryId && i.Id != item.Id)
                                                 .OrderByDescending(i => i.CreatedAt)
                                                 .Take(6)
                                                 .ToListAsync();

            return View(item);
        }

        public async Task<ActionResult> Purchase(int id)
        {
            UserProfile user = Context.UserProfiles.First(u => u.UserName == User.Identity.Name);

            if(user.UnusedRaffles == 0)
                return RedirectToAction("Index", new { id });

            Item item = await Context.Items.FindAsync(id);

            lock (item)
            {
                int mostRecentRaffleNumber = item.Raffles.OrderByDescending(r => r.RaffleNumber)
                                                         .Select(r => r.RaffleNumber)
                                                         .FirstOrDefault();

                item.Raffles.Add(new Raffle.Models.Raffle 
                {
                    UserProfileId = user.UserId,
                    PurchasedAt = DateTime.Now,
                    RaffleNumber = mostRecentRaffleNumber + 1
                });

                user.UnusedRaffles--;

                Context.SaveChanges();

                if (item.Raffles.Count == item.Price + (int)(item.Price * 0.40))
                {
                    Raffle.Models.Raffle selectedRaffle = item.Raffles.OrderBy(r => Guid.NewGuid()).First();
                    IEnumerable<Raffle.Models.Raffle> otherRaffles = item.Raffles.Where(r => r.RaffleNumber != selectedRaffle.RaffleNumber);

                    selectedRaffle.IsPrized = true;

                    foreach (var raffle in otherRaffles)
                        raffle.IsPrized = false;

                    item.ClosedAt = DateTime.Now;
                }
            }

            return RedirectToAction("Index", new { id });
        }

        public async Task<ActionResult> Shipped(int id)
        {
            Item item = await Context.Items.FindAsync(id);

            item.ShippedAt = DateTime.Now;

            return RedirectToAction("Index", new { id });
        }

        public async Task<ActionResult> Received(int id)
        {
            Item item = await Context.Items.FindAsync(id);

            item.ReceivedAt = DateTime.Now;

            return RedirectToAction("Index", new { id });
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

                return RedirectToAction("Index", new { id = item.Id });
            }

            return View(item);
        }
    }
}