using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raffle.Models;

namespace Raffle.Controllers
{
    public class ItemController : Controller
    {
        private Context db = new Context();

        //
        // GET: /Item/

        public ActionResult Index(int? id)
        {
            if (!id.HasValue)
                return View("List", db.Items.ToList());

            return View(db.Items.Find(id));
        }

        [HttpPost]
        public ActionResult Index(int id, Raffle.Models.Raffle raffle)
        {
            Item item = db.Items.Find(id);
            UserProfile user = db.UserProfiles.First(u => u.UserName == User.Identity.Name);

            if (user.UserId == item.OwnerId)
                ModelState.AddModelError("raffleNumber", "You cannot buy raffles of your own items");

            if (!ModelState.IsValid)
            {
                raffle.UserProfile = user;

                item.Raffles.Add(raffle);

                db.SaveChanges();
            }

            return RedirectToAction("Index", id);
        }

        //
        // GET: /Item/Details/5

        public ActionResult Details(int id = 0)
        {
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
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
                user.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(item);
        }

        //
        // GET: /Item/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        //
        // POST: /Item/Edit/5

        [HttpPost]
        public ActionResult Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
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