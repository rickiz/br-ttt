using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Helpers;
using BR.ToteToToe.Web.ViewModels;
using System.Transactions;

namespace BR.ToteToToe.Web.Controllers
{
    [Authorize]
    public partial class MyAccountController : TTTBaseController
    {
        public virtual ActionResult Index()
        {
            var imagePath = "~/images/";

            using (var context = new TTTEntities())
            {
                var user = context.tblaccesses.Single(a => a.ID == Util.SessionAccess.ID);
                var addresses = context.tbladdresses.Include(a => a.refcountry).Where(a => a.AccessID == user.ID).ToList();
                var recentOrders =
                    context.trnsalesorders
                        .Include(a => a.refstatu)
                        .Where(a => a.Email == user.Email && a.refstatu.Name != "Open")
                        .OrderByDescending(a => a.ID)
                        .Take(5)
                        .ToList();

                var recentOrderIDs = recentOrders.Select(a => a.ID).ToArray();

                var recentViewCookie = Request.Cookies[Util.GetRecentViewCookieName()];
                var recentOrderImages = new List<MyAccountIndexImageDetail>();

                if (recentViewCookie != null)
                {
                    var cookieID = recentViewCookie.Value;
                    recentOrderImages =
                        (from a in context.tblrecentviews
                         where a.Active && a.CookieID == cookieID
                         orderby a.ID descending
                         select new MyAccountIndexImageDetail
                         {
                             ImageContentUrl = imagePath + a.ImageUrl,
                             LinkUrl = a.LinkUrl
                         }).Take(5).ToList();
                }

                var wishlistItems =
                     (from wishlistItem in context.lnkwishlists
                      join wishlist in context.trnwishlists on wishlistItem.WishlistID equals wishlist.ID
                      join b in context.lnkmodelsizes on wishlistItem.ModelSizeID equals b.ID into bTemp
                      from bb in bTemp.DefaultIfEmpty()
                      join c in context.lnkmodelcolourdescs on new { ColourDescID = bb.ColourDescID, ModelID = bb.ModelID }
                        equals new { ColourDescID = c.ColourDescID, ModelID = c.ModelID } into cTemp
                      from cc in cTemp.DefaultIfEmpty()
                      join d in context.refmodels on bb.ModelID equals d.ID into dTemp
                      from dd in dTemp.DefaultIfEmpty()
                      join e in context.refbrands on dd.BrandID equals e.ID into eTemp
                      from ee in eTemp.DefaultIfEmpty()
                      join f in context.refcategories on ee.CategoryID equals f.ID into fTemp
                      from ff in fTemp.DefaultIfEmpty()
                      join g in context.lnkcustomizemodelimages on wishlistItem.SKU equals g.SKU into gTemp
                      from gg in gTemp.DefaultIfEmpty()
                      where wishlist.Email == user.Email && wishlist.Active && wishlistItem.Active
                      select new
                      {
                          ModelColourDescID = wishlistItem.ModelSizeID.HasValue ? cc.ID : 0,
                          ModelSizeID = wishlistItem.ModelSizeID,
                          ModelID = bb == null ? 0 : bb.ModelID,
                          ColourDescID = bb == null ? 0 : bb.ColourDescID,
                          SKU = gg.SKU,
                          ImageContentUrl =
                            wishlistItem.ModelSizeID.HasValue ?
                                imagePath + ff.Name + "/"
                                : imagePath + gg.MainImage
                      }).Distinct().Take(5).ToList();
                      
                var wishlistImages = new List<MyAccountIndexImageDetail>();

                foreach (var item in wishlistItems)
                {
                    var imageDetail = new MyAccountIndexImageDetail
                    {
                        ModelSizeID = item.ModelSizeID,
                        ModelID = item.ModelID,
                        ColourDescID = item.ColourDescID,
                        ImageContentUrl = item.ImageContentUrl,
                        SKU = item.SKU
                    };

                    if (item.ModelSizeID.HasValue)
                    {
                        var modelImage =
                            context.lnkmodelimages.FirstOrDefault(a => a.ModelColourDescID == item.ModelColourDescID);

                        if (modelImage != null)
                            imageDetail.ImageContentUrl += modelImage.Thumbnail;
                    }

                    wishlistImages.Add(imageDetail);
                }

                var viewModel = new MyAccountViewModel
                {
                    Addresses = addresses,
                    RecentOrderImages = recentOrderImages,
                    RecentOrders = recentOrders,
                    User = user,
                    WishlistImages = wishlistImages
                };

                return View(viewModel);
            }
        }

