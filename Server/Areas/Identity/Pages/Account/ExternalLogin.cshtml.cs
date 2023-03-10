using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Newtonsoft.Json;

using ShareInvest.Server.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace ShareInvest.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        public ExternalLoginModel(SignInManager<CoreUser> signInManager,
                                  UserManager<CoreUser> userManager,
                                  ILogger<ExternalLoginModel> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
        }
        [AllowNull, BindProperty]
        public InputModel Input
        {
            get; set;
        }
        public string? LoginProvider
        {
            get; set;
        }
        public string? ReturnUrl
        {
            get; set;
        }
        [TempData]
        public string? ErrorMessage
        {
            get; set;
        }
        public class InputModel
        {
            [Required,
             EmailAddress]
            public string? Email
            {
                get; set;
            }
        }
        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }
        public IActionResult OnPost(string provider,
                                    string? returnUrl = null)
        {
            var redirectUrl = Url.Page("./ExternalLogin",
                                       pageHandler: "Callback",
                                       values: new
                                       {
                                           returnUrl
                                       });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider,
                                                                                     redirectUrl);

            return new ChallengeResult(provider,
                                       properties);
        }
        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null,
                                                            string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";

                return RedirectToPage("./Login",
                                      new
                                      {
                                          ReturnUrl = returnUrl
                                      });
            }
            var info = await signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";

                return RedirectToPage("./Login",
                                      new
                                      {
                                          ReturnUrl = returnUrl
                                      });
            }
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                                                                      info.ProviderKey,
                                                                      isPersistent: false,
                                                                      bypassTwoFactor: true);

            if (result.Succeeded)
            {
                var user = await userManager.FindByLoginAsync(info.LoginProvider,
                                                              info.ProviderKey);
                var props = new AuthenticationProperties();

                props.StoreTokens(info.AuthenticationTokens);

                props.IsPersistent = true;

                await signInManager.SignInAsync(user,
                                                props,
                                                info.LoginProvider);

                if (info.LoginProvider.Equals(user.LoginProvider) is false)
                {
                    user.LoginProvider = info.LoginProvider;

                    await userManager.UpdateAsync(user);
                }
                var identityResult = await signInManager.UpdateExternalAuthenticationTokensAsync(info);

                if (identityResult.Succeeded)
                {
                    logger.LogInformation("{ } logged in with { } provider.",
                                          info.Principal.Identity?.Name,
                                          info.LoginProvider);
                }
                else
                {
                    logger.LogError("{ }", identityResult.Errors);
                }
                var stamp = user.ConcurrencyStamp;

                Response.DeclareTrailer(stamp);

                logger.LogInformation("stamp: { }", stamp);

                return LocalRedirect($"/Account/Profile?stamp={stamp}");
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;

                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                return Page();
            }
        }
        public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var info = await signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";

                return RedirectToPage("./Login",
                                      new
                                      {
                                          ReturnUrl = returnUrl
                                      });
            }
            if (ModelState.IsValid)
            {
                var user = new CoreUser
                {
                    UserName = Input?.Email,
                    Email = Input?.Email,
                    LoginProvider = info.LoginProvider
                };
                var result = await userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user,
                                                             info);

                    if (result.Succeeded)
                    {
                        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                        {
                            await userManager.AddClaimAsync(user,
                                                            info.Principal.FindFirst(ClaimTypes.GivenName));
                        }
                        if (info.Principal.HasClaim(c => c.Type == "urn:google:locale"))
                        {
                            await userManager.AddClaimAsync(user,
                                                            info.Principal.FindFirst("urn:google:locale"));
                        }
                        if (info.Principal.HasClaim(c => c.Type == "urn:google:picture"))
                        {
                            await userManager.AddClaimAsync(user,
                                                            info.Principal.FindFirst("urn:google:picture"));
                        }
                        var props = new AuthenticationProperties
                        {
                            IsPersistent = true
                        };
                        props.StoreTokens(info.AuthenticationTokens);

                        await signInManager.SignInAsync(user,
                                                        props,
                                                        info.LoginProvider);

                        var identityResult = await signInManager.UpdateExternalAuthenticationTokensAsync(info);

                        if (identityResult.Succeeded)
                        {
                            logger.LogInformation("{ } logged in with { } provider.",
                                                  info.Principal.Identity?.Name,
                                                  info.LoginProvider);
                        }
                        else
                        {
                            logger.LogError("{ }", identityResult.Errors);
                        }
                        var stamp = user.ConcurrencyStamp;

                        Response.DeclareTrailer(stamp);

                        logger.LogInformation("stamp: { }", stamp);

                        return LocalRedirect($"/Account/Profile?stamp={stamp}");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty,
                                             error.Description);
                }
            }
            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;

            return Page();
        }
        readonly SignInManager<CoreUser> signInManager;
        readonly UserManager<CoreUser> userManager;
        readonly ILogger<ExternalLoginModel> logger;
    }
}