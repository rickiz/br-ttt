using BR.ToteToToe.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BR.ToteToToe.Web.Controllers
{
    public partial class OrderController : TTTBaseController
    {
        //
        // GET: /Order/

        public virtual ActionResult Index()
        {
            return View();
        }

    }
}
