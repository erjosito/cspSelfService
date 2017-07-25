using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace cspWeb.Models
{
    public class ServiceViewModel
    {
        [DisplayName("Subscription Id")]
        public string SubscriptionId { get; set; }

        [DisplayName("Service Description")]
        public string Description { get; set; }

        [DisplayName("Service Id")]
        public string Id { get; set; }

        [DisplayName("CSP Tenant Id")]
        public string cspTenantId { get; set; }
}
}