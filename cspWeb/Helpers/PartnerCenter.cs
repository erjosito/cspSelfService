using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Store.PartnerCenter;
using Microsoft.Store.PartnerCenter.Extensions;
using Microsoft.Store.PartnerCenter.Models;
using Microsoft.Store.PartnerCenter.Models.Customers;
using Microsoft.Store.PartnerCenter.Models.Orders;
using Microsoft.Store.PartnerCenter.Profiles;
using cspWeb.Properties;
using cspWeb.Models;
using System.Threading;


namespace cspWeb.Helpers
{
    public class PartnerCenter
    {

        // WARNING, VARIABLE OVERLOAD!
        // PartnerCenter (this class) is different from Microsoft.Store.PartnerCenter
        // Models.Customer is different from Microsoft.Store.PartnerCenter.Customer

        public static string PartnerServiceApiRoot = "https://api.partnercenter.microsoft.com";
        public static string Authority = "https://login.windows.net";
        public static string ResourceUrl = "https://graph.windows.net";
        public static string ApplicationId = Settings.Default.AppId;
        public static string ApplicationSecret = Settings.Default.AppSecret;
        public static string ApplicationDomain = Settings.Default.CspTenantId;
    
        public string getCspDomain()
        {
            string[] strArr = ApplicationDomain.Split('@');
            return strArr[1];
        }

        public IAggregatePartner GetPartnerCenterTokenUsingAppCredentials()
        {
            // Get a user Azure AD Token
            var partnerCredentials = PartnerCredentials.Instance.GenerateByApplicationCredentials(PartnerCenter.ApplicationId, PartnerCenter.ApplicationSecret, PartnerCenter.ApplicationDomain);

            // get operation with partnerCredentials
            return PartnerService.Instance.CreatePartnerOperations(partnerCredentials);
        }

        /// <summary>
        /// Gets the customers using SDK.
        /// </summary>
        public List<Models.Customer> GetCspCustomers(string customerId = null)
        {
            IAggregatePartner partner = GetPartnerCenterTokenUsingAppCredentials();
            // get customers list
            var allCustomers = partner.Customers.Get();
            //extract relevant data and put in a list
            var customerList = new List<Models.Customer>();
            //browse answer
            foreach (var thisCustomer in allCustomers.Items)
            {
                var newCustomer = new Models.Customer();
                newCustomer.CustomerName = thisCustomer.CompanyProfile.CompanyName;
                newCustomer.CustomerId = thisCustomer.Id;
                customerList.Add(newCustomer);
            }
            return customerList;
        }

        public String GetMyCspDomain()
        {
            IAggregatePartner partner = GetPartnerCenterTokenUsingAppCredentials();
            var billingProfile = partner.Profiles.BillingProfile;
            return billingProfile.Partner.Domains.ToString();
        }

        public void DeleteCspCustomer(string CustomerId)
        {
            string token = REST.getCspToken();
            string url = "https://api.partnercenter.microsoft.com/v1/customers/" + CustomerId;
            string method = "DELETE";
            REST.sendHttpRequest(method, url, token);
        }

        public Models.Customer GetUserCspCustomers(string CustomerId)
        {
            IAggregatePartner partner = GetPartnerCenterTokenUsingAppCredentials();
            // get customers list
            var allCustomers = partner.Customers.Get();
            //browse answer
            foreach (var thisCustomer in allCustomers.Items)
            {
                if (thisCustomer.Id == CustomerId)
                {
                    var newCustomer = new Models.Customer();
                    newCustomer.CustomerName = thisCustomer.CompanyProfile.CompanyName;
                    newCustomer.CustomerId = thisCustomer.Id;
                    return newCustomer;
                }
            }
            return null;
        }

        public string CreateCspCustomer(string customerName, bool CreateSubscription = false)
        {
            // IAggregatePartner partnerOperations;
            IAggregatePartner partner = GetPartnerCenterTokenUsingAppCredentials();

            // Get an unique domain, based on the current day/time
            DateTime myDate = DateTime.Now;
            string TimePrefix = myDate.Year.ToString() + myDate.Month.ToString() + myDate.Day.ToString() + myDate.Hour.ToString() + myDate.Minute.ToString() + myDate.Second.ToString();
            string myUniqueDomain = TimePrefix + ".onmicrosoft.com";

            var customerToCreate = new Microsoft.Store.PartnerCenter.Models.Customers.Customer()
            {

                CompanyProfile = new CustomerCompanyProfile()
                {
                    Domain = myUniqueDomain
                },

                BillingProfile = new CustomerBillingProfile()
                {
                    Culture = "EN-US",
                    Email = "SomeEmail@Outlook.com",
                    Language = "En",
                    CompanyName = customerName,
                    DefaultAddress = new Address()
                    {
                        FirstName = "Fake",
                        LastName = "Name",
                        AddressLine1 = "One Microsoft Way",
                        City = "Redmond",
                        State = "WA",
                        Country = "US",
                        PostalCode = "98052",
                        PhoneNumber = ""
                    }
                }
            };
            var newCustomer = partner.Customers.Create(customerToCreate);
            if (CreateSubscription)
            {
                string SubscriptionId = CreateCspSubscription(newCustomer.Id);
                //Testing the ARM API does not work until some time (2 minutes) after having created the user
                //Thread.Sleep(120000);
                //ARM.createResourceGroup(newCustomer.Id, SubscriptionId, "testRg", "westeurope");
                //ARM.createSRVault(newCustomer.Id, SubscriptionId, "testRg", "testVault", "westeurope");
            }
            return newCustomer.Id;
        }

        public string CreateCspSubscription (string CustomerId)
        {
            string AzureOfferId = Settings.Default.AzureOfferId;
            IAggregatePartner partner = GetPartnerCenterTokenUsingAppCredentials();
            var order = new Order()
            {
                ReferenceCustomerId = CustomerId,
                LineItems = new List<OrderLineItem>()
                {
                    new OrderLineItem()
                    {
                        OfferId = AzureOfferId,
                        FriendlyName = "Azure Subscription",
                        Quantity = 5
                    }
                }
            };
            var createdOrder = partner.Customers.ById(CustomerId).Orders.Create(order);
            return createdOrder.LineItems.ElementAt(0).SubscriptionId;
        }
    }
}