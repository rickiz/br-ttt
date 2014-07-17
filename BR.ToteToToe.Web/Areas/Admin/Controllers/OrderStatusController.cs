using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using BR.ToteToToe.Web.Helpers;
using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Areas.Admin.ViewModels;

namespace BR.ToteToToe.Web.Areas.Admin.Controllers
{
    [Authorize]
    public partial class OrderStatusController : TTTBaseController
    {
        public virtual ActionResult Index()
        {
            if (!Util.SessionAccess.IsAdmin)
                throw new AccessViolationException("You have no rights to access this page.");

            using (var context = new TTTEntities())
            {
                var excludeStatus = new string[] { Status.Open.ToString(), Status.Closed.ToString() };

                var results =
                    context.trnsalesorders
                        .Include(a => a.refstatu)
                        .Where(a => !excludeStatus.Contains(a.refstatu.Name))
                        .OrderByDescending(a => a.ID)
                        .Take(50)
                        .Select(a => new OrderStatusSearchResult
                        {
                            Email = a.Email,
                            SalesOrderID = a.ID,
                            Status = a.refstatu.Name
                        }).ToList();

                var viewModel = new OrderStatusViewModel
                {
                    Results = results
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        public virtual ActionResult Index(OrderStatusViewModel viewModel)
        {
            if (!Util.SessionAccess.IsAdmin)
                throw new AccessViolationException("You have no rights to access this page.");

            var criteria = viewModel.Criteria;

            using (var context = new TTTEntities())
            {
                var excludeStatus = new string[] { Status.Open.ToString(), Status.Closed.ToString() };

                var query =
                    context.trnsalesorders
                        .Include(a => a.refstatu)
                        .Where(a => !excludeStatus.Contains(a.refstatu.Name));

                if (criteria.SalesOrderID.HasValue && criteria.SalesOrderID.Value > 0)
                    query = query.Where(a => a.ID == criteria.SalesOrderID.Value);

                if (criteria.StatusID > 0)
                    query = query.Where(a => a.StatusID == criteria.StatusID);

                viewModel.Results = query
                    .OrderByDescending(a => a.ID)
                    .Take(50)
                    .Select(a => new OrderStatusSearchResult
                    {
                        Email = a.Email,
                        SalesOrderID = a.ID,
                        Status = a.refstatu.Name
                    }).ToList();

                return View(viewModel);
            }
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
