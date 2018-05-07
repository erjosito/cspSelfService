using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace cspWeb.Models
{
    public class Service
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Service Id")]
        public int Id { get; set; }

        [DisplayName("Subscription Id")]
        public string SubscriptionId { get; set; }

        [DisplayName("Offering Id")]
        public string OfferingId { get; set; }

        [DisplayName("Service Code")]
        public string Description { get; set; }

        [DisplayName("ResourceId")]
        public string ResourceId { get; set; }
    }
}