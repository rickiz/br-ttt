using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using BR.ToteToToe.Web.ViewModels;
using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Helpers;
using BR.ToteToToe.Web.Extensions;

using log4net;
using BR.ToteToToe.Web.Properties;

namespace BR.ToteToToe.Web.Controllers
{
    public partial class PaymentController : TTTBaseController
    {
        private PaymentStatusResponseViewModel ConstructPaymentResponseModel()
        {
            var paymentInfo = new CheckoutPaymentInfo
            {
                MerchantCode = Request["MerchantCode"] ?? "",
                PaymentId = Request["PaymentId"] ?? "",
                RefNo = Request["RefNo"] ?? "",
                Amount = Request["Amount"] ?? "",
                Currency = Request["Currency"] ?? "",
                Remark = Request["Remark"] ?? "",
                Signature = Request["Signature"] ?? ""
            };

            var viewModel = new PaymentStatusResponseViewModel
            {
                PaymentInfo = paymentInfo,
                TransID = Request["TransId"] ?? "",
                AuthCode = Request["AuthCode"] ?? "",
                Status = Request["Status"] ?? "",
                ErrDesc = Request["ErrDesc"] ?? ""
            };

            return viewModel;
        }
        private logpayment ConstructLogPayment(PaymentStatusResponseViewModel paymentStatus)
        {
            var paymentInfo = paymentStatus.PaymentInfo;
            var requestReferrerUrl = Request.UrlReferrer.AbsolutePath;

            var log = new logpayment
            {
                Amount = paymentInfo.Amount.ToDecimal(),
                AuthCode = paymentStatus.AuthCode,
                CreateDT = DateTime.Now,
                Currency = paymentInfo.Currency,
                ErrDesc = paymentStatus.ErrDesc,
                MerchantCode = paymentInfo.MerchantCode,
                PaymentId = paymentInfo.PaymentId.ToInt(),
                RefNo = paymentInfo.RefNo,
                Remark = paymentInfo.Remark,
                RequestReferrerUrl = requestReferrerUrl,
                Signature = paymentInfo.Signature,
                Status = paymentStatus.Status,
                TransId = paymentStatus.TransID
            };

            return log;
        }

        [HttpPost]
        public virtual ActionResult StatusResponse()
        {
            try
            {
                var viewModel = ConstructPaymentResponseModel();
                var log = ConstructLogPayment(viewModel);
                var salesOrderID = int.Parse(log.RefNo);
                var pendingStatusID = Util.GetStatusID(Status.Pending.ToString());                

                using (var context = new TTTEntities())
                {
                    context.logpayments.Add(log);
                    context.SaveChanges();

                    var salesOrder = context.trnsalesorders
                        .Include(a => a.refstatu)
                        .SingleOrDefault(a => a.ID == salesOrderID);

                    if (salesOrder == null)
                        throw new ApplicationException("Order not found.");
                    if (salesOrder.refstatu.Name != Status.Open.ToString())
                        throw new ApplicationException("Invalid order status.");

                    if (log.Status == Settings.Default.iPay88_Status_Success)
                    {
                        salesOrder.StatusID = pendingStatusID;
                        salesOrder.PaymentGatewayTransID = log.TransId;
                        salesOrder.UpdateDT = DateTime.Now;
                        context.SaveChanges();

                        var soItems =
                            context.lnksalesorders
                                .Where(a => a.Active && a.SalesOrderID == salesOrderID && a.ModelSizeID.HasValue)
                                .ToList();

                        foreach (var item in soItems)
                        {
                            var modelSize = context.lnkmodelsizes.Single(a => a.ID == item.ModelSizeID.Value);
                            modelSize.Quantity -= item.Quantity;
                        }

                        context.SaveChanges();

                        return RedirectToAction(MVC.Checkout.Success(salesOrderID));
                    }
                    else
                    {
                        var errMsg = string.Format("Payment failed for Sales Order ID:{0}. Error Description: {1}", 
                            salesOrderID, log.ErrDesc);

                        throw new ApplicationException(errMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return RedirectException(ex);
            }            
        }

        [HttpPost]
        public virtual ActionResult BackendResponse()
        {
            try
            {
                var viewModel = ConstructPaymentResponseModel();
                var log = ConstructLogPayment(viewModel);
                var salesOrderID = int.Parse(log.RefNo);
                var pendingStatusID = Util.GetStatusID(Status.Pending.ToString());

                using (var context = new TTTEntities())
                {
                    context.logpayments.Add(log);
                    context.SaveChanges();

                    var salesOrder = context.trnsalesorders
                        .Include(a => a.refstatu)
                        .SingleOrDefault(a => a.ID == salesOrderID);

                    if (salesOrder == null)
                        throw new ApplicationException("Order not found.");

                    if (salesOrder.refstatu.Name == Status.Open.ToString())
                    {
                        if (log.Status == Settings.Default.iPay88_Status_Success)
                        {
                            salesOrder.StatusID = pendingStatusID;
                            salesOrder.PaymentGatewayTransID = log.TransId;
                            salesOrder.UpdateDT = DateTime.Now;
                            context.SaveChanges();

                            var soItems =
                            context.lnksalesorders
                                .Where(a => a.Active && a.SalesOrderID == salesOrderID && a.ModelSizeID.HasValue)
                                .ToList();

                            foreach (var item in soItems)
                            {
                                var modelSize = context.lnkmodelsizes.Single(a => a.ID == item.ModelSizeID.Value);
                                modelSize.Quantity -= item.Quantity;
                            }

                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        if(string.IsNullOrEmpty(salesOrder.PaymentGatewayTransID))
                            throw new ApplicationException("Invalid order status.");
                    }                    
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }

            return Content("RECEIVEOK");
        }
    }
}
