using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace cspWeb.Models
{
    public class CustomerViewModel
    {
        [DisplayName("Company Name")]
        public string CustomerName { get; set; }
    }
}