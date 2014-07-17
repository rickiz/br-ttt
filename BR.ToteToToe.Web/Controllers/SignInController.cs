﻿using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Helpers;
using BR.ToteToToe.Web.ViewModels;
using DotNetOpenAuth.AspNet;
using Facebook;
using Microsoft.Web.WebPages.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using System.Transactions;

namespace BR.ToteToToe.Web.Controllers
{
    public partial class SignInController : TTTBaseController
    {
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        private string cookieID = "";

        [AllowAnonymous]
        public virtual ActionResult Index(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                if (this.Request.UrlReferrer != null)
                {
                    var previousUrl = this.Request.UrlReferrer.ToString();
                    if (previousUrl.Contains("ShoppingBag"))
                    {
                        returnUrl = Url.Action("Index", "ShoppingBag");
                    }
                }
            }

            var viewModel = new SignInViewModel
            {
                ReturnUrl = returnUrl,
                RegisterViewModel = new SignInRegisterViewModel
                {
                    ReturnUrl = returnUrl
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Index(SignInViewModel viewModel, string returnUrl)
        {
            if (viewModel.RegisterViewModel == null)
                viewModel.RegisterViewModel = new SignInRegisterViewModel { ReturnUrl = returnUrl };
            viewModel.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return View(viewModel);

            var pwd = Util.GetMD5Hash(viewModel.Password);

            using (var context = new TTTEntities())
            {
                var user = context.tblaccesses.SingleOrDefault(a => a.Email == viewModel.Email && a.Password == pwd && a.Active);

                if (user == null)
                {
                    ModelState.AddModelError("LoginForm", "Invalid Email/Password.");
                    return View(viewModel);
                }

                Util.SessionAccess = user;
                FormsAuthentication.SetAuthCookie(viewModel.Email, false);
            }

            LinkToAccount();

            return RedirectToLocal(viewModel.ReturnUrl);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult FacebookLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("FacebookLoginCallback", new { returnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public virtual ActionResult FacebookLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("FacebookLoginCallback", new { returnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("FacebookLoginFailure");
            }

            var email = result.UserName;
            var accessToken = result.ExtraData["accesstoken"];
            var fbID = result.ExtraData["id"];
            var name = result.ExtraData["name"];
            //var gender = result.ExtraData.ContainsKey("gender") ? result.ExtraData["gender"] : "";
            //Session["FB_AccessToken"] = accessToken;
            tblaccess user = null;

            using (var context = new TTTEntities())
            {
                user = context.tblaccesses.Where(a => a.FacebookID == fbID && a.Active).SingleOrDefault();

                if (user == null)
                {
                    user = new tblaccess
                    {
                        Active = true,
                        CreateDT = DateTime.Now,
                        Email = email,
                        FacebookAccessToken = accessToken,
                        FacebookID = fbID,
                        Password = "",
                        FirstName = name
                    };

                    context.tblaccesses.Add(user);
                    context.SaveChanges();
                }
            }

            Util.SessionAccess = user;
            FormsAuthentication.SetAuthCookie(user.Email, false);

            LinkToAccount();

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Register(SignInViewModel viewModel)
        {
            var model = viewModel.RegisterViewModel;
            tblaccess user;

            using (var context = new TTTEntities())
            {
                user = context.tblaccesses.Where(a => a.Email == model.Email && a.Active).SingleOrDefault();

                if (user == null)
                {
                    //if (!string.IsNullOrEmpty(model.FacebookID))
                    //{
                    //    user = context.tblaccesses.Where(a => a.FacebookID == model.FacebookID && a.Active).SingleOrDefault();
                    //    if (user != null)
                    //    {
                    //        ModelState.Clear();
                    //        ModelState.AddModelError("RegisterForm", "Facebook account already registered.");
                    //        return View(Views.Index, viewModel);
                    //    }
                    //}

                    user = new tblaccess
                    {
                        Active = true,
                        CreateDT = DateTime.Now,
                        Email = model.Email,
                        //FacebookAccessToken = model.FacebookAccessToken,
                        //FacebookID = model.FacebookID,
                        Password = Util.GetMD5Hash(model.Password),
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        //BirthDateDay = model.BirthDateDay,
                        //BirthDateMonth = model.BirthDateMonth,
                        //BirthDateYear = model.BirthDateYear,
                        //Gender = model.Gender,
                        //Phone = model.Phone
                    };

                    context.tblaccesses.Add(user);
                    context.SaveChanges();

                    // create address if one of the field below has value
                    //if (false ==
                    //    (string.IsNullOrEmpty(model.AddressLine1)
                    //    && string.IsNullOrEmpty(model.AddressLine2)
                    //    && string.IsNullOrEmpty(model.CityTown)
                    //    && string.IsNullOrEmpty(model.Postcode)
                    //    && string.IsNullOrEmpty(model.State)
                    //    && !model.CountryID.HasValue))
                    //{
                    //    var address = new tbladdress
                    //    {
                    //        AccessID = user.ID,
                    //        Active = true,
                    //        AddressLine1 = model.AddressLine1,
                    //        AddressLine2 = model.AddressLine2,
                    //        CityTown = model.CityTown,
                    //        CountryID = model.CountryID,
                    //        CreateDT = DateTime.Now,
                    //        Postcode = model.Postcode,
                    //        State = model.State
                    //    };

                    //    context.tbladdresses.Add(address);
                    //    context.SaveChanges();
                    //}
                }
                else
                {
                    ModelState.Clear();
                    ModelState.AddModelError("RegisterForm", "Email already exists. Please enter a different Email.");
                    return View(Views.Index, viewModel);
                }
            }

            Util.SessionAccess = user;
            FormsAuthentication.SetAuthCookie(model.Email, false);

            LinkToAccount();

            return RedirectToLocal(model.ReturnUrl);
        }

        [Authorize]
        public virtual ActionResult Logout()
        {
            var accessToken = Util.SessionAccess.FacebookAccessToken;
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            if (!string.IsNullOrEmpty(accessToken))
            {
                var next = Url.ActionAbsolute(MVC.Home.Index());
                var fb = new FacebookClient(accessToken);
                var logoutUrl =
                    fb.GetLogoutUrl(
                    new
                    {
                        access_token = accessToken,
                        next = next
                    });

                Response.Redirect(logoutUrl.AbsoluteUri);
            }

            return RedirectToAction(MVC.Home.Index());
        }

        [AllowAnonymous]
        public virtual ActionResult FacebookLoginFailure()
        {
            return View();
        }

        private void LinkToAccount()
        {
            SetShoppingBagCookie();

            var existingSOItems = new List<lnksalesorder>();
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
                    else
                    {
                        existingSOItems = context.lnksalesorders.Where(a => a.Active && a.SalesOrderID == so.ID).ToList();
                    }


                    foreach (var soItem in salesOrderItems)
                    {
                        foreach (var item in existingSOItems)
                        {
                            if (item.SKU == soItem.SKU && item.ModelSizeID == soItem.ModelSizeID)
                                soItem.Active = false;
                            else
                                soItem.SalesOrderID = so.ID;
                        }

                        if (existingSOItems.Count == 0)
                            soItem.SalesOrderID = so.ID;

                        soItem.CookieID = "";
                        soItem.UpdateDT = DateTime.Now;
                    }

                    context.SaveChanges();
                }
                trans.Complete();
            }
        }
        private void SetShoppingBagCookie()
        {
            var shoppingBagCookie = Request.Cookies[Util.GetShoppingBagCookieName()];
            if (shoppingBagCookie != null)
                cookieID = shoppingBagCookie.Value;
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public virtual ActionResult Facebook(string returnUrl)
        //{
        //    var redirectUrl =
        //        Url.Action("FacebookCallback", "SignIn",
        //                   routeValues: new { ReturnUrl = returnUrl },
        //                   protocol: Request.Url.Scheme);
        //    var fb = new FacebookClient();
        //    var loginUrl = fb.GetLoginUrl(new
        //    {
        //        client_id = "267986850024373",
        //        client_secret = "846b5689ed1fbb3a2e9443be881de92c",
        //        redirect_uri = redirectUrl,
        //        response_type = "code",
        //        scope = "email" // Add other permissions as needed
        //    });

        //    return Redirect(loginUrl.AbsoluteUri);
        //}

        //public virtual ActionResult FacebookCallback(string code, string returnUrl)
        //{
        //    var redirectUrl =
        //        Url.Action("FacebookCallback", "SignIn",
        //                   routeValues: new { ReturnUrl = returnUrl },
        //                   protocol: Request.Url.Scheme);
        //    var fb = new FacebookClient();
        //    dynamic result = fb.Post("oauth/access_token", new
        //    {
        //        client_id = "267986850024373",
        //        client_secret = "846b5689ed1fbb3a2e9443be881de92c",
        //        redirect_uri = redirectUrl,
        //        code = code
        //    });

        //    var accessToken = result.access_token;

        //    fb = new FacebookClient(accessToken);
        //    dynamic userInfo = fb.Get("me");

        //    // TODO: Authenticate User
        //    return View();
        //}

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }
        #endregion
    }
}