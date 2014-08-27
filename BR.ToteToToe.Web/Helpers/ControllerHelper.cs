using System.IO;
using System.Web.Mvc;

namespace BR.ToteToToe.Web.Helpers
{
    public static class ControllerHelper
    {
        #region Render Partial to String
        public static string RenderPartialViewToString(this Controller controller)
        {
            return RenderPartialViewToString(null, null);
        }

        public static string RenderPartialViewToString(this Controller controller, string viewName)
        {
            return RenderPartialViewToString(controller, viewName, null);
        }

        public static string RenderPartialViewToString(this Controller controller, object model)
        {
            return RenderPartialViewToString(controller, null, model);
        }

        public static string RenderPartialViewToString(this Controller controller, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        #endregion

        #region Render View to String
        public static string RenderViewToString(this Controller controller)
        {
            return RenderViewToString(null, null);
        }

        public static string RenderViewToString(this Controller controller, string viewName)
        {
            return RenderViewToString(controller, viewName, null);
        }

        public static string RenderViewToString(this Controller controller, object model)
        {
            return RenderViewToString(controller, null, model);
        }

        public static string RenderViewToString(this Controller controller, string viewName, object model)
        {
            return RenderViewToString(controller, viewName, model, string.Empty);
        }

        public static string RenderViewToString(this Controller controller, string viewName, object model, string masterName)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, masterName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        #endregion
    }
}