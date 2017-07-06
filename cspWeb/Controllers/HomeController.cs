using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cspWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "CSP demo app";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "CSP demo app";

            return View();
        }
    }
}