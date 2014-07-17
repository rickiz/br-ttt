using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Helpers;
using BR.ToteToToe.Web.ViewModels;

namespace BR.ToteToToe.Web.Controllers
{
    public partial class CustomizeController : TTTBaseController
    {
        //
        // GET: /Customize/

        private HttpCookie shoppingBagCookie;
        private string cookieID;

        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult ShoeStyle()
        {
            var viewModel =new CustomizeViewModels();
            var results = new List<refcustomizemodel>();

            using (var context = new TTTEntities())
            {
                results = context.refcustomizemodels.ToList();
            }

            var styles = results.Select(a => a.Style).Distinct().ToList();
            foreach (var style in styles)
            {
                var customizeModels = new CustomizeModels()
                {
                     Style = style,
                     Items = results.Where(a => a.Style == style).ToList()
                };

                viewModel.CustomizeModelsList.Add(customizeModels);
            }

            return View(viewModel);
        }

        public virtual ActionResult ShoeColour(int id)
        {
            var viewModel = CnstructShoeColourViewModel(id);

            return View(viewModel);
        }
        private ShoeColourViewModel CnstructShoeColourViewModel(int customizeModelID, int heelHeight = 0)
        {
            var viewModel = new ShoeColourViewModel();

            using (var context = new TTTEntities())
            {
                var shoeStyle = context.refcustomizemodels.Where(a => a.ID == customizeModelID).Single();
                viewModel.Name = shoeStyle.Name;
                viewModel.Style = shoeStyle.Style;
                viewModel.Type = shoeStyle.Type;
                viewModel.Price = shoeStyle.Price;
                viewModel.HeelHeight = 3;
                viewModel.CustomizeModelID = shoeStyle.ID;

                viewModel.Items = context.lnkcustomizemodelimages
                                        .Where(a => a.CustomizeModelID == customizeModelID && (a.HeelHeight==heelHeight || heelHeight == 0))
                                        .OrderBy(a => a.Priority).ToList();
            }

            return viewModel;
        }

        public virtual ActionResult ShoeDetails(string sku)
        {
            var viewModel = new ShoeDetailsViewModel();

            using (var context = new TTTEntities())
            {
                viewModel.ModelImage = context.lnkcustomizemodelimages.Where(a => a.SKU == sku).Single();
                viewModel.CustomizeModel = context.refcustomizemodels.Where(a => a.ID == viewModel.ModelImage.CustomizeModelID).Single();
            }

            viewModel.ShareUrl = string.Format("http://www.tote-to-toe.com/Customize/ShoeDetails?sku={0}", sku);
                

            string imgName = string.Format("{0}_{1}_{2}_{3}_{4}.jpg",
                                            viewModel.CustomizeModel.Style,
                                            viewModel.CustomizeModel.Type,
                                            viewModel.ModelImage.HeelHeight,
                                            viewModel.ModelImage.Colour,
                                            viewModel.ModelImage.Priority);
            ConstructRecentViewCookie(string.Format("Customize/ShoeColour/{0}", imgName),
                                        string.Format("/Customize/ShoeDetails?sku={0}", sku));

            return View(viewModel);
        }

