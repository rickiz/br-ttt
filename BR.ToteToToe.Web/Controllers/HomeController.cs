using BR.ToteToToe.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BR.ToteToToe.Web.Controllers
{
    public partial class HomeController : TTTBaseController
    {
        public virtual ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public virtual ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public virtual ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public virtual ActionResult ReturnRefund()
        {
            ViewBag.Message = "Returns";

            return View();
        }

        public virtual ActionResult TnC()
        {
            ViewBag.Message = "TnC";

            return View();
        }

        public virtual ActionResult Privacy()
        {
            ViewBag.Message = "Privacy";

            return View();
        }

        public virtual ActionResult Shipping()
        {
            ViewBag.Message = "Shipping";

            return View();
        }

        public virtual ActionResult Payment()
        {
            ViewBag.Message = "Payment";

            return View();
        }

        public virtual ActionResult SizeGuidelines()
        {
            ViewBag.Message = "Size Guidelines";

            return View();
        }
    }
}
