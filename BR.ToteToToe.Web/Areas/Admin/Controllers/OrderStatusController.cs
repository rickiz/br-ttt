using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using BR.ToteToToe.Web.Helpers;
using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Areas.Admin.ViewModels;
using BR.ToteToToe.Web.Properties;

namespace BR.ToteToToe.Web.Areas.Admin.Controllers
{
    [Authorize]
    public partial class OrderStatusController : TTTBaseController
    {
        #region Private Methods

        private List<OrderStatusSearchResult> SearchOrder(OrderStatusSearchCriteria criteria)
        {
            var results = new List<OrderStatusSearchResult>();
            var excludeStatus = new string[] { Status.Open.ToString(), Status.Closed.ToString() };

            using (var context = new TTTEntities())
            {
                var query =
                    (from a in context.trnsalesorders
                     join b in context.logpayments on a.PaymentGatewayTransID equals b.TransId
                    where !excludeStatus.Contains(a.refstatu.Name)
                        && b.Status == Settings.Default.iPay88_Status_Success
                    select new { SalesOrder = a, LogPayment = b }).AsQueryable();

                if (criteria.SalesOrderID.HasValue && criteria.SalesOrderID.Value > 0)
                    query = query.Where(a => a.SalesOrder.ID == criteria.SalesOrderID.Value);

                if (criteria.StatusID > 0)
                    query = query.Where(a => a.SalesOrder.StatusID == criteria.StatusID);

                if (criteria.PaymentSuccessDateFrom.HasValue)
                {
                    var fromDate = criteria.PaymentSuccessDateFrom.Value.Date.AddMilliseconds(-1);

                    query = query.Where(a => a.LogPayment.CreateDT > fromDate);
                }

                if (criteria.PaymentSuccessDateTo.HasValue)
                {
                    var toDate = criteria.PaymentSuccessDateTo.Value.Date.AddDays(1);

                    query = query.Where(a => a.LogPayment.CreateDT < toDate);
                }

                query = query.OrderByDescending(a => a.SalesOrder.ID)
                             .Take(50);

                results =
                    query.Select(a => new OrderStatusSearchResult
                    {
                        Email = a.SalesOrder.Email,
                        PaymentSuccessDate = a.LogPayment.CreateDT,
                        SalesOrderID = a.SalesOrder.ID,
                        Status = a.SalesOrder.refstatu.Name
                    }).ToList();
            }

            return results;
        }

        #endregion

        public virtual ActionResult Index()
        {
            if (!Util.SessionAccess.IsAdmin)
                throw new AccessViolationException("You have no rights to access this page.");

            var viewModel = new OrderStatusViewModel();

            viewModel.Results = SearchOrder(viewModel.Criteria);

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Index(OrderStatusViewModel viewModel)
        {
            if (!Util.SessionAccess.IsAdmin)
                throw new AccessViolationException("You have no rights to access this page.");

            viewModel.Results = SearchOrder(viewModel.Criteria);

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Update(OrderStatusViewModel viewModel)
        {
            var updateModel = viewModel.UpdateModel;

            using (var context = new TTTEntities())
            {
                var salesOrder = context.trnsalesorders
                    .Include(a => a.refstatu)
                    .Single(a => a.ID == updateModel.SalesOrderID);
                var status = salesOrder.refstatu.Name;
                var processingStatus = Status.Processing.ToString();
                var deliveryStatus = Status.Delivery.ToString();
                var closedStatus = Status.Closed.ToString();

                if (status == Status.Pending.ToString())
                {
                    salesOrder.StatusID = context.refstatus.Single(a => a.Name == processingStatus && a.Active).ID;
                }
                else if (status == processingStatus)
                {
                    salesOrder.StatusID = context.refstatus.Single(a => a.Name == deliveryStatus && a.Active).ID;
                    salesOrder.DeliveryDT = updateModel.DeliveryDT;
                }
                else if (status == deliveryStatus)
                {
                    salesOrder.StatusID = context.refstatus.Single(a => a.Name == closedStatus && a.Active).ID;
                    salesOrder.DeliveryDT = updateModel.DeliveryDT;
                }

                context.SaveChanges();
            }

            return RedirectToAction(MVC.Admin.OrderStatus.Index());
        }
    }
}
