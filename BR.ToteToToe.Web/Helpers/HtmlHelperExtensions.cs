using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BR.ToteToToe.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Nl2Br(this HtmlHelper htmlHelper, string text)
        {
            if (string.IsNullOrEmpty(text))
                return MvcHtmlString.Create(text);

            var finalText = Regex.Replace(HttpUtility.HtmlEncode(text), @"\r\n?|\n", "<br />"); 

            return MvcHtmlString.Create(finalText);
        }

        public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                            string controllerName, string area = "")
        {
            return MenuLink(htmlHelper, linkText, actionName, controllerName, new { area = area });
        }

        public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                            string controllerName, object routeValues)
        {
            var currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
            var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");

            var builder = new TagBuilder("li")
            {
                InnerHtml = htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, null).ToHtmlString()
            };

            if (controllerName == currentController && actionName == currentAction)
                builder.AddCssClass("active");

            return new MvcHtmlString(builder.ToString());
        }

        public static string CheckBoxClientTemplate(this HtmlHelper htmlHelper, string fieldName)
        {
            return string.Format("<input type='checkbox' name='{0}' <#= {0}?\"checked\":\"\" #> disabled='disabled' />", fieldName);
        }
    }
}