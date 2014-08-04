using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.ViewModels;
using BR.ToteToToe.Web.Helpers;
using System.Web.Routing;
using System.Transactions;

namespace BR.ToteToToe.Web.Controllers
{
    public partial class ShoppingBagController : TTTBaseController
    {
        //
        // GET: /ShoppingBag/
        private HttpCookie shoppingBagCookie = null;
        private string cookieID = "";

        public virtual ActionResult Index(bool? rtn = null)
        {
            SetShoppingBagCookie();

            var shoppingBagViewModel = ConstructShoppingBagViewModel();

            var previousURI = this.Request.UrlReferrer;

            if (previousURI != null)
            {
                if (previousURI.ToString().Contains("/Shoes/") || previousURI.ToString().Contains("/Customize/"))
                {
                    shoppingBagViewModel.HasReturn = true;
                }
            }
            
            return View(shoppingBagViewModel);
        }
        private ShoppingBagIndexViewModel ConstructShoppingBagViewModel()
        {
            var shoppingBagViewModel = new ShoppingBagIndexViewModel();

            using (var context = new TTTEntities())
            {
                var salesOrderItems = GetPendingSOItems(context, shoppingBagViewModel);

                if (salesOrderItems.Count > 0)
                {
                    var soItemIDs = salesOrderItems.Select(a => a.ID).ToList();

                    GetNormalModel(shoppingBagViewModel, soItemIDs, context);

                    GetCustomizeModel(shoppingBagViewModel, soItemIDs, context);
                }
            }

            shoppingBagViewModel.ShoppingBagItems.Any(a => string.IsNullOrEmpty(a.CookieID));

            shoppingBagViewModel.ShoppingBagItems = shoppingBagViewModel.ShoppingBagItems.OrderBy(a => a.CookieID).ToList();

            shoppingBagViewModel.GrandTotal = shoppingBagViewModel.ShoppingBagItems.Sum(a => a.Quantity * a.Price);

            return shoppingBagViewModel;
        }
        private List<lnksalesorder> GetPendingSOItems(TTTEntities context, ShoppingBagIndexViewModel shoppingBagViewModel = null)
        {
            var soItemsA = new List<lnksalesorder>();
            var soItemsB = new List<lnksalesorder>();
            var salesOrderItems = new List<lnksalesorder>();

            var customerEmail = Util.GetCustomerEmail();

            if (!string.IsNullOrEmpty(customerEmail))
            {
                var openStatusID = Util.GetStatusID(Status.Open.ToString());

                var openSalesOrder = context.trnsalesorders.Where(a => a.Email == customerEmail && a.StatusID == openStatusID).SingleOrDefault();

                if (openSalesOrder != null)
                {
                    if (shoppingBagViewModel != null)
                        shoppingBagViewModel.SalesOrderID = openSalesOrder.ID;

                    soItemsA = context.lnksalesorders.Where(a => a.SalesOrderID == openSalesOrder.ID && a.Active).ToList();
                }
            }

            ConstructShoppingBagCookie();

            if (Request.Cookies[Util.GetShoppingBagCookieName()] != null)
            {
                soItemsB = context.lnksalesorders.Where(a => a.CookieID == cookieID && a.SalesOrderID == null && a.Active).ToList();
            }

            salesOrderItems = soItemsA.Union(soItemsB).ToList();

            return salesOrderItems;
        }
        private void GetNormalModel(ShoppingBagIndexViewModel shoppingBagViewModel, List<int> soItemIDs, TTTEntities context)
        {
            var query = from salesOrderItem in context.lnksalesorders
                        join modelSize in context.lnkmodelsizes on salesOrderItem.ModelSizeID equals modelSize.ID
                        join model in context.refmodels on modelSize.ModelID equals model.ID
                        join colourDesc in context.refcolourdescs on modelSize.ColourDescID equals colourDesc.ID
                        join modelColourDesc in context.lnkmodelcolourdescs on
                            new { ColourDescID = modelSize.ColourDescID, ModelID = modelSize.ModelID } equals
                            new { ColourDescID = modelColourDesc.ColourDescID, ModelID = modelColourDesc.ModelID }
                        join modelImage in context.lnkmodelimages on modelColourDesc.ID equals modelImage.ModelColourDescID
                        join brand in context.refbrands on model.BrandID equals brand.ID
                        join category in context.refcategories on brand.CategoryID equals category.ID
                        where soItemIDs.Contains(salesOrderItem.ID)
                        select new
                        {
                            SalesOrderItemID = salesOrderItem.ID,
                            Image = modelImage.Image,
                            Description = model.Description,
                            ModelName = model.Name,
                            BrandName = brand.Name,
                            Colour = colourDesc.Name,
                            Size = modelSize.Size,
                            AvailableQty = modelSize.Quantity,
                            Quantity = salesOrderItem.Quantity,
                            Price = (model.DiscountPrice.HasValue && model.DiscountPrice.Value > 0) ? model.DiscountPrice.Value : model.Price,
                            Active = salesOrderItem.Active,
                            CategoryName = category.Name,
                            ModelSizeID = modelSize.ID,
                            ModelID = model.ID,
                            ColourDescID = colourDesc.ID,
                            SKU = salesOrderItem.SKU,
                            CookieID = salesOrderItem.CookieID
                        };

            var salesOrderDetails = query.Where(a => a.Active).ToList();

            if (salesOrderDetails.Count > 0)
            {
                var uniqueSOItemIDs = salesOrderDetails.Select(a => a.SalesOrderItemID).Distinct().ToList();

                foreach (var id in uniqueSOItemIDs)
                {
                    var item = salesOrderDetails.Where(a => a.SalesOrderItemID == id).First();

                    shoppingBagViewModel.GrandTotal += item.Price;

                    shoppingBagViewModel.ShoppingBagItems.Add(new ShoppingBagItem()
                    {
                        AvailableQuantity = item.AvailableQty,
                        Availability = item.AvailableQty >= 5 ? string.Format("{0} pairs left", item.AvailableQty) : string.Format("Only {0} pairs left", item.AvailableQty),
                        Brand = item.BrandName,
                        ColourDescName = item.Colour,
                        Image = string.Format("~/Images/{0}/{1}", item.CategoryName.Replace(" ", ""), item.Image),
                        ModelName = item.ModelName,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        //Size = Util.GetSizeText(Convert.ToInt32(item.Size)),
                        Size = item.Size,
                        SalesOrderItemID = item.SalesOrderItemID,
                        ModelSizeID = item.ModelSizeID,
                        ModelID = item.ModelID,
                        ColourDescID = item.ColourDescID,
                        SKU = item.SKU,
                        CookieID = item.CookieID
                    });
                }
            }
            //if (salesOrderDetails.Count > 0)
            //{
            //    shoppingBagViewModel.GrandTotal += salesOrderDetails.Sum(a => a.Price);

            //    foreach (var item in salesOrderDetails)
            //    {
            //        shoppingBagViewModel.ShoppingBagItems.Add(new ShoppingBagItem()
            //        {
            //            AvailableQuantity = item.AvailableQty,
            //            Availability = item.AvailableQty >= 5 ? string.Format("{0} pairs left", item.AvailableQty) : string.Format("Only {0} pairs left", item.AvailableQty),
            //            Brand = item.BrandName,
            //            ColourDescName = item.Colour,
            //            Image = string.Format("~/Images/{0}/{1}", item.CategoryName.Replace(" ",""), item.Image),
            //            ModelName = item.ModelName,
            //            Price = item.Price,
            //            Quantity = item.Quantity,
            //            //Size = Util.GetSizeText(Convert.ToInt32(item.Size)),
            //            Size = item.Size,
            //            SalesOrderItemID = item.SalesOrderItemID,
            //            ModelSizeID = item.ModelSizeID,
            //            ModelID = item.ModelID,
            //            ColourDescID = item.ColourDescID,
            //            SKU = item.SKU,
            //            CookieID = item.CookieID
            //        });
            //    }
            //}
        }
        private void GetCustomizeModel(ShoppingBagIndexViewModel shoppingBagViewModel, List<int> soItemIDs, TTTEntities context)
        {
            var query = from salesOrderItem in context.lnksalesorders
                        join customizeModelImage in context.lnkcustomizemodelimages on salesOrderItem.SKU equals customizeModelImage.SKU
                        join customizeModel in context.refcustomizemodels on customizeModelImage.CustomizeModelID equals customizeModel.ID
                        where soItemIDs.Contains(salesOrderItem.ID)
                        select new
                        {
                            SalesOrderItemID = salesOrderItem.ID,
                            Image = customizeModel.MainImage,
                            Description = customizeModel.Description,
                            ModelName = customizeModel.Name,
                            BrandName = "",
                            Colour = customizeModelImage.Colour,
                            Size = salesOrderItem.Size,
                            AvailableQty = 0,
                            Quantity = salesOrderItem.Quantity,
                            Price = customizeModel.Price,
                            Active = salesOrderItem.Active,
                            CategoryName = "",
                            SKU = salesOrderItem.SKU,
                            CookieID = salesOrderItem.CookieID
                        };

            var salesOrderDetails = query.Where(a => a.Active).ToList();

            if (salesOrderDetails.Count > 0)
            {
                shoppingBagViewModel.GrandTotal += salesOrderDetails.Sum(a => a.Price);

                foreach (var item in salesOrderDetails)
                {
                    shoppingBagViewModel.ShoppingBagItems.Add(new ShoppingBagItem()
                    {
                        Availability = "Custom Made",
                        Brand = item.BrandName,
                        ColourDescName = item.Colour,
                        Image = string.Format("~/Images/Customize/ShoeStyle/{0}", item.Image),
                        ModelName = item.ModelName,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Size = item.Size,
                        SalesOrderItemID = item.SalesOrderItemID,
                        SKU = item.SKU,
                        CookieID = item.CookieID
                    });
                }
            }
        }

