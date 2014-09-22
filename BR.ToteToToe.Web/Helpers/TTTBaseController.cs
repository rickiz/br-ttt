using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BR.ToteToToe.Web.Helpers
{
    public class TTTBaseController : Controller
    {
        protected static readonly ILog Logger = LogManager.GetLogger("TTTBaseController");

        protected RedirectToRouteResult RedirectException(Exception ex)
        {
            Util.SetSessionErrMsg(ex);
            return RedirectToAction("Index", "Error", new { area = "" });
        }

        protected void LogException(Exception ex)
        {
            Logger.Error("", ex);
        }

        protected override void Initialize(RequestContext requestContext)
        {
            Util.CheckSessionAccess(requestContext);
            base.Initialize(requestContext);
        }

        protected void ValidateIsAdmin()
        {
            if (!Util.SessionAccess.IsAdmin)
                throw new AccessViolationException();
        }
    }
}
