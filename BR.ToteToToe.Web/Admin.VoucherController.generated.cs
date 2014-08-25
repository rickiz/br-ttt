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
namespace BR.ToteToToe.Web.Areas.Admin.Controllers
{
    public partial class VoucherController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public VoucherController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected VoucherController(Dummy d) { }

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


        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public VoucherController Actions { get { return MVC.Admin.Voucher; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "Admin";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Voucher";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Voucher";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string Maintain = "Maintain";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string Maintain = "Maintain";
        }


        static readonly ActionParamsClass_Index s_params_Index = new ActionParamsClass_Index();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Index IndexParams { get { return s_params_Index; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Index
        {
            public readonly string viewModel = "viewModel";
            public readonly string collection = "collection";
        }
        static readonly ActionParamsClass_Maintain s_params_Maintain = new ActionParamsClass_Maintain();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Maintain MaintainParams { get { return s_params_Maintain; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Maintain
        {
            public readonly string viewModel = "viewModel";
            public readonly string collection = "collection";
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
                public readonly string Maintain = "Maintain";
            }
            public readonly string Index = "~/Areas/Admin/Views/Voucher/Index.cshtml";
            public readonly string Maintain = "~/Areas/Admin/Views/Voucher/Maintain.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_VoucherController : BR.ToteToToe.Web.Areas.Admin.Controllers.VoucherController
    {
        public T4MVC_VoucherController() : base(Dummy.Instance) { }

        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            IndexOverride(callInfo);
            return callInfo;
        }

        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, BR.ToteToToe.Web.Areas.Admin.ViewModels.VoucherSearchViewModel viewModel, System.Web.Mvc.FormCollection collection);

        public override System.Web.Mvc.ActionResult Index(BR.ToteToToe.Web.Areas.Admin.ViewModels.VoucherSearchViewModel viewModel, System.Web.Mvc.FormCollection collection)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "viewModel", viewModel);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "collection", collection);
            IndexOverride(callInfo, viewModel, collection);
            return callInfo;
        }

        partial void MaintainOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        public override System.Web.Mvc.ActionResult Maintain()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Maintain);
            MaintainOverride(callInfo);
            return callInfo;
        }

        partial void MaintainOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, BR.ToteToToe.Web.Areas.Admin.ViewModels.VoucherMaintainViewModel viewModel, System.Web.Mvc.FormCollection collection);

        public override System.Web.Mvc.ActionResult Maintain(BR.ToteToToe.Web.Areas.Admin.ViewModels.VoucherMaintainViewModel viewModel, System.Web.Mvc.FormCollection collection)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Maintain);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "viewModel", viewModel);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "collection", collection);
            MaintainOverride(callInfo, viewModel, collection);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591