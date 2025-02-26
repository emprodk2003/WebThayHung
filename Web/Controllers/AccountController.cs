using Application.Email.Service;
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
        private readonly EmailSenderService emailSender;
        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager, EmailSenderService emailSenderService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSenderService;
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
                    //Then send the Confirmation Email to the User
                    await SendConfirmationEmail(model.Email, user);

                    // If the user is signed in and in the Admin role, then it is
                    // the Admin user that is creating a new user. 
                    // So, redirect the Admin user to ListUsers action of Administration Controller
                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    //If it is not Admin user, then redirect the user to RegistrationSuccessful View
                    return View("RegistrationSuccessful");
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
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
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (ModelState.IsValid)
    {
        // 1. Find user by email
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Avoid enumerating which part is invalid, just show generic error
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }

        // 2. Check password
        var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // 3. Now that we know the password is correct, check if email is confirmed
            if (!user.EmailConfirmed)
            {
                // Show a specific error message
                ModelState.AddModelError(string.Empty, "Your email address is not confirmed. Please confirm your email before logging in.");
                model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return View(model);
            }

            // 4. If email is confirmed, manually sign-in the user
            await signInManager.SignInAsync(user, isPersistent: model.RememberMe);

            // 5. Redirect user after successful sign in
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

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
            // Generic failure message for invalid credentials
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
    }

    // If we got this far, something failed, redisplay the login form
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

        /// <summary>
        /////////////////////////////////////////////////// Xac nhan email ////////////////////////////////////////////////////////////// 
        /// </summary>
        private async Task SendConfirmationEmail(string email,User user)
        {
            // Generate the email confirmation token
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            // Save the token into the AspNetUserTokens database table
            await userManager.SetAuthenticationTokenAsync(user, "EmailConfirmation", "EmailConfirmationToken", token);

            // Build the confirmation callback URL
            // protocol: HttpContext.Request.Scheme: Generate the fully qualified URL with domain
            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { UserId = user.Id, Token = token }, protocol: HttpContext.Request.Scheme);

            // Encode the link to prevent XSS and other injection attacks
            var safeLink = HtmlEncoder.Default.Encode(confirmationLink);

            // Craft a more polished email subject
            var subject = "Welcome to Dot Net Tutorials! Please Confirm Your Email";

            // Create a professional HTML body
            // Customize inline styles, text, and branding as needed
            var messageBody = $@"
        <div style=""font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:1.6;color:#333;"">
            <p>Hi {user.UserName},</p>

            <p>Thank you for creating an account at <strong>Dot Net Tutorials</strong>.
            To start enjoying all of our features, please confirm your email address by clicking the button below:</p>

            <p>
                <a href=""{safeLink}"" 
                   style=""background-color:#007bff;color:#fff;padding:10px 20px;text-decoration:none;
                          font-weight:bold;border-radius:5px;display:inline-block;"">
                    Confirm Email
                </a>
            </p>

            <p>If the button doesn’t work for you, copy and paste the following URL into your browser:
                <br />
                <a href=""{safeLink}"" style=""color:#007bff;text-decoration:none;"">{safeLink}</a>
            </p>

            <p>If you did not sign up for this account, please ignore this email.</p>

            <p>Thanks,<br />
            The Dot Net Tutorials Team</p>
        </div>
    ";

            //Send the Confirmation Email to the User Email Id
            await emailSender.SendEmailAsync(email, subject, messageBody, true);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string UserId, string Token)
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Token))
            {
                // Provide a descriptive error message for the view
                ViewBag.ErrorMessage = "The link is invalid or has expired. Please request a new one if needed.";
                return View();
            }

            //Find the User by Id
            var user = await userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                // Provide a descriptive error for a missing user scenario
                ViewBag.ErrorMessage = "We could not find a user associated with the given link.";
                return View();
            }

            // Attempt to confirm the email
            var result = await userManager.ConfirmEmailAsync(user, Token);

            //Once the Email is Confirm, remove the token from the database
            await userManager.RemoveAuthenticationTokenAsync(user, "EmailConfirmation", "EmailConfirmationToken");

            if (result.Succeeded)
            {
                ViewBag.Message = "Thank you for confirming your email address. Your account is now verified!";
                return View();
            }

            // If confirmation fails
            ViewBag.ErrorMessage = "We were unable to confirm your email address. Please try again or request a new link.";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendConfirmationEmail(bool IsResend = true)
        {
            if (IsResend)
            {
                ViewBag.Message = "Resend Confirmation Email";
            }
            else
            {
                ViewBag.Message = "Send Confirmation Email";
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null || await userManager.IsEmailConfirmedAsync(user))
            {
                // Handle the situation when the user does not exist or Email already confirmed.
                // For security, don't reveal that the user does not exist or Email is already confirmed
                return View("ConfirmationEmailSent");
            }

            //Then send the Confirmation Email to the User
            await SendConfirmationEmail(Email, user);

            return View("ConfirmationEmailSent");
        }

        /// <summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            // Check if the model passes all validation rules (e.g., [Required], [EmailAddress]).
            // If these rules fail, ModelState will contain errors.
            if (ModelState.IsValid)
            {
                // Attempt to find a user in the database whose email address matches the one entered by the user.
                var user = await userManager.FindByEmailAsync(model.Email);

                // If a user is found
                if (user != null)
                {
                    // Send the user an email containing a unique, secure link to reset their password.
                    // This is done by generating a password reset token and building the appropriate URL.
                    await SendForgotPasswordEmail(user.Email, user);

                    // Redirect the user to the "ForgotPasswordConfirmation" page,
                    // which typically displays a message that an email has been sent.
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }

                // If the user was not found, we still redirect to "ForgotPasswordConfirmation" without revealing that the email does not exist.
                // This approach helps prevent account enumeration and brute force attacks.
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If ModelState is not valid (e.g., the email field is empty or invalid),
            // re-display the form so the user can correct the errors.
            return View(model);
        }
        private async Task SendForgotPasswordEmail(string? email, User? user)
        {
            // Generate the reset password token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            // Save the token into the AspNetUserTokens database table
            await userManager.SetAuthenticationTokenAsync(user, "ResetPassword", "ResetPasswordToken", token);

            // Build the password reset link
            var passwordResetLink = Url.Action("ResetPassword", "Account",
                new { Email = email, Token = token }, protocol: HttpContext.Request.Scheme);

            // Encode the link to prevent XSS attacks
            var safeLink = HtmlEncoder.Default.Encode(passwordResetLink);

            // Create the email subject
            var subject = "Reset Your Password";

            // Craft HTML message body
            var messageBody = $@"
    <div style=""font-family: Arial, Helvetica, sans-serif; font-size: 16px; color: #333; line-height: 1.5; padding: 20px;"">
        <h2 style=""color: #007bff; text-align: center;"">Password Reset Request</h2>
        <p style=""margin-bottom: 20px;"">Hi {user.UserName},</p>
        
        <p>We received a request to reset your password for your <strong>Dot Net Tutorials</strong> account. If you made this request, please click the button below to reset your password:</p>
        
        <div style=""text-align: center; margin: 20px 0;"">
            <a href=""{safeLink}"" 
               style=""background-color: #007bff; color: #fff; padding: 10px 20px; text-decoration: none; font-weight: bold; border-radius: 5px; display: inline-block;"">
                Reset Password
            </a>
        </div>
        
        <p>If the button above doesn’t work, copy and paste the following URL into your browser:</p>
        <p style=""background-color: #f8f9fa; padding: 10px; border: 1px solid #ddd; border-radius: 5px;"">
            <a href=""{safeLink}"" style=""color: #007bff; text-decoration: none;"">{safeLink}</a>
        </p>
        
        <p>If you did not request to reset your password, please ignore this email or contact support if you have concerns.</p>
        
        <p style=""margin-top: 30px;"">Thank you,<br />The Dot Net Tutorials Team</p>
    </div>";

            // Send the email
            await emailSender.SendEmailAsync(email, subject, messageBody, IsBodyHtml: true);
        }
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string Token, string Email)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (Token == null || Email == null)
            {
                ViewBag.ErrorTitle = "Invalid Password Reset Token";
                ViewBag.ErrorMessage = "The Link is Expired or Invalid";
                return View("Error");
            }
            else
            {
                ResetPasswordViewModel model = new ResetPasswordViewModel();
                model.Token = Token;
                model.Email = Email;
                return View(model);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            // Check if the incoming model passes all validation rules specified in the data annotations.
            if (ModelState.IsValid)
            {
                // Find the user in the database using the provided email address.
                var user = await userManager.FindByEmailAsync(model.Email);

                // Proceed only if the user exists in the database.
                if (user != null)
                {
                    // Attempt to reset the user's password using the token and the new password provided in the model.
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    //Once the Password is Reset, remove the token from the database
                    await userManager.RemoveAuthenticationTokenAsync(user, "ResetPassword", "ResetPasswordToken");

                    // If the password reset operation is successful, redirect the user to the Reset Password Confirmation page.
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ResetPasswordConfirmation", "Account");
                    }

                    // If the password reset fails, loop through the list of errors returned by Identity
                    // and add them to the ModelState to display on the view.
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description); // Add each error description to ModelState.
                    }

                    // Return the model back to the view so the user can fix any validation errors.
                    return View(model);
                }

                // If the user is not found, redirect to the Reset Password Confirmation page to avoid
                // revealing whether an account exists for the provided email.
                // This approach prevents account enumeration attacks.
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            // If the model state is invalid (e.g., missing or incorrect data), return the same view
            // and display validation errors to the user.
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        
    }
}
