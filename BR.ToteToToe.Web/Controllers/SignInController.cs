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
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

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

            var pwd = Util.GetMD5Hash(viewModel.Password.Trim());

            using (var context = new TTTEntities())
            {
                var user = 
                    context.tblaccesses
                        .SingleOrDefault(a => a.Email == viewModel.Email && 
                                              a.Password == pwd && 
                                              a.Active);

                if (user == null)
                {
                    ModelState.AddModelError("LoginForm", "Invalid Email/Password.");
                    return View(viewModel);
                }

                if (!user.ConfirmedEmail)
                {
                    var verifyEmailModel = new VerifyEmailViewModel { User = user, AfterLogin = true };
                    return View(MVC.SignIn.Views.VerifyEmail, verifyEmailModel);
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
                        FirstName = name,
                        EmailToken = Guid.NewGuid().ToString()
                    };

                    context.tblaccesses.Add(user);
                    context.SaveChanges();

                    var verifyEmailModel = new VerifyEmailViewModel { User = user, AfterRegistration = true };
                    return View(MVC.SignIn.Views.VerifyEmail, verifyEmailModel);
                }
                else
                {
                    if (user.ConfirmedEmail)
                    {
                        Util.SessionAccess = user;
                        FormsAuthentication.SetAuthCookie(user.Email, false);
                        LinkToAccount();
                    }
                    else
                    {
                        var verifyEmailModel = new VerifyEmailViewModel { User = user, AfterLogin = true };
                        return View(MVC.SignIn.Views.VerifyEmail, verifyEmailModel);
                    }                    
                }
            }

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
                    user = new tblaccess
                    {
                        Active = true,
                        CreateDT = DateTime.Now,
                        Email = model.Email,
                        Password = Util.GetMD5Hash(model.Password),
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        EmailToken = Guid.NewGuid().ToString()
                    };

                    context.tblaccesses.Add(user);
                    context.SaveChanges();

                    var verifyEmailModel = new VerifyEmailViewModel { User = user, AfterRegistration = true };
                    return View(MVC.SignIn.Views.VerifyEmail, verifyEmailModel);
                }
                else
                {
                    ModelState.Clear();
                    ModelState.AddModelError("RegisterForm", "Email already exists. Please enter a different Email.");
                    return View(Views.Index, viewModel);
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult VerifyEmail(VerifyEmailViewModel viewModel)
        {
            using (var context = new TTTEntities())
            {
                var user = context.tblaccesses.Where(a => a.ID == viewModel.User.ID).Single();

                SendEmailVerification(user);

                ViewBag.Email = user.Email;
            }

            return View(MVC.SignIn.Views.VerifyEmailSent);
        }

        private void SendEmailVerification(tblaccess user)
        {
            var viewModel = new SignInConfirmEmailViewModel
            {
                User = user,
            };

            var body = this.RenderViewToString(Views.ViewNames.ConfirmEmail, viewModel);

            var message = new MailMessage
            {
                Subject = "Tote To Toe Email Verification",
                IsBodyHtml = true,
                Body = body
            };
            message.To.Add(user.Email);

            //ServicePointManager.ServerCertificateValidationCallback =
            //    delegate(object s, X509Certificate certificate,
            //             X509Chain chain, SslPolicyErrors sslPolicyErrors)
            //    { return true; };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Send(message);
            }              
        }
        private void SendWelcomeEmail(tblaccess user)
        {
            var viewModel = new SignInConfirmEmailViewModel
            {
                User = user,
            };

            var body = this.RenderViewToString(Views.ViewNames.WelcomeEmail, viewModel);
            var message = new MailMessage
            {
                Subject = "Tote To Toe Welcome Email",
                IsBodyHtml = true,
                Body = body
            };
            message.To.Add(user.Email);

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Send(message);
            }
        }
        private void SendForgotPasswordEmail(ForgotPasswordViewModel viewModel)
        {
            var body = this.RenderViewToString(Views.ViewNames.ResetPassword, viewModel);
            var message = new MailMessage
            {
                Subject = "Tote To Toe Reset Password",
                IsBodyHtml = true,
                Body = body
            };
            message.To.Add(viewModel.Email);

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Send(message);
            }
        }

        [AllowAnonymous]
        public virtual ActionResult ConfirmEmail(string token, string email)
        {
            tblaccess user;

            using (var context = new TTTEntities())
            {
                user = 
                    context.tblaccesses
                        .Where(a => a.Email == email && a.Active && a.EmailToken == token)
                        .SingleOrDefault();

                user.ConfirmedEmail = true;
                user.UpdateDT = DateTime.Now;

                context.SaveChanges();
            }

            SendWelcomeEmail(user);

            Util.SessionAccess = user;
            FormsAuthentication.SetAuthCookie(user.Email, false);
            LinkToAccount();

            return RedirectToAction(MVC.Home.Index());
        }

        [AllowAnonymous]
        public virtual ActionResult WelcomeLogin(string t, string email)
        {
            tblaccess user;

            using (var context = new TTTEntities())
            {
                user =
                    context.tblaccesses
                        .Where(a => a.Email == email && a.Active && a.EmailToken == t && a.ConfirmedEmail)
                        .SingleOrDefault();

                if (user == null)
                    throw new InvalidOperationException();

                user.EmailToken = null;
                user.UpdateDT = DateTime.Now;

                context.SaveChanges();
            }

            Util.SessionAccess = user;
            FormsAuthentication.SetAuthCookie(user.Email, false);
            LinkToAccount();

            return RedirectToAction(MVC.Home.Index());
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

        public virtual ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        public virtual ActionResult ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            if(string.IsNullOrEmpty(viewModel.Email))
                return View(new ForgotPasswordViewModel());

            using (var context = new TTTEntities())
            {
                var user = context.tblaccesses.Where(a => a.Email == viewModel.Email).SingleOrDefault();

                if (user == null)
                {
                    viewModel.ErrorMessage = "Email account does not exist.";
                    return View(viewModel);
                }

                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                viewModel.NewPassword = new string(
                    Enumerable.Repeat(chars, 8)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray());

                user.Password = Util.GetMD5Hash(viewModel.NewPassword);
                user.UpdateDT = DateTime.Now;

                context.SaveChanges();
            }

            SendForgotPasswordEmail(viewModel);

            return View("ForgotPasswordSuccess", viewModel);
        }

        public virtual ActionResult ForgotPasswordSuccess()
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
