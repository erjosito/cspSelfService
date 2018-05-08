using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using cspWeb.Models;
using cspWeb.Properties;


namespace cspWeb.Helpers
{
    public static class ModelTools
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static string cspTenantId = System.Configuration.ConfigurationManager.AppSettings["CspTenantId"];
        public static string cspUsername = System.Configuration.ConfigurationManager.AppSettings["CspUsername"];


        public static string getCspDomain()
        {
            string[] strArr = cspUsername.Split('@');
            return strArr[1];
        }

        public static string getCspTenantId()
        {
            return cspTenantId;
        }


        public static List<Models.Customer> GetCustomersFromUserID (string userId)
        {
            var customerList = new List<Models.Customer>();
            customerList = db.Customers.Where(c => c.OwnerId == userId).ToList();
            return customerList;
        }


        public static List<Models.Subscription> GetSubscriptionsFromUserID(string userId)
        {
            var customerList = GetCustomersFromUserID(userId);
            var subscriptionList = new List<Models.Subscription>();
            if (customerList.Count > 0)
            {
                foreach (var customer in customerList)
                {
                    var customerId = customer.CustomerId;
                    var thisSubscriptionList = db.Subscriptions.Where(s => s.CustomerId == customerId).ToList();
                    subscriptionList = subscriptionList.Concat(thisSubscriptionList).ToList();
                }
            }
            return subscriptionList;
        }

        public static List<Models.Subscription> GetSubscriptionsFromCustomerID(string customerId)
        {
            return db.Subscriptions.Where(s => s.CustomerId == customerId).ToList();
        }

        public static string GetCustomerIdFromSubscriptionID(string subscriptionId)
        {
            List<Models.Subscription> subscriptionList = db.Subscriptions.Where(s => s.SubscriptionId == subscriptionId).ToList();
            if (subscriptionList.Count > 0)
            {
                return subscriptionList[0].CustomerId;
            } else
            {
                return null;
            }
        }


        public static List<Models.Service> GetServicesFromSubscriptionID(string subscriptionId)
        {
            return db.Services.Where(s => s.SubscriptionId == subscriptionId).ToList();
        }


        public static List<Models.Service> GetServicesFromUserID(string userId)
        {
            var subscriptionList = GetSubscriptionsFromUserID(userId);
            var serviceList = new List<Models.Service>();
            if (subscriptionList.Count > 0)
            {
                foreach (var subscription in subscriptionList)
                {
                    var subscriptionId = subscription.SubscriptionId;
                    var thisServiceList = db.Services.Where(s => s.SubscriptionId == subscriptionId).ToList();
                    serviceList = serviceList.Concat(thisServiceList).ToList();
                }
            }
            return serviceList;
        }

        public static List<Models.Service> AddEmptyOfferings(List<Models.Service> serviceList) {
            var newServiceList = serviceList; 
            var offeringsList = db.Offerings.ToList();
            foreach (var offering in offeringsList)
            {
                var newService = new Models.Service();
                newService.ResourceId = null;
                newService.SubscriptionId = null;
                newService.Description = offering.Description;
                newService.OfferingId = offering.Id;
                newServiceList.Add(newService);
            }
            return newServiceList;
        }

        public static bool UserIdHasRSVault(string userId)
        {
            var servicesList = GetServicesFromUserID(userId);
            bool aux = false;
            foreach (var service in servicesList)
            {
                if (service.OfferingId == "RSVault")
                {
                    aux = true;
                }
            }
            return aux;
        }

        public static bool DeleteServiceId (int serviceId)
        {
            try
            {
                Service service = db.Services.Find(serviceId);
                db.Services.Remove(service);
                db.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteSubscriptionId(string subId)
        {
            try
            {
                Subscription sub = db.Subscriptions.Find(subId);
                db.Subscriptions.Remove(sub);
                db.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static string GetOfferingRGName(string OfferingId)
        {
            //Initialization
            string rgName = "";
            // Loop through all subs belonging to the customer
            var offeringsList = db.Offerings.ToList();
            foreach (var offering in offeringsList)
            {
                if (offering.Id == OfferingId) {
                    rgName = offering.RgName;
                }
            }
            return rgName;
        }

        public static int NextServiceId()
        {
            //Initialization
            int counter = 1;
            // Loop through all existing services
            var servicesList = db.Services.ToList();
            foreach (var service in servicesList)
            {
                if (service.Id >= counter)
                {
                    counter = service.Id + 1;
                }
            }
            return counter;
        }

        public static bool DeleteCustomerId (string customerId)
        {
            // Loop through all subs belonging to the customer
            var subscriptionsList = GetSubscriptionsFromCustomerID(customerId);
            foreach (var sub in subscriptionsList)
            {
                // Loop through all services for those subs
                var servicesList = GetServicesFromSubscriptionID(sub.SubscriptionId);
                foreach (var service in servicesList)
                {
                    DeleteServiceId(service.Id);
                }
                DeleteSubscriptionId(sub.SubscriptionId);
            }

            // And finally, delete the customer
            try
            {
                Customer customer = db.Customers.Find(customerId);
                db.Customers.Remove(customer);
                db.SaveChanges();
            } catch
            {
                return false;
            }
            return true;
        }

    }
}