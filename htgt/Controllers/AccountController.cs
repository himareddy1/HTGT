using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using htgt.Business;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using htgt.Models;
using HTGT.Data.Models;
using log4net;

namespace htgt.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AccountController));

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var signinManager = new UserSignInManager();
                string browserString = $"Browser: {Request.Browser.Browser}, Version: {Request.Browser.Version}";

                var result = signinManager.ValidateUserCredentials(model.Email, model.Password);

                string logMessage;
                switch (result.Status)
                {
                    case SignInStatus.Success:
                        IdentitySignin(result.UserInfo, isPersistent: model.RememberMe);
                        logMessage = Helpers.FormatLogMessage(TableNameConstants.UsersTableName, model.Email, model.Email, InfoMessages.UserLoginSuccess, browserString);
                        log.Info(logMessage);
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        logMessage = Helpers.FormatLogMessage(TableNameConstants.UsersTableName, model.Email, model.Email, InfoMessages.UserLockedOut, browserString);
                        log.Info(logMessage);
                        ModelState.AddModelError("", "Invalid login attempt - Account Locked out. Please contact your project manager.");
                        return View(model);
                    default:
                        logMessage = Helpers.FormatLogMessage(TableNameConstants.UsersTableName, model.Email, model.Email, InfoMessages.UserLoginFailure, browserString);
                        log.Info(logMessage);
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception occurred!!!", ex);
                ModelState.AddModelError("", "Exception occurred!!!");
                return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(HTGTUsersCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var userRegistrationManager = new UserRegistrationManager();
                var result = userRegistrationManager.RegisterUser(model);
                string logMessage;
                switch (result)
                {
                    case RegistrationStatus.Success:
                        logMessage = Helpers.FormatLogMessage(TableNameConstants.UsersTableName, model.EmailAddress, model.EmailAddress, InfoMessages.UserCreateSuccess);
                        log.Info(logMessage);
                        return RedirectToAction("RegisterSuccess");
                    case RegistrationStatus.InvalidEmailAddress:
                        logMessage = Helpers.FormatLogMessage(TableNameConstants.UsersTableName, model.EmailAddress, model.EmailAddress, InfoMessages.UserExists);
                        log.Info(logMessage);
                        ModelState.AddModelError("", InfoMessages.UserExists);
                        return View(model);
                    default:
                        logMessage = Helpers.FormatLogMessage(TableNameConstants.UsersTableName, model.EmailAddress, model.EmailAddress, InfoMessages.UserCreateFailure);
                        log.Info(logMessage);
                        ModelState.AddModelError("", InfoMessages.UserCreateFailure);
                        return View(model);
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception occurred!!!", ex);
                ModelState.AddModelError("", "Exception occurred!!!");
                return View(model);
            }
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult RegisterSuccess()
        {
            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserSignInManager manager = new UserSignInManager();
                    var result = manager.GeneratePasswordResetToken(model.Email);

                    if (result.Status == PasswordResetStatus.Success)
                    {
                        EmailHelpers eh = new EmailHelpers();
                        string resetcode = result.ResetToken;

                        string username = result.EmailAddress;
                        var callbackUrl = Url.Action("ResetPassword", "Account", new { code = resetcode }, protocol: Request.Url?.Scheme);

                        eh.SendPasswordResetEmail(model.Email, callbackUrl);

                        var logMessage = Helpers.FormatLogMessage(TableNameConstants.UsersTableName, username, model.Email, "Password reset email sent successfully!!!");
                        log.Info(logMessage);

                        return RedirectToAction("ForgotPasswordConfirmation", "Account");
                    }
                    else
                    {
                        log.WarnFormat("Unable to reset password for email: {0}", model.Email);
                        ModelState.AddModelError("",
                            result.Status == PasswordResetStatus.InvalidEmailAddress
                                ? "Invalid email address provided. Please try again."
                                : $"Unable to reset password for email: {model.Email}");
                    }
                }
                catch (Exception ex)
                {
                    string message = "Exception occurred!!!";
                    log.Error(message, ex);
                    ModelState.AddModelError("Exception", message);
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                UserSignInManager manager = new UserSignInManager();
                var result = manager.ResetUserPassword(model);

                if (result.Status == PasswordResetStatus.Success)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else if (result.Status == PasswordResetStatus.InvalidEmailAddress)
                {
                    ModelState.AddModelError("", "Invalid email address provided. Please try again.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.Contains("one non letter or digit character"))
                        {
                            var replaceError = error.Replace("Passwords must have at least one non letter or digit character.", "Passwords must have at least one special character.");
                            ModelState.AddModelError("", replaceError);
                        }
                        else
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Exception occurred!!!";
                log.Error(message, ex);
                ModelState.AddModelError("", message);
                return View(model);
            }
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            IdentitySignout();
            return RedirectToAction("Index", "Home");
        }

        

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private void IdentitySignin(HTGTUsersViewModel user, bool isPersistent = false)
        {
            var claims = new List<Claim>
            {
                // create required claims
                new Claim(ClaimTypes.NameIdentifier, user.EmailAddress),
                new Claim(ClaimTypes.Name, UserSignInManager.FormatName(user.FirstName, user.LastName)),
                // custom – my serialized AppUserState object
                new Claim("HTGTUserInfo", user.ToString())
            };

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignIn(new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddDays(1)
            }, identity);
        }

        private void IdentitySignout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                                            DefaultAuthenticationTypes.ExternalCookie);
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri, string userId = null)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            private string LoginProvider { get; }
            private string RedirectUri { get; }
            private string UserId { get; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}