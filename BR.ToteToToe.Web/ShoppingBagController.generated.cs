// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace BR.ToteToToe.Web.Controllers
{
    public partial class ShoppingBagController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ShoppingBagController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected ShoppingBagController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult Remove()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Remove);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.JsonResult VoucherDiscount()
        {
            return new T4MVC_System_Web_Mvc_JsonResult(Area, Name, ActionNames.VoucherDiscount);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ShoppingBagController Actions { get { return MVC.ShoppingBag; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "ShoppingBag";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "ShoppingBag";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string Remove = "Remove";
            public readonly string LinkToAccount = "LinkToAccount";
            public readonly string VoucherDiscount = "VoucherDiscount";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string Remove = "Remove";
            public const string LinkToAccount = "LinkToAccount";
            public const string VoucherDiscount = "VoucherDiscount";
        }


        static readonly ActionParamsClass_Index s_params_Index = new ActionParamsClass_Index();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Index IndexParams { get { return s_params_Index; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Index
        {
            public readonly string rtn = "rtn";
            public readonly string collection = "collection";
        }
        static readonly ActionParamsClass_Remove s_params_Remove = new ActionParamsClass_Remove();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Remove RemoveParams { get { return s_params_Remove; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Remove
        {
            public readonly string soID = "soID";
        }
        static readonly ActionParamsClass_VoucherDiscount s_params_VoucherDiscount = new ActionParamsClass_VoucherDiscount();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_VoucherDiscount VoucherDiscountParams { get { return s_params_VoucherDiscount; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_VoucherDiscount
        {
            public readonly string voucherCode = "voucherCode";
            public readonly string total = "total";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string Index = "Index";
            }
            public readonly string Index = "~/Views/ShoppingBag/Index.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_ShoppingBagController : BR.ToteToToe.Web.Controllers.ShoppingBagController
    {
        public T4MVC_ShoppingBagController() : base(Dummy.Instance) { }

        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, bool? rtn);

        public override System.Web.Mvc.ActionResult Index(bool? rtn)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "rtn", rtn);
            IndexOverride(callInfo, rtn);
            return callInfo;
        }

        partial void RemoveOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int soID);

        public override System.Web.Mvc.ActionResult Remove(int soID)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Remove);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "soID", soID);
            RemoveOverride(callInfo, soID);
            return callInfo;
        }

        partial void LinkToAccountOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        public override System.Web.Mvc.ActionResult LinkToAccount()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.LinkToAccount);
            LinkToAccountOverride(callInfo);
            return callInfo;
        }

        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, System.Web.Mvc.FormCollection collection);

        public override System.Web.Mvc.ActionResult Index(System.Web.Mvc.FormCollection collection)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "collection", collection);
            IndexOverride(callInfo, collection);
            return callInfo;
        }

        partial void VoucherDiscountOverride(T4MVC_System_Web_Mvc_JsonResult callInfo, string voucherCode, string total);

        public override System.Web.Mvc.JsonResult VoucherDiscount(string voucherCode, string total)
        {
            var callInfo = new T4MVC_System_Web_Mvc_JsonResult(Area, Name, ActionNames.VoucherDiscount);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "voucherCode", voucherCode);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "total", total);
            VoucherDiscountOverride(callInfo, voucherCode, total);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
