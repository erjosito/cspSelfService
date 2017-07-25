using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cspWeb.Models;
using Microsoft.Store.PartnerCenter;
using cspWeb.Properties;
using cspWeb.Helpers;
using Microsoft.AspNet.Identity;

namespace cspWeb.Controllers
{
    public class CustomersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Customers
        [Authorize]
        public ActionResult Index()
        {
            var csp = new Helpers.PartnerCenter();
            if (User.IsInRole("Admin"))
            {
                // We can either get the customers from the CSP API
                //var allCustomers = csp.GetCspCustomers();
                // Or from the DB :-)
                var allCustomers = db.Customers.ToList();
                return View(allCustomers);
            }
            else
            {
                var customerList = new List<Models.Customer>();
                //We can either get the customers from the CSP API
                //foreach (var customer in db.Customers)
                //{
                //    if (customer.OwnerId == User.Identity.GetUserId()) {
                //        var thisCustomer = csp.GetUserCspCustomers(customer.CustomerId);
                //        customerList.Add(thisCustomer);
                //    }
                //}

                // Or from the DB
                string userId = User.Identity.GetUserId();
                customerList = db.Customers.Where(c => c.OwnerId == userId).ToList();
                if (customerList.Count > 0)
                {
                    return View(customerList);
                }
                else
                {
                    // If no customer was found for this User Id
                    return RedirectToAction("Create");
                }
            }
        }

        // GET: Customer associated to own User Id
        [Authorize]
        public ActionResult MyCustomer()
        {
            var userId = User.Identity.GetUserId();
            var csp = new Helpers.PartnerCenter();
            var allCustomers = csp.GetCspCustomers();
            return View(allCustomers);
        }


        // GET: Customers/Details/5
        public ActionResult Details(string id)
        {
            var customer = new Customer();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "CustomerName")] CustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                // Instantiate class
                var csp = new Helpers.PartnerCenter();
                // Creat new customer
                string customerId = csp.CreateCspCustomer(customer.CustomerName, CreateSubscription: true);
                var newCustomer = new Models.Customer();
                newCustomer.OwnerId = User.Identity.GetUserId();
                newCustomer.CustomerId = customerId;
                newCustomer.CustomerName = customer.CustomerName;
                db.Customers.Add(newCustomer);
                // Create new subscription
                string subscriptionId = csp.CreateCspSubscription(customerId);
                var newSubscription = new Models.Subscription();
                newSubscription.CustomerId = customerId;
                newSubscription.SubscriptionId = subscriptionId.ToLower();
                db.Subscriptions.Add(newSubscription);
                // Exit
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return Index();
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Customer customer = db.Customers.Find(id);
            var customer = new Customer();
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerId,CustomerName")] Customer customer)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Entry(customer).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            // Delete customer from the database
            ModelTools.DeleteCustomerId(id);
            // Delete customer from Partner Center
            var csp = new PartnerCenter();
            csp.DeleteCspCustomer(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    db.Dispose();
            //}
            //base.Dispose(disposing);
        }
    }
}
