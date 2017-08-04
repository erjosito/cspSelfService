using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cspWeb.Models;

namespace cspWeb.Controllers
{
    public class OfferingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Offerings
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Offerings.ToList());
        }

        // GET: Offerings/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Offering offering = db.Offerings.Find(id);
            if (offering == null)
            {
                return HttpNotFound();
            }
            return View(offering);
        }

        // GET: Offerings/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Offerings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Description")] Offering offering)
        {
            if (ModelState.IsValid)
            {
                db.Offerings.Add(offering);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(offering);
        }

        // GET: Offerings/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Offering offering = db.Offerings.Find(id);
            if (offering == null)
            {
                return HttpNotFound();
            }
            return View(offering);
        }

        // POST: Offerings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Description")] Offering offering)
        {
            if (ModelState.IsValid)
            {
                db.Entry(offering).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(offering);
        }

        // GET: Offerings/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Offering offering = db.Offerings.Find(id);
            if (offering == null)
            {
                return HttpNotFound();
            }
            return View(offering);
        }

        // POST: Offerings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            Offering offering = db.Offerings.Find(id);
            db.Offerings.Remove(offering);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
