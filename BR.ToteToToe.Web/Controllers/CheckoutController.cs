using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Helpers;
using BR.ToteToToe.Web.ViewModels;
using BR.ToteToToe.Web.Properties;

namespace BR.ToteToToe.Web.Controllers
{
    [Authorize]
    public partial class CheckoutController : TTTBaseController
    {
        private CheckoutAddress ConstructMyAccountAddress(tbladdress address)
        {
            var result = new CheckoutAddress
            {
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                CityTown = address.CityTown,
                CountryID = address.CountryID.HasValue ? address.CountryID.Value : 0,
                Postcode = address.Postcode,
                State = address.State
            };

            return result;
        }

        private CheckoutAddress ConstructMyAccountAddress(lnksoaddress address)
        {
            var result = new CheckoutAddress
            {
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                CityTown = address.CityTown,
                CountryID = address.CountryID.HasValue ? address.CountryID.Value : 0,
                Postcode = address.Postcode,
                State = address.State
            };

            return result;
        }

        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult Billing(int chooseAddress = 0)
        {
            var access = Util.SessionAccess;
            var viewModel = new CheckoutBillingViewModel();
            var addressType = (CheckoutAddressType)chooseAddress;

            using (var context = new TTTEntities())
            {
                var salesOrder = context.trnsalesorders
                    .Include(a => a.refstatu)
                    .SingleOrDefault(a => a.Email == access.Email && a.refstatu.Name == "Open");

                if (salesOrder == null)
                    throw new ApplicationException("No Order found.");

                var soBillingAddress = context.lnksoaddresses
                    .FirstOrDefault(a => a.SalesOrderID == salesOrder.ID && a.Active && a.IsBilling);
                var billingAddress = context.tbladdresses
                    .FirstOrDefault(a => a.AccessID == access.ID && a.Active && a.IsBilling);

                switch (addressType)
                {
                    case CheckoutAddressType.None:
                        if (soBillingAddress == null)
                        {
                            if (billingAddress == null)
                            {
                                var primayAddress = context.tbladdresses
                                    .FirstOrDefault(a => a.AccessID == access.ID && a.Active && a.IsPrimary);

                                if (primayAddress == null)
                                {
                                    var secondAddress = context.tbladdresses
                                        .FirstOrDefault(a => a.AccessID == access.ID && a.Active && !a.IsPrimary);

                                    if (secondAddress == null)
                                    {
                                        addressType = CheckoutAddressType.Primary;
                                    }
                                    else
                                    {
                                        addressType = CheckoutAddressType.Secondary;
                                        viewModel.Address = ConstructMyAccountAddress(secondAddress);
                                        viewModel.IsAddressReadOnly = true;
                                    }                                    
                                }
                                else
                                {
                                    addressType = CheckoutAddressType.Primary;
                                    viewModel.Address = ConstructMyAccountAddress(primayAddress);
                                    viewModel.IsAddressReadOnly = true;
                                }
                            }
                            else
                            {
                                addressType = billingAddress.IsPrimary ? CheckoutAddressType.Primary : CheckoutAddressType.Secondary;
                                viewModel.Address = ConstructMyAccountAddress(billingAddress);
                                viewModel.IsAddressReadOnly = true;
                            }
                        }
                        else
                        {
                            viewModel.Address = ConstructMyAccountAddress(soBillingAddress);                            
                            viewModel.IsAddressReadOnly = soBillingAddress.ChooseAddress != (int)CheckoutAddressType.Others;
                            viewModel.SalesOrderBillingAddressID = soBillingAddress.ID;
                            addressType = (CheckoutAddressType)soBillingAddress.ChooseAddress;
                        }

                        break;

                    case CheckoutAddressType.Primary:
                    case CheckoutAddressType.Secondary:
                        var isPrimary = addressType == CheckoutAddressType.Primary;
                        var accountAddress = context.tbladdresses
                            .FirstOrDefault(a => a.AccessID == access.ID && a.Active && a.IsPrimary == isPrimary);

                        if (accountAddress != null)
                        {
                            viewModel.Address = ConstructMyAccountAddress(accountAddress);
                            viewModel.IsAddressReadOnly = true;
                        }
                    
                        break;

                    case CheckoutAddressType.Others:
                        if (soBillingAddress != null && soBillingAddress.ChooseAddress == (int)CheckoutAddressType.Others)
                        {
                            viewModel.Address = ConstructMyAccountAddress(soBillingAddress);
                            viewModel.SalesOrderBillingAddressID = soBillingAddress.ID;
                        }

                        break;

                    default:
                        break;
                }

                //if (addressType == CheckoutAddressType.None)
                //    addressType = CheckoutAddressType.Others;
                viewModel.AddressTypeID = (int)addressType;


                if (soBillingAddress != null && soBillingAddress.ChooseAddress == (int)addressType)
                {
                    viewModel.FirstName = soBillingAddress.FirstName;
                    viewModel.LastName = soBillingAddress.LastName;
                    viewModel.Phone = string.IsNullOrEmpty(soBillingAddress.Phone) ? access.Phone : soBillingAddress.Phone;
                }
                else
                {
                    viewModel.FirstName = access.FirstName;
                    viewModel.LastName = access.LastName;
                    viewModel.Phone = access.Phone;
                }
            }

            viewModel.AddressType = addressType;

            if (viewModel.Address.CountryID == 0)
                viewModel.Address.CountryID = Util.GetDefaultCountryID(); 

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Billing(CheckoutBillingViewModel viewModel)
        {
            var access = Util.SessionAccess;
            var chooseAddress = 0;

            using (var context = new TTTEntities())
            {
                var salesOrderID = context.trnsalesorders
                    .Include(a => a.refstatu)
                    .Single(a => a.Email == access.Email && a.refstatu.Name == "Open")
                    .ID;

                if (viewModel.SalesOrderBillingAddressID == 0)
                {
                    var oldAddress = context.lnksoaddresses
                        .Where(a => a.Active && a.IsBilling && a.SalesOrderID == salesOrderID).ToList();
                    oldAddress.ForEach(a => { a.Active = false; a.UpdateDT = DateTime.Now; });

                    context.lnksoaddresses.Add(new lnksoaddress
                    {
                        Active = true,
                        AddressLine1 = viewModel.Address.AddressLine1,
                        AddressLine2 = viewModel.Address.AddressLine2,
                        CityTown = viewModel.Address.CityTown,
                        CountryID = viewModel.Address.CountryID,
                        CreateDT = DateTime.Now,
                        FirstName = viewModel.FirstName,
                        IsBilling = true,
                        LastName = viewModel.LastName,
                        Phone = viewModel.Phone,
                        Postcode = viewModel.Address.Postcode,
                        SalesOrderID = salesOrderID,
                        State = viewModel.Address.State,
                        ChooseAddress = viewModel.AddressTypeID
                    });
                }
                else
                {
                    var salesOrderBillingAddress = context.lnksoaddresses.Single(a => a.ID == viewModel.SalesOrderBillingAddressID);
                    salesOrderBillingAddress.AddressLine1 = viewModel.Address.AddressLine1;
                    salesOrderBillingAddress.AddressLine2 = viewModel.Address.AddressLine2;
                    salesOrderBillingAddress.CityTown = viewModel.Address.CityTown;
                    salesOrderBillingAddress.CountryID = viewModel.Address.CountryID;
                    salesOrderBillingAddress.UpdateDT = DateTime.Now;
                    salesOrderBillingAddress.FirstName = viewModel.FirstName;
                    salesOrderBillingAddress.IsBilling = true;
                    salesOrderBillingAddress.LastName = viewModel.LastName;
                    salesOrderBillingAddress.Phone = viewModel.Phone;
                    salesOrderBillingAddress.Postcode = viewModel.Address.Postcode;
                    salesOrderBillingAddress.State = viewModel.Address.State;
                    salesOrderBillingAddress.ChooseAddress = viewModel.AddressTypeID;
                }

                var addressType = (CheckoutAddressType)viewModel.AddressTypeID;
                if (addressType != CheckoutAddressType.Others)
                {
                    var isPrimary = addressType == CheckoutAddressType.Primary;
                    var accountAddress = context.tbladdresses
                        .FirstOrDefault(a => a.IsPrimary == isPrimary && a.Active && a.AccessID == access.ID);
                    var billingExist = context.tbladdresses
                        .Any(a => a.Active && a.AccessID == access.ID && a.IsBilling && a.IsPrimary != isPrimary);
                    var isBilling = billingExist ? false : true;

                    if (accountAddress == null)
                    {
                        accountAddress = new tbladdress
                        {
                            AccessID = access.ID,
                            Active = true,
                            AddressLine1 = viewModel.Address.AddressLine1,
                            AddressLine2 = viewModel.Address.AddressLine2,
                            CityTown = viewModel.Address.CityTown,
                            CountryID = viewModel.Address.CountryID,
                            CreateDT = DateTime.Now,
                            IsBilling = isBilling,
                            IsPrimary = isPrimary,
                            Postcode = viewModel.Address.Postcode,
                            State = viewModel.Address.State
                        };
                        context.tbladdresses.Add(accountAddress);
                    }
                    else
                    {
                        accountAddress.IsBilling = isBilling;
                    }
                }

                context.SaveChanges();

                var soAddress = context.lnksoaddresses.FirstOrDefault(a => a.SalesOrderID == salesOrderID && a.Active && a.IsShipping);
                chooseAddress = soAddress == null ? 0 : soAddress.ChooseAddress;
            }

            return RedirectToAction(MVC.Checkout.Shipping(chooseAddress));
        }

        public virtual ActionResult Shipping(int chooseAddress = 0)
        {
            var access = Util.SessionAccess;
            var viewModel = new CheckoutShippingViewModel();
            var addressType = (CheckoutAddressType)chooseAddress;

            using (var context = new TTTEntities())
            {
                var salesOrder = context.trnsalesorders
                    .Include(a => a.refstatu)
                    .SingleOrDefault(a => a.Email == access.Email && a.refstatu.Name == "Open");

                if (salesOrder == null)
                    throw new ApplicationException("No Order found.");

                var soShippingAddress = context.lnksoaddresses
                    .FirstOrDefault(a => a.SalesOrderID == salesOrder.ID && a.Active && a.IsShipping);
                var shippingAddress = context.tbladdresses
                    .FirstOrDefault(a => a.AccessID == access.ID && a.Active && a.IsShipping);

                switch (addressType)
                {
                    case CheckoutAddressType.None:
                        if (soShippingAddress == null)
                        {
                            if (shippingAddress == null)
                            {
                                var primayAddress = context.tbladdresses
                                    .FirstOrDefault(a => a.AccessID == access.ID && a.Active && a.IsPrimary);

                                if (primayAddress == null)
                                {
                                    var secondAddress = context.tbladdresses
                                        .FirstOrDefault(a => a.AccessID == access.ID && a.Active && !a.IsPrimary);

                                    if (secondAddress == null)
                                    {
                                        addressType = CheckoutAddressType.Primary;
                                    }
                                    else
                                    {
                                        addressType = CheckoutAddressType.Secondary;
                                        viewModel.Address = ConstructMyAccountAddress(secondAddress);
                                        viewModel.IsAddressReadOnly = true;
                                    }
                                }
                                else
                                {
                                    addressType = CheckoutAddressType.Primary;
                                    viewModel.Address = ConstructMyAccountAddress(primayAddress);
                                    viewModel.IsAddressReadOnly = true;
                                }
                            }
                            else
                            {
                                addressType = shippingAddress.IsPrimary ? CheckoutAddressType.Primary : CheckoutAddressType.Secondary;
                                viewModel.Address = ConstructMyAccountAddress(shippingAddress);
                                viewModel.IsAddressReadOnly = true;
                            }
                        }
                        else
                        {
                            viewModel.Address = ConstructMyAccountAddress(soShippingAddress);
                            viewModel.IsAddressReadOnly = soShippingAddress.ChooseAddress != (int)CheckoutAddressType.Others;
                            viewModel.SalesOrderAddressID = soShippingAddress.ID;
                            addressType = (CheckoutAddressType)soShippingAddress.ChooseAddress;
                        }

                        break;

                    case CheckoutAddressType.Primary:
                    case CheckoutAddressType.Secondary:
                        var isPrimary = addressType == CheckoutAddressType.Primary;
                        var accountAddress = context.tbladdresses
                            .FirstOrDefault(a => a.AccessID == access.ID && a.Active && a.IsPrimary == isPrimary);

                        if (accountAddress != null)
                        {
                            viewModel.Address = ConstructMyAccountAddress(accountAddress);
                            viewModel.IsAddressReadOnly = true;
                        }

                        break;

                    case CheckoutAddressType.Others:
                        if (soShippingAddress != null && soShippingAddress.ChooseAddress == (int)CheckoutAddressType.Others)
                        {
                            viewModel.Address = ConstructMyAccountAddress(soShippingAddress);
                            viewModel.SalesOrderAddressID = soShippingAddress.ID;
                        }

                        break;

                    default:
                        break;
                }

                if (addressType == CheckoutAddressType.None)
                    addressType = CheckoutAddressType.Others;
                viewModel.AddressTypeID = (int)addressType;


                if (soShippingAddress != null && soShippingAddress.ChooseAddress == (int)addressType)
                {
                    viewModel.FirstName = soShippingAddress.FirstName;
                    viewModel.LastName = soShippingAddress.LastName;
                    viewModel.Phone = string.IsNullOrEmpty(soShippingAddress.Phone) ? access.Phone : soShippingAddress.Phone;
                }
                else
                {
                    viewModel.FirstName = access.FirstName;
                    viewModel.LastName = access.LastName;
                    viewModel.Phone = access.Phone;
                }
            }

            viewModel.AddressType = addressType;

            if (viewModel.Address.CountryID == 0)
                viewModel.Address.CountryID = Util.GetDefaultCountryID(); 

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Shipping(CheckoutShippingViewModel viewModel)
        {
            var access = Util.SessionAccess;

            using (var context = new TTTEntities())
            {
                if (viewModel.SalesOrderAddressID == 0)
                {
                    var salesOrderID = context.trnsalesorders
                        .Include(a => a.refstatu)
                        .Single(a => a.Email == access.Email && a.refstatu.Name == "Open")
                        .ID;

                    var oldAddress = context.lnksoaddresses
                        .Where(a => a.Active && a.IsShipping && a.SalesOrderID == salesOrderID).ToList();
                    oldAddress.ForEach(a => { a.Active = false; a.UpdateDT = DateTime.Now; });

                    context.lnksoaddresses.Add(new lnksoaddress
                    {
                        Active = true,
                        AddressLine1 = viewModel.Address.AddressLine1,
                        AddressLine2 = viewModel.Address.AddressLine2,
                        CityTown = viewModel.Address.CityTown,
                        CountryID = viewModel.Address.CountryID,
                        CreateDT = DateTime.Now,
                        FirstName = viewModel.FirstName,
                        IsShipping = true,
                        LastName = viewModel.LastName,
                        Postcode = viewModel.Address.Postcode,
                        SalesOrderID = salesOrderID,
                        State = viewModel.Address.State,
                        ChooseAddress = viewModel.AddressTypeID,
                        Phone = viewModel.Phone
                    });
                }
                else
                {
                    var salesOrderShippingAddress = context.lnksoaddresses.Single(a => a.ID == viewModel.SalesOrderAddressID);
                    salesOrderShippingAddress.AddressLine1 = viewModel.Address.AddressLine1;
                    salesOrderShippingAddress.AddressLine2 = viewModel.Address.AddressLine2;
                    salesOrderShippingAddress.CityTown = viewModel.Address.CityTown;
                    salesOrderShippingAddress.CountryID = viewModel.Address.CountryID;
                    salesOrderShippingAddress.UpdateDT = DateTime.Now;
                    salesOrderShippingAddress.FirstName = viewModel.FirstName;
                    salesOrderShippingAddress.IsShipping = true;
                    salesOrderShippingAddress.LastName = viewModel.LastName;
                    salesOrderShippingAddress.ChooseAddress = viewModel.AddressTypeID;
                    salesOrderShippingAddress.Postcode = viewModel.Address.Postcode;
                    salesOrderShippingAddress.State = viewModel.Address.State;
                    salesOrderShippingAddress.Phone = viewModel.Phone;
                }

                var addressType = (CheckoutAddressType)viewModel.AddressTypeID;
                if (addressType != CheckoutAddressType.Others)
                {
                    var isPrimary = addressType == CheckoutAddressType.Primary;
                    var accountAddress = context.tbladdresses
                        .FirstOrDefault(a => a.IsPrimary == isPrimary && a.Active && a.AccessID == access.ID);
                    var shippingExist = context.tbladdresses
                        .Any(a => a.Active && a.AccessID == access.ID && a.IsShipping && a.IsPrimary != isPrimary);
                    var isShipping = shippingExist ? false : true;

                    if (accountAddress == null)
                    {
                        accountAddress = new tbladdress
                        {
                            AccessID = access.ID,
                            Active = true,
                            AddressLine1 = viewModel.Address.AddressLine1,
                            AddressLine2 = viewModel.Address.AddressLine2,
                            CityTown = viewModel.Address.CityTown,
                            CountryID = viewModel.Address.CountryID,
                            CreateDT = DateTime.Now,
                            IsShipping = isShipping,
                            IsPrimary = isPrimary,
                            Postcode = viewModel.Address.Postcode,
                            State = viewModel.Address.State
                        };
                        context.tbladdresses.Add(accountAddress);
                    }
                    else
                    {
                        accountAddress.IsShipping = isShipping;
                    }
                }

                context.SaveChanges();
            }

            return RedirectToAction(MVC.Checkout.Summary());
        }

        public virtual ActionResult Summary()
        {
            var access = Util.SessionAccess;
            var viewModel = new CheckoutSummaryViewModel();
            var openStatus = Status.Open.ToString();

            using (var context = new TTTEntities())
            {
                var salesOrder = context.trnsalesorders
                    .Include(a => a.refstatu)
                    .SingleOrDefault(a => a.Email == access.Email && a.refstatu.Name == openStatus);

                if (salesOrder == null)
                    throw new ApplicationException("No Order found.");

                viewModel.AllowPayment = salesOrder.refstatu.Name == openStatus;

                viewModel.SalesOrderID = salesOrder.ID;

                viewModel.BillingAddress = context.lnksoaddresses
                    .Include(a => a.refcountry)
                    .Single(a => a.IsBilling && a.Active && a.SalesOrderID == salesOrder.ID);

                viewModel.ShippingAddress = context.lnksoaddresses
                    .Include(a => a.refcountry)
                    .Single(a => a.IsShipping && a.Active && a.SalesOrderID == salesOrder.ID);

                var items =
                    (from a in context.lnksalesorders
                     join b in context.lnkmodelsizes on a.ModelSizeID equals b.ID into bTemp
                     from bb in bTemp.DefaultIfEmpty()
                     join c in context.lnkmodelcolourdescs on new { ColourDescID = bb.ColourDescID, ModelID = bb.ModelID }
                         equals new { ColourDescID = c.ColourDescID, ModelID = c.ModelID } into cTemp
                     from cc in cTemp.DefaultIfEmpty()
                     join d in context.refmodels on cc.ModelID equals d.ID into dTemp
                     from dd in dTemp.DefaultIfEmpty()
                     join e in context.refbrands on dd.BrandID equals e.ID into eTemp
                     from ee in eTemp.DefaultIfEmpty()
                     join f in context.refcategories on ee.CategoryID equals f.ID into fTemp
                     from ff in fTemp.DefaultIfEmpty()
                     join h in context.refcolourdescs on cc.ColourDescID equals h.ID into hTemp
                     from hh in hTemp.DefaultIfEmpty()
                     join i in context.refcolours on hh.ColourID equals i.ID into iTemp
                     from ii in iTemp.DefaultIfEmpty()
                     join g in context.lnkcustomizemodelimages on a.SKU equals g.SKU into gTemp
                     from gg in gTemp.DefaultIfEmpty()
                     join j in context.refcustomizemodels on gg.CustomizeModelID equals j.ID into jTemp
                     from jj in jTemp.DefaultIfEmpty()
                     where a.SalesOrderID == salesOrder.ID && a.Active
                     select new
                     {
                         Color = a.ModelSizeID.HasValue ? hh.Name : gg.Colour,
                         ImageUrl = a.ModelSizeID.HasValue ? "~/images/" + ff.Name + "/"
                            : "~/Images/Customize/ShoeColour/" + gg.MainImage,
                         Name = a.ModelSizeID.HasValue ? dd.Name : jj.Name,
                         Price = a.ModelSizeID.HasValue ? dd.Price : jj.Price,
                         Quantity = a.Quantity,
                         Size = a.ModelSizeID.HasValue ? bb.Size : a.Size,
                         ModelSizeID = a.ModelSizeID,
                         ModelColourDescID = a.ModelSizeID.HasValue ? cc.ID : 0
                     }).Distinct().ToList();

                viewModel.Items = new List<CheckoutSummaryItem>();

                foreach (var item in items)
                {
                    var summaryItem = new CheckoutSummaryItem
                    {
                        Color = item.Color,
                        ImageUrl = item.ImageUrl,
                        Name = item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Size = item.Size
                    };

                    if (item.ModelSizeID.HasValue)
                    {
                        var modelImage = 
                            context.lnkmodelimages.FirstOrDefault(a => a.ModelColourDescID == item.ModelColourDescID);

                        if (modelImage != null)
                            summaryItem.ImageUrl += modelImage.Thumbnail;
                    }

                    viewModel.Items.Add(summaryItem);
                }

                viewModel.Subtotal = viewModel.Items.Sum(a => a.Price * a.Quantity);
                viewModel.ShippingPrice = Properties.Settings.Default.ShippingFee;
                viewModel.OrderTotalPrice = viewModel.Subtotal + viewModel.ShippingPrice;

                if (salesOrder.VoucheID.HasValue)
                {
                    var currentVoucher = context.tblvouchers.Where(a => a.ID == salesOrder.VoucheID.Value).Single();

                    viewModel.VoucherCode = currentVoucher.Code;
                    viewModel.RebateCashValue = currentVoucher.Value;
                    viewModel.OrderTotalPrice -= currentVoucher.Value;
                }
            }

            //foreach (var item in viewModel.Items)
            //{
            //    item.Size = Util.GetSizeText(Convert.ToInt32(item.Size));
            //}

            var paymentAmount = 1.00; // Convert.ToDouble(viewModel.OrderTotalPrice)
            var paymentInfo = new CheckoutPaymentInfo
            {
                Amount = paymentAmount.ToString("N"),
                BackendUrl = Url.ActionAbsolute(MVC.Payment.BackendResponse()),
                Currency = Settings.Default.iPay88_Currency,
                EntryUrl = Settings.Default.iPay88_EntryUrl,
                Lang = Settings.Default.iPay88_Lang,
                MerchantCode = Settings.Default.iPay88_MerchantCode,
                PaymentId = "",
                ProdDesc = "Sales Order",
                RefNo = viewModel.SalesOrderID.ToString(),
                Remark = "",
                ResponseUrl = Url.ActionAbsolute(MVC.Payment.StatusResponse()),
                UserContact = access.Phone,
                UserEmail = access.Email,
                UserName = access.FirstName + access.LastName
            };

            paymentInfo.Signature = Util.ConstructIPay88Signature(paymentInfo.RefNo, paymentAmount);
            viewModel.PaymentInfo = paymentInfo;

            return View(viewModel);
        }

        [HttpPost]
        public virtual JsonResult ValidateVoucherCode(string voucherCode, int soID)
        {
            var message = "";
            var voucherValue = 0;
            decimal grandTotal = 0;

            using (var context = new TTTEntities())
            {
                // retrieve voucher details
                var voucherDetails = context.tblvouchers.Where(a => a.Code == voucherCode).ToList();
                
                // retrieve sales order
                var salesOrder = context.trnsalesorders.Where(a => a.ID == soID).Single();

                // current voucher code
                if(salesOrder.VoucheID == voucherDetails[0].ID)
                    return Json(new { voucherValue = voucherDetails[0].Value, grandTotal = salesOrder.GrandTotal, message = message });

                if (!string.IsNullOrEmpty(voucherCode))
                {
                    if (voucherDetails.Count > 1) // voucher code more than one
                        message = string.Format("Voucher appeared more than once");
                    else if (voucherDetails.Count == 0) // voucher code not found
                        message = string.Format("Voucher not found");
                    else //voucher code found
                    {
                        if (!voucherDetails[0].Active) // voucher code 
                            message = string.Format("Voucher already used");
                    }
                }

                if (salesOrder.VoucheID.HasValue)
                {
                    var currentVoucher = context.tblvouchers.Where(a => a.ID == salesOrder.VoucheID).SingleOrDefault();

                    // update voucher as un-used
                    currentVoucher.Active = true;
                    currentVoucher.UpdateDT = DateTime.Now;

                    //update sales order amount
                    salesOrder.VoucheID = null;
                    salesOrder.UpdateDT = DateTime.Now;
                    salesOrder.GrandTotal += currentVoucher.Value;
                }

                if (string.IsNullOrEmpty(message)) // valid voucher code
                {
                    //update sales order amount
                    salesOrder.VoucheID = voucherDetails[0].ID;
                    salesOrder.UpdateDT = DateTime.Now;
                    salesOrder.GrandTotal -= voucherDetails[0].Value;

                    // update voucher as used
                    voucherDetails[0].Active = false;
                    voucherDetails[0].UpdateDT = DateTime.Now;

                    voucherValue = voucherDetails[0].Value;
                }

                grandTotal = salesOrder.GrandTotal;

                context.SaveChanges();
            }

            return Json(new { voucherValue = voucherValue, grandTotal = grandTotal, message = message });
        }

        public virtual ActionResult Success(int id)
        {
            var access = Util.SessionAccess;
            var viewModel = new CheckoutSuccessViewModel
            {
                SalesOrderID = id,
                Name = access.FirstName + access.LastName
            };

            using (var context = new TTTEntities())
            {
                viewModel.Quantity = 
                    context.lnksalesorders
                        .Where(a => a.SalesOrderID == id && a.Active)
                        .Sum(a => a.Quantity);
            }

            return View(viewModel);
        }
    }
}
