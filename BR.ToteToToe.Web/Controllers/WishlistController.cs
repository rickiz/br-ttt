using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Helpers;
using BR.ToteToToe.Web.ViewModels;

namespace BR.ToteToToe.Web.Controllers
{
    public partial class WishlistController : TTTBaseController
    {
        //
        // GET: /Wishlist/

        private HttpCookie shoppingBagCookie;
        private string cookieID;

        [Authorize]
        public virtual ActionResult Index()
        {
            var viewModel = ConstructWishistIndexViewModel();

            viewModel.IsShareWishlist = false;
            viewModel.ShareUrl = string.Format(@"http://www.tote-to-toe.com/Wishlist/MyWishlist/{0}", viewModel.WishlistID);
            //viewModel.ShareUrl = Url.Action("Index", "Wishlist", new { id = viewModel.WishlistID }, this.Request.Url.Scheme);

            return View(viewModel);
        }

        private WishlistIndexViewModel ConstructWishistIndexViewModel(int id = 0)
        {
            var viewModel = new WishlistIndexViewModel()
            {
                WishlistID = id
            };

            using (var context = new TTTEntities())
            {
                GetNormalModel(viewModel, context);

                GetCustomizeModel(viewModel, context);
            }

            return viewModel;
        }
        private void GetNormalModel(WishlistIndexViewModel viewModel, TTTEntities context)
        {
            var query = from wishlistItem in context.lnkwishlists
                        join wishlist in context.trnwishlists on wishlistItem.WishlistID equals wishlist.ID
                        join modelSize in context.lnkmodelsizes on wishlistItem.ModelSizeID equals modelSize.ID
                        join model in context.refmodels on modelSize.ModelID equals model.ID
                        join brand in context.refbrands on model.BrandID equals brand.ID
                        join modelColourDesc in context.lnkmodelcolourdescs on
                            new { ColourDescID = modelSize.ColourDescID, ModelID = modelSize.ModelID } equals
                            new { ColourDescID = modelColourDesc.ColourDescID, ModelID = modelColourDesc.ModelID }
                        join colourDesc in context.refcolourdescs on modelColourDesc.ColourDescID equals colourDesc.ID
                        //join modelImages in context.lnkmodelimages on modelColourDesc.ID equals modelImages.ModelColourDescID
                        join category in context.refcategories on brand.CategoryID equals category.ID
                        select new
                        {
                            WishlistID = wishlist.ID,
                            Email = wishlist.Email,
                            WishlistItemID = wishlistItem.ID,
                            ModelName = model.Name,
                            Description = model.Description,
                            CategoryName = category.Name,
                            BrandName = brand.Name,
                            ColourName = colourDesc.Name,
                            Size = modelSize.Size,
                            ModelImage = modelColourDesc.MainImage,
                            Active = wishlistItem.Active,
                            ModelID = model.ID,
                            ColourDescID = colourDesc.ID,
                            ModelSizeID = modelSize.ID,
                            SKU = modelColourDesc.SKU
                        };

            if (viewModel.WishlistID > 0)
                query = query.Where(a => a.WishlistID == viewModel.WishlistID && a.Active);
            else
            {
                var customerEmail = Util.SessionAccess.Email;
                query = query.Where(a => a.Email == customerEmail && a.Active);
            }

            var results = query.Distinct().ToList();

            if (viewModel.WishlistID == 0)
                viewModel.WishlistID = results.Select(a => a.WishlistID).Distinct().SingleOrDefault();

            foreach (var item in results)
            {
                viewModel.WishlistItems.Add(new WishlistItem()
                {
                    BrandName = item.BrandName,
                    ColourName = item.ColourName,
                    Description = item.Description,
                    ModelName = item.ModelName,
                    Size = item.Size,
                    Image = string.Format("~/Images/{0}/{1}", item.CategoryName.Replace(" ",""), item.ModelImage),
                    WishlistItemID = item.WishlistItemID,
                    ModelID = item.ModelID,
                    ColourDescID = item.ColourDescID,
                    ModelSizeID = item.ModelSizeID,
                    SKU = item.SKU,
                    DetailsUrl = item.ModelSizeID > 0 ?
                                Url.Action("Details", "Shoes", new { modelID = item.ModelID, colourDescID = item.ColourDescID, modelSizeID = item.ModelSizeID }) :
                                Url.Action("Details", "Shoes", new { modelID = item.ModelID, colourDescID = item.ColourDescID })
                });
            }
        }
        private void GetCustomizeModel(WishlistIndexViewModel viewModel, TTTEntities context)
        {
            var query = from wishlistItem in context.lnkwishlists
                        join wishlist in context.trnwishlists on wishlistItem.WishlistID equals wishlist.ID
                        join customizeModelImage in context.lnkcustomizemodelimages on wishlistItem.SKU equals customizeModelImage.SKU
                        join customizeModel in context.refcustomizemodels on customizeModelImage.CustomizeModelID equals customizeModel.ID
                        select new
                        {
                            WishlistID = wishlist.ID,
                            Email = wishlist.Email,
                            WishlistItemID = wishlistItem.ID,
                            ModelName = customizeModel.Name,
                            Description = customizeModel.Description,
                            CategoryName = "",
                            BrandName = "",
                            ColourName = customizeModelImage.Colour,
                            Size = wishlistItem.Size,
                            ModelImage = customizeModel.MainImage,
                            Active = wishlistItem.Active,
                            SKU = customizeModelImage.SKU
                        };


            if (viewModel.WishlistID > 0)
                query = query.Where(a => a.WishlistID == viewModel.WishlistID && a.Active);
            else
            {
                var customerEmail = Util.SessionAccess.Email;
                query = query.Where(a => a.Email == customerEmail && a.Active);
            }

            var results = query.Distinct().ToList();

            if (viewModel.WishlistID == 0)
                viewModel.WishlistID = results.Select(a => a.WishlistID).Distinct().SingleOrDefault();

            foreach (var item in results)
            {
                viewModel.WishlistItems.Add(new WishlistItem()
                {
                    BrandName = item.BrandName,
                    ColourName = item.ColourName,
                    Description = item.Description,
                    ModelName = item.ModelName,
                    Size = item.Size,
                    Image = string.Format("~/Images/Customize/ShoeStyle/{0}", item.ModelImage),
                    WishlistItemID = item.WishlistItemID,
                    SKU = item.SKU,
                    DetailsUrl = Url.Action("ShoeDetails", "Customize", new { sku = item.SKU })
                });
            }
        }

