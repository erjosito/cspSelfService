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

        public static bool UserIdHasRSVault(string userId)
        {
            var servicesList = GetServicesFromUserID(userId);
            bool aux = false;
            foreach (var service in servicesList)
            {
                if (service.Description == "RSVault")
                {
                    aux = true;
                }
            }
            return aux;
        }


    }
}