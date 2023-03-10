using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ShareInvest.Server.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;

namespace ShareInvest.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        public RegisterModel(UserManager<CoreUser> userManager,
                             SignInManager<CoreUser> signInManager,
                             ILogger<RegisterModel> logger,
                             IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
        }
        [BindProperty,
         AllowNull]
        public InputModel Input
        {
            get; set;
        }
        public string? ReturnUrl
        {
            get; set;
        }
        public class InputModel
        {
            [Display(Name = "Email"),
             Required,
             EmailAddress]
            public string? Email
            {
                get; set;
            }
            [DataType(DataType.Password),
             Display(Name = "Password"),
             StringLength(100,
                          ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                          MinimumLength = 6),
             Required]
            public string? Password
            {
                get; set;
            }
            [Display(Name = "Confirm password"),
             Compare("Password",
                     ErrorMessage = "The password and confirmation password do not match."),
             DataType(DataType.Password)]
            public string? ConfirmPassword
            {
                get; set;
            }
        }
        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new CoreUser
                {
                    UserName = Input.Email,
                    Email = Input.Email
                };
                var result = await userManager.CreateAsync(user,
                                                           Input.Password);

                if (result.Succeeded)
                {
                    logger.LogInformation("User created a new account with password.");

                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Page("/Account/ConfirmEmail",
                                               pageHandler: null,
                                               values: new
                                               {
                                                   userId = user.Id,
                                                   code
                                               },
                                               protocol: Request.Scheme);

                    await emailSender.SendEmailAsync(Input.Email,
                                                     "Confirm your email",
                                                     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

                    await signInManager.SignInAsync(user,
                                                    isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty,
                                             error.Description);
            }
            return Page();
        }
        readonly SignInManager<CoreUser> signInManager;
        readonly UserManager<CoreUser> userManager;
        readonly ILogger<RegisterModel> logger;
        readonly IEmailSender emailSender;
    }
}