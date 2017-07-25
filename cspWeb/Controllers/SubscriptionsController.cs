using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cspWeb.Models;
using cspWeb.Helpers;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace cspWeb.Controllers
{
    [Authorize]
    public class SubscriptionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Subscriptions
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.Subscriptions.ToList());
            } else
            {
                //Get customer ID corresponding to this user
                string userId = User.Identity.GetUserId();
                //Get customers
                var customerList = new List<Models.Customer>();
                customerList = db.Customers.Where(c => c.OwnerId == userId).ToList();
                if (customerList.Count > 0)
                {
                    var subscriptionList = new List<Models.Subscription>();
                    foreach (var customer in customerList)
                    {
                        var customerId = customer.CustomerId;
                        var thisSubscriptionList = db.Subscriptions.Where(s => s.CustomerId == customerId).ToList();
                        subscriptionList = subscriptionList.Concat(thisSubscriptionList).ToList();
                    }
                    return View(subscriptionList);
                }
                else
                {
                    // If no customer was found for this User Id
                    return RedirectToAction("Create");
                }

            }
        }

        // GET: Subscriptions/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // GET: Subscriptions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubscriptionId,CustomerId")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                db.Subscriptions.Add(subscription);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(subscription);
        }

        // GET: Subscriptions/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SubscriptionId,CustomerId")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subscription).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Subscription subscription = db.Subscriptions.Find(id);
            db.Subscriptions.Remove(subscription);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Subscriptions/CreateVault/5
        public ActionResult CreateVault(string id)
        {
            if (id == null)
            {
                // Instead of returning BadRequest, we try first to find out the subscription
                string userId = User.Identity.GetUserId();
                var customers = db.Customers.ToList();
                var customerList = new List<Models.Customer>();
                customerList = db.Customers.Where(c => c.OwnerId == userId).ToList();
                if (customerList.Count > 0)
                {
                    var subscriptionList = new List<Models.Subscription>();
                    foreach (var customer in customerList)
                    {
                        var customerId = customer.CustomerId;
                        var thisSubscriptionList = db.Subscriptions.Where(s => s.CustomerId == customerId).ToList();
                        subscriptionList = subscriptionList.Concat(thisSubscriptionList).ToList();
                    }
                    if (subscriptionList.Count == 1)
                    {
                        // We pick up the first subscription
                        // An alternative would be to send the user to the Subscriptions/Index view, so that he chooses the right one
                        id = subscriptionList[0].SubscriptionId;
                        return View(subscriptionList[0]);
                    }
                    if (subscriptionList.Count > 1)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        return RedirectToAction("Create");
                    }
                }
                else
                {
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    return RedirectToAction("../Customers/Create");
                }
            } else
            {
                Subscription subscription = db.Subscriptions.Find(id);
                if (subscription == null)
                {
                    return HttpNotFound();
                }
                return View(subscription);
            }
        }

        // POST: Subscriptions/CreateVault/5
        [HttpPost, ActionName("CreateVault")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateVaultConfirmed(Models.Subscription sub)
        {
            if (sub == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            string subscriptionId = sub.SubscriptionId;
            Subscription subscription = db.Subscriptions.Find(subscriptionId);
            if (subscription == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            // Create ARM Resource Group and Recovery Services Vault
            await ARM.createResourceGroupAsync(subscription.CustomerId, subscription.SubscriptionId, "testRg", "westeurope");
            string VaultId = await ARM.createSRVaultAsync(subscription.CustomerId, subscription.SubscriptionId, "testRg", "testVault", "westeurope");

            Models.Service newService = new Models.Service()
            {
                SubscriptionId = subscription.SubscriptionId,
                Description = "RSVault",
                Id = VaultId
            };
            db.Services.Add(newService);
            db.SaveChanges();
            return RedirectToAction("../Home/Index");
        }


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