        public virtual ActionResult Edit()
        {
            var accessID = Util.SessionAccess.ID;

            using (var context = new TTTEntities())
            {
                var access = context.tblaccesses.Single(a => a.ID == accessID);
                var primaryAddress = context.tbladdresses.SingleOrDefault(a => a.Active && a.IsPrimary && a.AccessID == accessID);
                var secondaryAddress = context.tbladdresses.FirstOrDefault(a => a.Active && !a.IsPrimary && a.AccessID == accessID);

                var viewModel = new MyAccountEditViewModel
                {
                    AccessID = access.ID,
                    Email = access.Email,
                    FirstName = access.FirstName,
                    LastName = access.LastName,
                    Phone = access.Phone,
                    Gender = string.IsNullOrEmpty(access.Gender) ? null : (Gender?)Util.ParseEnum<Gender>(access.Gender),
                    BirthDateDay = access.BirthDateDay,
                    BirthDateMonth = access.BirthDateMonth,
                    BirthDateYear = access.BirthDateYear,
                    //Password = access.Password,
                    //ConfirmPassword = access.Password,
                    IsFBLogin = !string.IsNullOrEmpty(access.FacebookID)
                };

                if (primaryAddress != null)
                {
                    viewModel.PrimaryAddress = ConvertToMyAccountAddress(primaryAddress);
                    viewModel.BillingFlag = primaryAddress.IsBilling ? 1 : viewModel.BillingFlag;
                    viewModel.ShippingFlag = primaryAddress.IsShipping ? 1 : viewModel.ShippingFlag;

                    if (!string.IsNullOrEmpty(primaryAddress.AddressLine1)
                        && !string.IsNullOrEmpty(primaryAddress.AddressLine2)
                        && !string.IsNullOrEmpty(primaryAddress.CityTown)
                        && !string.IsNullOrEmpty(primaryAddress.Postcode)
                        && !string.IsNullOrEmpty(primaryAddress.State))
                    {
                        viewModel.EnableHomeFlag = true;
                    }
                }

                if (secondaryAddress != null)
                {
                    viewModel.SecondaryAddress = ConvertToMyAccountAddress(secondaryAddress);
                    viewModel.BillingFlag = secondaryAddress.IsBilling ? 2 : viewModel.BillingFlag;
                    viewModel.ShippingFlag = secondaryAddress.IsShipping ? 2 : viewModel.ShippingFlag;

                    if (!string.IsNullOrEmpty(secondaryAddress.AddressLine1)
                        && !string.IsNullOrEmpty(secondaryAddress.AddressLine2)
                        && !string.IsNullOrEmpty(secondaryAddress.CityTown)
                        && !string.IsNullOrEmpty(secondaryAddress.Postcode)
                        && !string.IsNullOrEmpty(secondaryAddress.State))
                    {
                        viewModel.EnableWorkFlag = true;
                    }
                }

                var defaultCountryID = Util.GetDefaultCountryID(); ;
                if (!viewModel.PrimaryAddress.CountryID.HasValue || viewModel.PrimaryAddress.CountryID == 0)
                    viewModel.PrimaryAddress.CountryID = defaultCountryID;

                if (!viewModel.SecondaryAddress.CountryID.HasValue || viewModel.SecondaryAddress.CountryID == 0)
                    viewModel.SecondaryAddress.CountryID = defaultCountryID;

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(MyAccountEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            using (var trans = new TransactionScope())
            {
                using (var context = new TTTEntities())
                {
                    var access = context.tblaccesses.Single(a => a.ID == viewModel.AccessID);                    

                    access.Email = viewModel.Email; //TODO: Validation on existing email

                    if (!string.IsNullOrEmpty(viewModel.Password) && !viewModel.IsFBLogin)
                        access.Password = Util.GetMD5Hash(viewModel.Password);
                    access.FirstName = viewModel.FirstName;
                    access.LastName = viewModel.LastName;
                    access.BirthDateDay = viewModel.BirthDateDay;
                    access.BirthDateMonth = viewModel.BirthDateMonth;
                    access.BirthDateYear = viewModel.BirthDateYear;
                    access.Gender = viewModel.Gender.HasValue ? viewModel.Gender.Value.ToString() : null;
                    access.Phone = viewModel.Phone;

                    context.SaveChanges();

                    Util.SessionAccess = access;

                    EditAddresses(viewModel, context);

                    context.SaveChanges();
                }

                trans.Complete();
            }

            return RedirectToAction(MVC.MyAccount.Index());
        }

        private void EditAddresses(MyAccountEditViewModel viewModel, TTTEntities context)
        {
            var primaryAddress =
                context.tbladdresses
                    .SingleOrDefault(a => a.Active && a.IsPrimary && a.AccessID == viewModel.AccessID);
            var secondaryAddress =
                context.tbladdresses
                    .FirstOrDefault(a => a.Active && !a.IsPrimary && a.AccessID == viewModel.AccessID);

            primaryAddress = ConstructEditAddress(primaryAddress, viewModel.PrimaryAddress);
            secondaryAddress = ConstructEditAddress(secondaryAddress, viewModel.SecondaryAddress);

            if (viewModel.BillingFlag == 1)
            {
                primaryAddress.IsBilling = true;
                secondaryAddress.IsBilling = false;
            }
            else if (viewModel.BillingFlag == 2)
            {
                primaryAddress.IsBilling = false;
                secondaryAddress.IsBilling = true;
            }
            else
            {
                primaryAddress.IsBilling = false;
                secondaryAddress.IsBilling = false;
            }

            if (viewModel.ShippingFlag == 1)
            {
                primaryAddress.IsShipping = true;
                secondaryAddress.IsShipping = false;
            }
            else if (viewModel.ShippingFlag == 2)
            {
                primaryAddress.IsShipping = false;
                secondaryAddress.IsShipping = true;
            }
            else
            {
                primaryAddress.IsShipping = false;
                secondaryAddress.IsShipping = false;
            }

            if (ValidNewAddress(primaryAddress))
                context.tbladdresses.Add(primaryAddress);

            if (ValidNewAddress(secondaryAddress))
                context.tbladdresses.Add(secondaryAddress);

            var access = Util.SessionAccess;
            var salesOrder = context.trnsalesorders
                .Include(a => a.refstatu)
                .FirstOrDefault(a => a.Email == access.Email && a.refstatu.Name == "Open");
            if (salesOrder != null)
            {
                var soAddresses = context.lnksoaddresses.Where(a => a.SalesOrderID == salesOrder.ID && a.Active).ToList();
                foreach (var address in soAddresses)
                {
                    var addressType = (CheckoutAddressType)address.ChooseAddress;

                    switch (addressType)
                    {
                        case CheckoutAddressType.Primary:
                            BindToSalesOrderAddress(address, primaryAddress);
                            break;
                        case CheckoutAddressType.Secondary:
                            BindToSalesOrderAddress(address, secondaryAddress);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private MyAccountAddress ConvertToMyAccountAddress(tbladdress address)
        {
            var myAccountAddress = new MyAccountAddress
            {
                ID = address.ID,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                CityTown = address.CityTown,
                CountryID = address.CountryID,
                IsPrimary = address.IsPrimary,
                Postcode = address.Postcode,
                State = address.State
            };

            return myAccountAddress;
        }
        private tbladdress ConstructEditAddress(tbladdress address, MyAccountAddress modelAddress)
        {
            if (address == null)
            {
                address = new tbladdress
                {
                    AccessID = Util.SessionAccess.ID,
                    Active = true,
                    CreateDT = DateTime.Now
                };
            }
            else
            {
                address.UpdateDT = DateTime.Now;
            }

            address.AddressLine1 = modelAddress.AddressLine1;
            address.AddressLine2 = modelAddress.AddressLine2;
            address.CityTown = modelAddress.CityTown;
            address.CountryID = modelAddress.CountryID;
            address.Postcode = modelAddress.Postcode;
            address.State = modelAddress.State;
            address.IsPrimary = modelAddress.IsPrimary;

            return address;
        }
        private bool ValidNewAddress(tbladdress address)
        {
            var valid = false;

            valid = 
                address.ID == 0
                && !string.IsNullOrEmpty(address.AddressLine1)
                && !string.IsNullOrEmpty(address.AddressLine2)
                && !string.IsNullOrEmpty(address.CityTown)
                && !string.IsNullOrEmpty(address.Postcode)
                && !string.IsNullOrEmpty(address.State);

            return valid;
        }
        private void BindToSalesOrderAddress(lnksoaddress soAddress, tbladdress address)
        {
            soAddress.AddressLine1 = address.AddressLine1;
            soAddress.AddressLine2 = address.AddressLine2;
            soAddress.CityTown = address.CityTown;
            soAddress.CountryID = address.CountryID;
            soAddress.Postcode = address.Postcode;
            soAddress.State = address.State;
        }

        public virtual ActionResult OrderSummary(int id)
        {
            var access = Util.SessionAccess;
            var viewModel = new MyAccountOrderSummaryViewModel() { SalesOrderID = id };

            using (var context = new TTTEntities())
            {
                var salesOrder = context.trnsalesorders.Single(a => a.ID == id);

                if (salesOrder == null)
                    throw new ApplicationException("No Order found.");

                viewModel.BillingAddress = context.lnksoaddresses
                    .Include(a => a.refcountry)
                    .SingleOrDefault(a => a.IsBilling && a.Active && a.SalesOrderID == salesOrder.ID);

                viewModel.ShippingAddress = context.lnksoaddresses
                    .Include(a => a.refcountry)
                    .SingleOrDefault(a => a.IsShipping && a.Active && a.SalesOrderID == salesOrder.ID);

                viewModel.Items =
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
                     select new MyAccountOrderSummaryItem
                     {
                         Color = a.ModelSizeID.HasValue ? hh.Name : gg.Colour,
                         Name = a.ModelSizeID.HasValue ? dd.Name : jj.Name,
                         Price = a.ModelSizeID.HasValue ? dd.Price : jj.Price,
                         Quantity = a.Quantity,
                         Size = a.ModelSizeID.HasValue ? bb.Size : a.Size,
                         SKU = a.SKU
                     }).ToList();

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

                if (!string.IsNullOrEmpty(salesOrder.PaymentGatewayTransID))
                {
                    var salesOrderIDString = salesOrder.ID.ToString();
                    var logPayment = context.logpayments
                        .FirstOrDefault(a => a.TransId == salesOrder.PaymentGatewayTransID && a.RefNo == salesOrderIDString);

                    viewModel.PaymentDT = logPayment == null ? "" : logPayment.CreateDT.ToString("dd MMM yyyy, hh.mmsstt");
                    viewModel.PaymentStatus = "Paid";
                }
                
            }

            return View(viewModel);
        }

        public virtual ActionResult OrderHistory(int statusID = 0)
        {
            var access = Util.SessionAccess;
            var viewModel = new MyAccountOrderHistoryViewModel();
            var openStatus = Status.Open.ToString();

            using (var context = new TTTEntities())
            {
                var openStatusID = context.refstatus.Where(a => a.Name == openStatus).Single().ID;

                viewModel.Items =
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
                     join g in context.lnkcustomizemodelimages on a.SKU equals g.SKU into gTemp
                     from gg in gTemp.DefaultIfEmpty()
                     where (a.trnsalesorder.StatusID == statusID || statusID == 0)
                        && a.trnsalesorder.StatusID != openStatusID
                        && a.Active
                        && a.SalesOrderID.HasValue
                        && a.trnsalesorder.Email == access.Email
                     select new MyAccountOrderHistoryItem
                     {
                         ImageUrl = a.ModelSizeID.HasValue ? "~/images/" + ff.Name + "/" + cc.MainImage
                            : "~/Images/Customize/ShoeColour/" + gg.MainImage,
                         OrderDate = a.trnsalesorder.CreateDT,
                         Status = a.trnsalesorder.refstatu.Name,
                         SalesOrderID = a.SalesOrderID.Value
                     }).OrderByDescending(a => a.OrderDate).Take(16).ToList();
            }            

            return View(viewModel);
        }
    }
}
