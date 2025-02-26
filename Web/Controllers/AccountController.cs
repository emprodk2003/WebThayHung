using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Web.Models;


namespace Web.Controllers
{
    public class AccountController : Controller
    {
        //userManager will hold the UserManager instance
        private readonly UserManager<User> userManager;
        //signInManager will hold the SignInManager instance
        private readonly SignInManager<User> signInManager;
        //Both UserManager and SignInManager services are injected into the AccountController
        //using constructor injection
        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        /// <summary>
        /// //////////////////////////////// Register //////////////////////////
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Copy data from RegisterViewModel to IdentityUser
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, model.Password);
                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }
                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    Console.WriteLine($"Error: {error.Code} - {error.Description}");
                }
            }
            return View(model);
        }

        /// <summary>
        /// /////////////////////////////////////// Login //////////////////////////////////
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? ReturnUrl = null)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = ReturnUrl,

                //The GetExternalAuthenticationSchemesAsync() method of the SignInManager class is used to retrieve
                //a list of all external authentication schemes that have been configured in the application.
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }

                    // Handle successful login
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                if (result.RequiresTwoFactor)
                {
                    // Handle two-factor authentication case
                }
                if (result.IsLockedOut)
                {
                    // Handle lockout scenario
                }
                else
                {
                    model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                    // Handle failure
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }

        /// <summary>
        /// /////////////////////////////////////// Logout ////////////////////////////////////
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        /// <summary>
        /// /////////////////////////////////////// Kiem tra email dang ki ////////////////////////////////////
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> IsEmailAvailable(string Email)
        {
            //Check If the Email Id is Already in the Database
            var user = await userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {Email} is already in use.");
            }
        }

        /// <summary>
        ////////////////////////////////////////// Login Google /////////////////////////////////////////////////
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Generate a URL for the "ExternalLoginCallback" action method in the "Account" controller.
            // This URL includes the returnUrl as a route parameter, which will be used to redirect the user
            // back to the original page they were trying to access after a successful external login.
            var redirectUrl = Url.Action(
                action: "ExternalLoginCallback", // The name of the callback action method.
                controller: "Account",           // The name of the controller containing the callback method.
                values: new { ReturnUrl = returnUrl } // Pass the returnUrl as a parameter to the callback method.
            );

            // Configure authentication properties for the external login.
            // The "ConfigureExternalAuthenticationProperties" method sets up parameters needed for the external provider,
            // such as the login provider name (e.g., Google, Facebook) and the redirect URL to be used after login.
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            // Redirect the user to the external provider's login page (e.g., Google or Facebook).
            // The "ChallengeResult" triggers the external authentication process, which redirects the user
            // to the external provider's login page using the configured properties.
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl, string? remoteError)
        {
            // If no returnUrl is provided, default to the application's home page.
            returnUrl = returnUrl ?? Url.Content("~/");

            // Check if an error occurred during the external authentication process.
            // If so, display an alert to the user and close the popup window.
            if (remoteError != null)
            {
                return Content($"<script>alert('Error from external provider: {remoteError}'); window.close();</script>", "text/html");
            }

            // Retrieve login information about the user from the external login provider (e.g., Google, Facebook).
            // This includes details like the provider's name and the user's identifier within that provider.
            var info = await signInManager.GetExternalLoginInfoAsync();

            // If the login information could not be retrieved, display an error message
            // and close the popup window.
            if (info == null)
            {
                return Content($"<script>alert('Error loading external login information.'); window.close();</script>", "text/html");
            }

            // Attempt to sign in the user using their external login details.
            // If a corresponding record exists in the AspNetUserLogins table, the user will be logged in.
            var signInResult = await signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,    // The name of the external login provider (e.g., Google, Facebook).
                info.ProviderKey,      // The unique identifier of the user within the external provider.
                isPersistent: false,   // Indicates whether the login session should persist across browser restarts.
                bypassTwoFactor: true  // Bypass two-factor authentication if enabled.
            );

            // If the external login succeeds, redirect the parent window to the returnUrl
            // and close the popup window.
            if (signInResult.Succeeded)
            {
                returnUrl = Url.Content("~/");
                return Content($"<script>window.opener.location.href = '{returnUrl}'; window.close();</script>", "text/html");
            }

            // If the user does not have a corresponding record in the AspNetUserLogins table,
            // attempt to create a new account using the user's email from the external provider.
            var email = info.Principal.FindFirstValue(ClaimTypes.Email); // Retrieve the user's email from the external login provider.

            if (email != null)
            {
                // Check if a local user account with the retrieved email already exists.
                var user = await userManager.FindByEmailAsync(email);

                // If no local account exists, create a new user in the AspNetUsers table.
                if (user == null)
                {
                    user = new User
                    {
                        UserName = email, // Set the username to the user's email.
                        Email = email,    // Set the email.
                    };

                    // Create the new user in the database.
                    await userManager.CreateAsync(user);
                }

                // Link the external login to the newly created or existing user account.
                // This inserts a record into the AspNetUserLogins table.
                await userManager.AddLoginAsync(user, info);

                // Sign in the user locally after linking their external login.
                await signInManager.SignInAsync(user, isPersistent: false);

                // Redirect the parent window to the returnUrl and close the popup window.
                return Content($"<script>window.opener.location.href = '{returnUrl}'; window.close();</script>", "text/html");
            }

            // If the email claim is not provided by the external login provider,
            // display an error message and close the popup window.
            return Content($"<script>alert('Email claim not received. Please contact support.'); window.close();</script>", "text/html");
        }
    }
}