        public virtual ActionResult Remove(int soID)
        {
            decimal price = 0;

            using (var context = new TTTEntities())
            {
                var soItem = context.lnksalesorders.Where(a => a.ID == soID).Single();
                soItem.Active = false;
                soItem.UpdateDT = DateTime.Now;

                if (soItem.ModelSizeID.HasValue) // normal model
                {
                    var query = from modelSize in context.lnkmodelsizes
                                join model in context.refmodels on modelSize.ModelID equals model.ID
                                select new { modelSize, model };

                    var soItemModelSize = query.Where(a => a.modelSize.ID == soItem.ModelSizeID).Single();

                    var discountPrice = soItemModelSize.model.DiscountPrice.HasValue ? soItemModelSize.model.DiscountPrice.Value : 0;

                    price = discountPrice != 0 ? soItemModelSize.model.DiscountPrice.Value : soItemModelSize.model.Price;
                }
                else // customize model
                {
                    var query = from customizeModelImage in context.lnkcustomizemodelimages
                                join customizeModel in context.refcustomizemodels on customizeModelImage.CustomizeModelID equals customizeModel.ID
                                select new { customizeModelImage, customizeModel };

                    price = query.Where(a => a.customizeModelImage.SKU == soItem.SKU).Single().customizeModel.Price;
                }

                if (!string.IsNullOrEmpty(Util.GetCustomerEmail()))
                {
                    var so = context.trnsalesorders.Where(a => a.ID == soItem.SalesOrderID).SingleOrDefault();

                    if (so != null)
                    {
                        so.GrandTotal -= soItem.Quantity * price;
                        so.UpdateDT = DateTime.Now;
                    }
                }

                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public virtual ActionResult LinkToAccount()
        {
            CreateUpdateSOItems();

            return RedirectToAction("Index");
        }
        private void CreateUpdateSOItems()
        {
            SetShoppingBagCookie();

            var salesOrderItems = new List<lnksalesorder>();
            var customerEmail = Util.GetCustomerEmail();

            using (var trans = new TransactionScope())
            {
                using (var context = new TTTEntities())
                {
                    var openStatusID = Util.GetStatusID(Status.Open.ToString());
                    var so = context.trnsalesorders.Where(a => a.Email == customerEmail && a.StatusID == openStatusID).SingleOrDefault();

                    salesOrderItems = context.lnksalesorders.Where(a => a.Active && a.CookieID == cookieID).ToList();

                    if (so == null)
                    {
                        so = context.trnsalesorders.Add(new trnsalesorder()
                        {
                            StatusID = openStatusID,
                            Email = customerEmail,
                            SubTotal = 0,
                            GrandTotal = salesOrderItems.Sum(b => b.UnitPrice),
                            CreateDT = DateTime.Now
                        });
                    }

                    foreach (var soItem in salesOrderItems)
                    {
                        soItem.CookieID = "";
                        soItem.SalesOrderID = so.ID;
                        soItem.UpdateDT = DateTime.Now;
                    }

                    context.SaveChanges();
                }
                trans.Complete();
            }
        }

        [HttpPost]
        public virtual ActionResult Index(FormCollection collection)
        {
            if (collection["submit"].ToString() == "VIEW WISH LIST")
                return RedirectToAction("Index", "Wishlist");
            else if (collection["submit"].ToString() == "PROCEED TO CHECKOUT")
                return RedirectToAction("Billing", "Checkout");

            // here update SalesOrder
            using (var context = new TTTEntities())
            {
                var customerEmail = Util.GetCustomerEmail();
                var openStatusID = Util.GetStatusID(Status.Open.ToString());
                var openSalesOrder = context.trnsalesorders.Where(a => (a.Email == customerEmail && a.StatusID == openStatusID)).SingleOrDefault();

                var salesItems = GetPendingSOItems(context, null);;

                // update sales order quantity and total amount
                foreach (var item in salesItems)
                {
                    var txtQty = string.Format("qty_{0}",item.ID);
                    //var hdfPrice = string.Format("price_{0}", item.ID);
                    var newQty = Convert.ToInt32(collection[txtQty].ToString());
                    var nowDT = DateTime.Now;

                    // price = price_[soID]
                    if (!(newQty == item.Quantity))
                    {
                        openSalesOrder.GrandTotal -= item.Quantity * item.UnitPrice;

                        item.Quantity = newQty;
                        item.UpdateDT = nowDT;

                        openSalesOrder.GrandTotal += item.Quantity * item.UnitPrice;
                    }
                }

                // update database
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public int PendingCheckoutCount()
        {
            SetShoppingBagCookie();

            var count = 0;

            using (var context = new TTTEntities())
            {
                //var customerEmail = Util.GetCustomerEmail();

                //if (!string.IsNullOrEmpty(customerEmail))
                //{
                //    var openStatusID = Util.GetStatusID(Status.Open.ToString());

                //    var so = context.trnsalesorders.Where(a => a.Email == customerEmail && a.StatusID == openStatusID).SingleOrDefault();

                //    if (so != null)
                //        count = context.lnksalesorders.Where(a => a.SalesOrderID == so.ID && a.Active).ToList().Count();
                //}
                //else
                //{
                //    ConstructShoppingBagCookie();

                //    count = context.lnksalesorders.Where(a => a.SalesOrderID == null && a.CookieID == cookieID && a.Active).ToList().Count();
                //}
                count = GetPendingSOItems(context, null).Count();
            }

            return count;
        }
        public void ConstructShoppingBagCookie()
        {
            shoppingBagCookie = Request.Cookies[Util.GetShoppingBagCookieName()];

            if (shoppingBagCookie == null)
            {
                shoppingBagCookie = new HttpCookie(Util.GetShoppingBagCookieName());
                shoppingBagCookie.Value = System.Guid.NewGuid().ToString();

                Response.Cookies.Add(shoppingBagCookie);
            }

            cookieID = shoppingBagCookie.Value;
        }
        private void SetShoppingBagCookie()
        {
            var shoppingBagCookie = Request.Cookies[Util.GetShoppingBagCookieName()];
            if (shoppingBagCookie != null)
                cookieID = shoppingBagCookie.Value;
        }

        public virtual JsonResult VoucherDiscount(string voucherCode, string total)
        {
            var voucher = new tblvoucher();

            using (var context = new TTTEntities())
            {
                var vouchers = context.tblvouchers.Where(a => a.Code == voucherCode).ToList();

                if (vouchers.Count == 0)
                {
                    voucher = null;
                }
                else if (vouchers.Count > 0)
                {
                    voucher = vouchers[0];
                }
            }

            return Json(new { });
        }
    }
}
