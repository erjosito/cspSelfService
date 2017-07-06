using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace cspWeb.Models
{
    public class Subscription
    {
        [DisplayName("Customer Id")]
        public string CustomerId { get; set; }

        [DisplayName("Subscription Id")]
        public string SubscriptionId { get; set; }
    }

}