        public void ConstructRecentViewCookie(string imageUrl, string linkUrl)
        {
            try
            {
                var recentViewCookie = GetRecentViewCookie();
                var cookieID = recentViewCookie.Value;

                using (var context = new TTTEntities())
                {
                    if (string.IsNullOrEmpty(cookieID)) // create new cookie
                    {
                        var guid = System.Guid.NewGuid().ToString();

                        context.tblrecentviews.Add(new tblrecentview()
                        {
                            Active = true,
                            CookieID = guid,
                            CreateDT = DateTime.Now,
                            UpdateDT = DateTime.Now,
                            ImageUrl = imageUrl,
                            LinkUrl = linkUrl
                        });

                        recentViewCookie.Value = guid;
                        Response.Cookies.Add(recentViewCookie);
                    }
                    else //cookie create early
                    {
                        // check if item already exist, if yes then update
                        var existingItem = context.tblrecentviews.Where(a => a.CookieID == cookieID &&
                                                                            a.ImageUrl == imageUrl &&
                                                                            a.LinkUrl == linkUrl).SingleOrDefault();
                        if (existingItem != null)
                        {
                            existingItem.UpdateDT = DateTime.Now;
                        }
                        else
                        {
                            var recentItems = context.tblrecentviews.Where(a => a.CookieID == cookieID).ToList();
                            if (recentItems.Count < 5)
                            {
                                context.tblrecentviews.Add(new tblrecentview()
                                {
                                    Active = true,
                                    CookieID = cookieID,
                                    CreateDT = DateTime.Now,
                                    UpdateDT = DateTime.Now,
                                    ImageUrl = imageUrl,
                                    LinkUrl = linkUrl
                                });
                            }
                            else if (recentItems.Count == 5)
                            {
                                var itemToUpdate = context.tblrecentviews.Where(a => a.CookieID == cookieID)
                                                                        .OrderBy(a => a.UpdateDT)
                                                                        .First();

                                itemToUpdate.ImageUrl = imageUrl;
                                itemToUpdate.LinkUrl = linkUrl;
                                itemToUpdate.UpdateDT = DateTime.Now;
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private HttpCookie GetRecentViewCookie()
        {
            var recentViewCookie = Request.Cookies[Util.GetRecentViewCookieName()];
            recentViewCookie = recentViewCookie == null ? new HttpCookie(Util.GetRecentViewCookieName()) : recentViewCookie;

            return recentViewCookie;
        }

        [HttpPost]
        public virtual ActionResult ShoeDetails(ShoeDetailsViewModel viewModel, FormCollection collection)
        {
            try
            {
                if (collection["submit"].ToString() == "+ ADD TO BAG")
                {
                    #region Add to Cart

                    var salesOrder = new trnsalesorder();

                    using (var context = new TTTEntities())
                    {
                        var query = from customizeModelImage in context.lnkcustomizemodelimages
                                    join customizeModel in context.refcustomizemodels on customizeModelImage.CustomizeModelID equals customizeModel.ID
                                    select new { CustomizeModel = customizeModel, CustomizeModelImage = customizeModelImage };

                        var customizeModelDetails = query.Where(a => a.CustomizeModelImage.SKU == viewModel.ModelImage.SKU).Single();

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

                                    AddSalesOrderItem(salesOrder.ID, viewModel.ModelImage.SKU, viewModel.Size, viewModel.CustomizeModel.Price, context);
                                }
                                else
                                {
                                    var soItem = context.lnksalesorders.Where(a => a.SalesOrderID == salesOrder.ID &&
                                                                                    a.Size == viewModel.Size &&
                                                                                    a.SKU == viewModel.ModelImage.SKU).SingleOrDefault();
                                    if (soItem == null)
                                    {
                                        salesOrder.GrandTotal += customizeModelDetails.CustomizeModel.Price;
                                        salesOrder.UpdateDT = DateTime.Now;

                                        AddSalesOrderItem(salesOrder.ID, viewModel.ModelImage.SKU, viewModel.Size, viewModel.CustomizeModel.Price, context);
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
                                                                            a.Size == viewModel.Size && a.SKU == viewModel.ModelImage.SKU &&
                                                                            a.CookieID == cookieID).SingleOrDefault();
                            if (soItem == null)
                            {
                                AddSalesOrderItem(null, viewModel.ModelImage.SKU, viewModel.Size, viewModel.CustomizeModel.Price, context);
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
                else if (collection["submit"].ToString() == "+ ADD TO WISH LIST")
                {
                    #region Add to Wishlist

                    if (!Request.IsAuthenticated)
                    {
                        return RedirectToAction(MVC.SignIn.Index(Url.Action("ShoeDetails", new { sku = viewModel.ModelImage.SKU })));
                    }

                    AddToWishList(viewModel);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return RedirectToAction("ShoeDetails", new { sku = viewModel.ModelImage.SKU });
        }
        private void AddSalesOrderItem(int? salesOrderID, string sku, string size, decimal unitPrice, TTTEntities context)
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

        private void AddToWishList(ShoeDetailsViewModel viewModel)
        {
            var wishlistID = 0;

            using (var context = new TTTEntities())
            {
                using (var trans = new TransactionScope())
                {
                    var customerEmail = Util.SessionAccess.Email;
                    var wishlist = context.trnwishlists.Where(a => a.Email == customerEmail).SingleOrDefault();
                    if (wishlist == null)
                    {
                        wishlistID = context.trnwishlists.Add(new trnwishlist()
                        {
                            Active = true,
                            CreateDT = DateTime.Now,
                            Email = customerEmail
                        }).ID;

                        AddWishlistItem(wishlistID, viewModel.ModelImage.SKU, viewModel.Size, context);
                    }
                    else
                    {
                        wishlistID = wishlist.ID;

                        var wishlistItem = context.lnkwishlists.Where(a => a.SKU == viewModel.ModelImage.SKU &&
                                                                            a.Size == viewModel.Size &&
                                                                            a.WishlistID == wishlistID).SingleOrDefault();

                        if (wishlistItem == null)
                            AddWishlistItem(wishlistID, viewModel.ModelImage.SKU, viewModel.Size, context);
                        else
                        {
                            wishlistItem.Active = true;
                            wishlistItem.UpdateDT = DateTime.Now;
                        }
                    }

                    context.SaveChanges();
                    trans.Complete();
                }
            }
        }
        private void AddWishlistItem(int wishlistID, string sku, string size, TTTEntities context)
        {
            context.lnkwishlists.Add(new lnkwishlist()
            {
                WishlistID = wishlistID,
                SKU = sku,
                Size = size,
                Active = true,
                CreateDT = DateTime.Now,
            });
        }

        [HttpPost]
        public virtual JsonResult FilterHeelHeight(int id, int heelHeight)
        {
            var viewModel = CnstructShoeColourViewModel(id, heelHeight);

            var stringView = RenderRazorViewToString("_ShoeColour", viewModel);
            return Json(stringView, JsonRequestBehavior.AllowGet);
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
