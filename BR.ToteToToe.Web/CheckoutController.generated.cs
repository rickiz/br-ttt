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
    public partial class CheckoutController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public CheckoutController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected CheckoutController(Dummy d) { }

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
        public virtual System.Web.Mvc.JsonResult ValidateVoucherCode()
        {
            return new T4MVC_System_Web_Mvc_JsonResult(Area, Name, ActionNames.ValidateVoucherCode);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult Success()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Success);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public CheckoutController Actions { get { return MVC.Checkout; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Checkout";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Checkout";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string Billing = "Billing";
            public readonly string Shipping = "Shipping";
            public readonly string Summary = "Summary";
            public readonly string ValidateVoucherCode = "ValidateVoucherCode";
            public readonly string Success = "Success";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string Billing = "Billing";
            public const string Shipping = "Shipping";
            public const string Summary = "Summary";
            public const string ValidateVoucherCode = "ValidateVoucherCode";
            public const string Success = "Success";
        }


        static readonly ActionParamsClass_Billing s_params_Billing = new ActionParamsClass_Billing();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Billing BillingParams { get { return s_params_Billing; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Billing
        {
            public readonly string chooseAddress = "chooseAddress";
            public readonly string viewModel = "viewModel";
        }
        static readonly ActionParamsClass_Shipping s_params_Shipping = new ActionParamsClass_Shipping();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Shipping ShippingParams { get { return s_params_Shipping; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Shipping
        {
            public readonly string chooseAddress = "chooseAddress";
            public readonly string viewModel = "viewModel";
        }
        static readonly ActionParamsClass_ValidateVoucherCode s_params_ValidateVoucherCode = new ActionParamsClass_ValidateVoucherCode();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ValidateVoucherCode ValidateVoucherCodeParams { get { return s_params_ValidateVoucherCode; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ValidateVoucherCode
        {
            public readonly string voucherCode = "voucherCode";
            public readonly string soID = "soID";
        }
        static readonly ActionParamsClass_Success s_params_Success = new ActionParamsClass_Success();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Success SuccessParams { get { return s_params_Success; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Success
        {
            public readonly string id = "id";
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
                public readonly string Billing = "Billing";
                public readonly string Shipping = "Shipping";
                public readonly string Success = "Success";
                public readonly string Summary = "Summary";
            }
            public readonly string Billing = "~/Views/Checkout/Billing.cshtml";
            public readonly string Shipping = "~/Views/Checkout/Shipping.cshtml";
            public readonly string Success = "~/Views/Checkout/Success.cshtml";
            public readonly string Summary = "~/Views/Checkout/Summary.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_CheckoutController : BR.ToteToToe.Web.Controllers.CheckoutController
    {
        public T4MVC_CheckoutController() : base(Dummy.Instance) { }

        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            IndexOverride(callInfo);
            return callInfo;
        }

        partial void BillingOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int chooseAddress);

        public override System.Web.Mvc.ActionResult Billing(int chooseAddress)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Billing);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "chooseAddress", chooseAddress);
            BillingOverride(callInfo, chooseAddress);
            return callInfo;
        }

        partial void BillingOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, BR.ToteToToe.Web.ViewModels.CheckoutBillingViewModel viewModel);

        public override System.Web.Mvc.ActionResult Billing(BR.ToteToToe.Web.ViewModels.CheckoutBillingViewModel viewModel)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Billing);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "viewModel", viewModel);
            BillingOverride(callInfo, viewModel);
            return callInfo;
        }

        partial void ShippingOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int chooseAddress);

        public override System.Web.Mvc.ActionResult Shipping(int chooseAddress)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Shipping);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "chooseAddress", chooseAddress);
            ShippingOverride(callInfo, chooseAddress);
            return callInfo;
        }

        partial void ShippingOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, BR.ToteToToe.Web.ViewModels.CheckoutShippingViewModel viewModel);

        public override System.Web.Mvc.ActionResult Shipping(BR.ToteToToe.Web.ViewModels.CheckoutShippingViewModel viewModel)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Shipping);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "viewModel", viewModel);
            ShippingOverride(callInfo, viewModel);
            return callInfo;
        }

        partial void SummaryOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        public override System.Web.Mvc.ActionResult Summary()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Summary);
            SummaryOverride(callInfo);
            return callInfo;
        }

        partial void ValidateVoucherCodeOverride(T4MVC_System_Web_Mvc_JsonResult callInfo, string voucherCode, int soID);

        public override System.Web.Mvc.JsonResult ValidateVoucherCode(string voucherCode, int soID)
        {
            var callInfo = new T4MVC_System_Web_Mvc_JsonResult(Area, Name, ActionNames.ValidateVoucherCode);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "voucherCode", voucherCode);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "soID", soID);
            ValidateVoucherCodeOverride(callInfo, voucherCode, soID);
            return callInfo;
        }

        partial void SuccessOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int id);

        public override System.Web.Mvc.ActionResult Success(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Success);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            SuccessOverride(callInfo, id);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