        [Authorize]
        public virtual ActionResult Remove(int wishlistItemID)
        {
            using (var context = new TTTEntities())
            {
                var wishlistItem = context.lnkwishlists.Where(a => a.ID == wishlistItemID).Single();

                wishlistItem.Active = false;
                wishlistItem.UpdateDT = DateTime.Now;

                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public int PendingCheckoutCount()
        {
            var count = 0;

            if (Util.SessionAccess != null)
            {
                using (var context = new TTTEntities())
                {
                    var customerEmail = Util.SessionAccess.Email;
                    var wishlist = context.trnwishlists.Where(a => a.Email == customerEmail && a.Active).SingleOrDefault();

                    if (wishlist != null)
                        count = context.lnkwishlists.Where(a => a.WishlistID == wishlist.ID && a.Active).ToList().Count();
                }
            }

            return count;
        }

        public virtual ActionResult MyWishlist(int id)
        {
            var viewModel = ConstructWishistIndexViewModel(id);

            viewModel.IsShareWishlist = true;

            return View(viewModel);
        }

        public virtual ActionResult AddToCartNormal(int modelSizeID, string sku)
        {
            try
            {
                ConstructShoppingBagCookie();

                #region Add to Cart

                var salesOrder = new trnsalesorder();

                using (var context = new TTTEntities())
                {
                    var query = from modelSize in context.lnkmodelsizes
                                join model in context.refmodels on modelSize.ModelID equals model.ID
                                select new { ModelSizeID = modelSize.ID, Model = model };

                    var modelDetails = query.Where(a => a.ModelSizeID == modelSizeID).Single();
                    var modelPrice = modelDetails.Model.DiscountPrice.HasValue ? modelDetails.Model.DiscountPrice.Value : modelDetails.Model.Price;

                    var customerEmail = Util.SessionAccess == null ? "" : Util.SessionAccess.Email; ;

                    if (!string.IsNullOrEmpty(customerEmail)) // login customer
                    {
                        var openStatusID = Util.GetStatusID(Status.Open.ToString());

                        salesOrder = context.trnsalesorders.Where(a => a.Email == customerEmail && a.StatusID == openStatusID).SingleOrDefault();

                        using (var trans = new TransactionScope())
                        {
                            if (salesOrder == null)
                            {
                                salesOrder = context.trnsalesorders.Add(new trnsalesorder()
                                {
                                    StatusID = openStatusID,
                                    Email = customerEmail,
                                    SubTotal = modelPrice,
                                    GrandTotal = modelPrice,
                                    CreateDT = DateTime.Now
                                });
                                context.SaveChanges();

                                AddNormalSalesOrderItem(salesOrder.ID, modelSizeID, sku, modelPrice, context);
                            }
                            else
                            {
                                // update sales order grand total
                                salesOrder.GrandTotal += modelPrice;

                                var soItem = context.lnksalesorders.Where(a => a.SalesOrderID == salesOrder.ID &&
                                                                                a.ModelSizeID == modelSizeID).FirstOrDefault();
                                if (soItem == null)
                                {
                                    salesOrder.UpdateDT = DateTime.Now;

                                    AddNormalSalesOrderItem(salesOrder.ID, modelSizeID, sku, modelPrice, context);
                                }
                                else
                                {
                                    soItem.Active = true;
                                    soItem.UpdateDT = DateTime.Now;
                                }
                            }

                            context.SaveChanges();
                            trans.Complete();
                        }
                    }
                    else
                    {
                        ConstructShoppingBagCookie();

                        var soItem = context.lnksalesorders.Where(a => a.SalesOrderID == null &&
                                                                        a.ModelSizeID == modelSizeID &&
                                                                        a.CookieID == cookieID).FirstOrDefault();
                        if (soItem == null)
                        {
                            AddNormalSalesOrderItem(null, modelSizeID, sku, modelPrice, context);
                        }
                        else
                        {
                            soItem.Active = true;
                            soItem.UpdateDT = DateTime.Now;
                        }

                        context.SaveChanges();
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw;
            }

            return RedirectToAction("Index");
        }
        private void AddNormalSalesOrderItem(int? salesOrderID, int modelSizeID, string sku, decimal unitPrice, TTTEntities context)
        {
            context.lnksalesorders.Add(new lnksalesorder()
            {
                CreateDT = DateTime.Now,
                Active = true,
                ModelSizeID = modelSizeID,
                Quantity = 1,
                SalesOrderID = salesOrderID,
                SKU = sku,
                UnitPrice = unitPrice,
                CookieID = salesOrderID.HasValue ? null : cookieID
            });
        }

        public virtual ActionResult AddToCartCustomize(string size, string sku)
        {
            try
            {
                #region Add to Cart

                var salesOrder = new trnsalesorder();

                using (var context = new TTTEntities())
                {
                    var query = from customizeModelImage in context.lnkcustomizemodelimages
                                join customizeModel in context.refcustomizemodels on customizeModelImage.CustomizeModelID equals customizeModel.ID
                                select new { CustomizeModel = customizeModel, CustomizeModelImage = customizeModelImage };

                    var customizeModelDetails = query.Where(a => a.CustomizeModelImage.SKU == sku).Single();

                    var customerEmail = Util.SessionAccess == null ? "" : Util.SessionAccess.Email;


                    if (!string.IsNullOrEmpty(customerEmail)) // login customer
                    {
                        var openStatusID = Util.GetStatusID(Status.Open.ToString());

                        salesOrder = context.trnsalesorders.Where(a => a.Email == customerEmail && a.StatusID == openStatusID).SingleOrDefault();

                        using (var trans = new TransactionScope())
                        {
                            if (salesOrder == null)
                            {
                                salesOrder = context.trnsalesorders.Add(new trnsalesorder()
                                {
                                    StatusID = openStatusID,
                                    Email = customerEmail,
                                    SubTotal = customizeModelDetails.CustomizeModel.Price,
                                    GrandTotal = customizeModelDetails.CustomizeModel.Price,
                                    CreateDT = DateTime.Now
                                });
                                context.SaveChanges();

                                AddCustomizeSalesOrderItem(salesOrder.ID, sku, size, customizeModelDetails.CustomizeModel.Price, context);
                            }
                            else
                            {
                                var soItem = context.lnksalesorders.Where(a => a.SalesOrderID == salesOrder.ID &&
                                                                                a.Size == size &&
                                                                                a.SKU == sku).FirstOrDefault();
                                if (soItem == null)
                                {
                                    salesOrder.GrandTotal += customizeModelDetails.CustomizeModel.Price;
                                    salesOrder.UpdateDT = DateTime.Now;

                                    AddCustomizeSalesOrderItem(salesOrder.ID, sku, size, customizeModelDetails.CustomizeModel.Price, context);
                                }
                                else
                                {
                                    soItem.Active = true;
                                    soItem.UpdateDT = DateTime.Now;
                                }
                            }

                            context.SaveChanges();
                            trans.Complete();
                        }
                    }
                    else // Anonymous user
                    {
                        ConstructShoppingBagCookie();

                        var soItem = context.lnksalesorders.Where(a => a.SalesOrderID == null &&
                                                                        a.Size == size && a.SKU == sku &&
                                                                        a.CookieID == cookieID).FirstOrDefault();
                        if (soItem == null)
                        {
                            AddCustomizeSalesOrderItem(null, sku, size, customizeModelDetails.CustomizeModel.Price, context);
                        }
                        else
                        {
                            soItem.Active = true;
                            soItem.UpdateDT = DateTime.Now;
                        }

                        context.SaveChanges();
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
        private void AddCustomizeSalesOrderItem(int? salesOrderID, string sku, string size, decimal unitPrice, TTTEntities context)
        {
            context.lnksalesorders.Add(new lnksalesorder()
            {
                CreateDT = DateTime.Now,
                Active = true,
                SKU = sku,
                Size = size,
                Quantity = 1,
                SalesOrderID = salesOrderID,
                CookieID = salesOrderID.HasValue ? null : cookieID,
                UnitPrice = unitPrice
            });
        }

        public HttpCookie ConstructShoppingBagCookie()
        {
            shoppingBagCookie = Request.Cookies[Util.GetShoppingBagCookieName()];

            if (shoppingBagCookie == null)
            {
                shoppingBagCookie = new HttpCookie(Util.GetShoppingBagCookieName());
                shoppingBagCookie.Value = System.Guid.NewGuid().ToString();

                Response.Cookies.Add(shoppingBagCookie);
            }

            cookieID = shoppingBagCookie.Value;

            return shoppingBagCookie;
        }
    }
}
