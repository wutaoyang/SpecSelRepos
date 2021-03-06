﻿using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpecSelRepos.Models;
using SpecSelRepos.Models.AccountViewModels;
using SpecSelRepos.Services;

namespace SpecSelRepos.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        public const string ADMIN = "admin";
        public const string USER = "user";
        public const string EMAILER = "email";// users who receive email after user registers
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        //private ManageRolesViewModel viewModel;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        /// <summary>
        /// Launches the user and role management page for admin users
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = ADMIN)]
        public IActionResult ManageRoles()
        {
            ViewData["Message"] = "User Role Management Page";
            ManageRolesViewModel viewModel = new ManageRolesViewModel(_userManager, _roleManager);
            return View(viewModel);
        }

        /// <summary>
        /// Method adds role to the database if it doesn't exist or removes it if it does exist - cannot remove ADMIN
        /// from: https://forums.asp.net/t/2132158.aspx?add+users+and+roles+in+asp+net+core+2+0+
        /// url to invoke action: https://localhost:44327/account/addrole?role=admin (44327 could be any port)
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [Authorize(Roles = ADMIN)]
        public async Task<IActionResult> AddRemoveRole(string role)
        {
            if (null != role)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));//create role
                }
                else
                {
                    if (role != ADMIN && role != USER && role != EMAILER)// dont delete admin, user or email role
                    {
                        IdentityRole iRole = await _roleManager.FindByNameAsync(role);
                        await _roleManager.DeleteAsync(iRole);//delete role
                    }
                }
            }
            return RedirectToAction("ManageRoles", "Account");
        }

        /// <summary>
        /// Remove a specific role from a user (Admin only)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [Authorize(Roles = ADMIN)]
        public async Task<IActionResult> RemoveUserRole(string email, string role)
        {
            ApplicationUser user = GetUserByEmail(email);
            if (null == user)
            {
                return Json("No user with username: " + email);
            }

            if (await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
                return RedirectToAction("ManageRoles", "Account");
            }
            return Content("User " + email + " does not hold role " + role);
        }

        /// <summary>
        /// Deletes user from the database if they hold no roles
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [Authorize(Roles = ADMIN)]
        public async Task<IActionResult> DeleteUser(string email)
        {
            ApplicationUser user = GetUserByEmail(email);
            if (null == user)
            {
                return Json("No user with username: " + email);
            }
            else
            {
                IList<string> roles = GetUserRoles(user);
                if(roles.Count == 0)
                {
                    await _userManager.DeleteAsync(user);
                    return RedirectToAction("ManageRoles", "Account");
                }
                else
                {
                    return Content("Please remove user from roles before deletion");
                }
            }
        }

        /// <summary>
        /// Assign a role to a user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [Authorize(Roles = ADMIN)]
        public async Task<IActionResult> AssignRole(string email, string role)
        {
            // check for a null value - might not be required after debugging complete
            if (null == email || null == role)
            {
                if (null == email) { email = "null"; }
                if (null == role) { role = "null"; }
                return Content("A null value was passed - email: " + email + ", role: " + role);
            }

            // process request
            ApplicationUser user = GetUserByEmail(email);
            if (null == user)
            {
                return Content("No user with given email:" + email);
            }
            // if the role exists, assign the role to the user
            if (await _roleManager.RoleExistsAsync(role))
            {
                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                    //return Content("Role assigned: " + GetUserRoles(user));

                    return RedirectToAction("ManageRoles", "Account");
                }
                else
                {
                    return Content("User is in role: " + role + ": " + GetUserRolesString(user));
                }
            }
            else
            {
                return Content("Role: " + role + " does not exist");
            }
        }

        private string GetUserRolesString(ApplicationUser user)
        {
            IList<string> list = GetUserRoles(user);
            return ListToString(list);
        }

        private IList<string> GetUserRoles(ApplicationUser user)
        {
            return _userManager.GetRolesAsync(user).Result;
        }

        private string ListToString(IList<string> list)
        {
            string[] array = new string[list.Count];
            list.CopyTo(array, 0);
            return "[" + string.Join(",", array) + "]";
        }

        private ApplicationUser GetUserByEmail(string email)
        {
            return _userManager.FindByEmailAsync(email).Result;
        }

        private IList<ApplicationUser> GetUsersByRole(string role)
        {
            return _userManager.GetUsersInRoleAsync(role).Result;
        }

        /// <summary>
        /// Sets user with email to the administrator if no admin currently exists
        /// Following successful call to this method the user must log out and back in in order to make use of admin priveleges
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<IActionResult> CreateAdmin(string email)
        {
            IList<ApplicationUser> adminUsers = await _userManager.GetUsersInRoleAsync(ADMIN);
            if (adminUsers.Count < 1)
            {
                // create the admin role if it doesnt exist
                if (!await _roleManager.RoleExistsAsync(ADMIN))
                {
                    await _roleManager.CreateAsync(new IdentityRole(ADMIN));//create role
                    await _roleManager.CreateAsync(new IdentityRole(USER));//create role
                    await _roleManager.CreateAsync(new IdentityRole(EMAILER));//create role
                }
                // assign admin role to email user
                await AssignRole(email, ADMIN);
                return Content("User " + email + " is now the first site " + ADMIN + ". If logged in, please log out and back in to activate admin privileges.");
            }
            return Json("An admin exists - Contact admin to modify user privileges.");
        }

        /// <summary>
        /// Checks user is an administrator
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = ADMIN)]
        public bool UserIsAdmin()
        {
            if (User.IsInRole(ADMIN))
            {
                return true;
            }
            return false;
        }
        

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    EmailToAdministrator(model.Email);
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Email 
        /// </summary>
        /// <param name="email"></param>
        private void EmailToAdministrator(string email)
        {
            IList<ApplicationUser> usersToEmail = GetUsersByRole(EMAILER);
            SendGmail(usersToEmail, "User " + email + " has registered to SpecSelRepos. Please assign roles.");
        }

        /// <summary>
        /// Send email to each user in list using specselrepos@gmail account
        /// </summary>
        /// <param name="usersToEmail"></param>
        /// <param name="content"></param>
        private void SendGmail(IList<ApplicationUser> usersToEmail, string content)
        {
            if(usersToEmail.Count > 0)
            {
                MailMessage mail = new MailMessage();
                foreach (ApplicationUser user in usersToEmail)
                {
                    var email = user.Email;
                    mail.To.Add(new MailAddress(email));
                }
                mail.From = new MailAddress("specselrepos@gmail.com", "SpecSelRepos", System.Text.Encoding.UTF8);
                mail.Subject = "SpecSel New User";
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.Body = content;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;
                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("specselrepos@gmail.com", "Pa55word#");
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                try
                {
                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    Exception ex2 = ex;
                    string errorMessage = string.Empty;
                    while (ex2 != null)
                    {
                        errorMessage += ex2.ToString();
                        ex2 = ex2.InnerException;
                    }
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
