using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace cspWeb.Models
{
    public class Customer
    {
        [DisplayName ("Customer Id")]
        public string CustomerId { get; set; }

        // user ID from AspNetUser table
        public string OwnerId { get; set; }

        [DisplayName("Company Name")]
        public string CustomerName { get; set; }

        public List<Subscription> Subscriptions { get; set; }
    }
}