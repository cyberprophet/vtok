using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Server.Data;
using ShareInvest.Server.Data.Models;

using System.Net;

namespace ShareInvest.Server.Controllers;

[Route("[controller]"),
 ApiController]
public class AuthController : ControllerBase
{
    [ApiExplorerSettings(IgnoreApi = true),
     HttpGet("{scheme}")]
    public async Task GetAsync([FromRoute] string scheme)
    {
        if ((await signInManager.GetExternalAuthenticationSchemesAsync())
                                .Select(o => o.Name)
                                .Any(o => scheme.Equals(o)) is false)
        {
            logger.LogWarning("Unable to log in with { }.",
                              scheme);

            return;
        }
        var auth = await Request.HttpContext.AuthenticateAsync(scheme);

        if (auth.Succeeded &&
            auth?.Principal != null &&
            auth.Principal.Identities.Any(o => o.IsAuthenticated) &&
            string.IsNullOrEmpty(auth.Properties.GetTokenValue(Properties.Resources.ACCESS)) is false)
        {
            var providerKey = string.Empty;

            foreach (var sec in auth.Principal.Identities)
            {
                providerKey = sec.Claims.Single(o => o.Type.EndsWith(name)).Value;

                if ((await signInManager.ExternalLoginSignInAsync(scheme,
                                                                  providerKey,
                                                                  false))
                                        .Succeeded)
                {
                    var props = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };
                    props.StoreTokens(auth.Properties.GetTokens());

                    var user = await userManager.FindByLoginAsync(scheme,
                                                                  providerKey);

                    if (user is null)
                    {
                        user = new CoreUser
                        {
                            UserName = auth.Principal.Identity?.Name,
                            Email = auth.Principal.Identity?.Name
                        };
                        var result = await userManager.CreateAsync(user);

                        if (result.Succeeded)
                        {
                            logger.LogInformation("Create CoreUser is { }.",
                                                  auth.Principal.Identity?.Name);
                        }
                        else
                            foreach (var err in result.Errors)
                            {
                                logger.LogError("Error Code: { }\n\tError Description: { }",
                                                err.Code,
                                                err.Description);
                            }
                    }
                    await signInManager.SignInAsync(user,
                                                    props,
                                                    scheme);

                    if (await signInManager.GetExternalLoginInfoAsync() is ExternalLoginInfo info &&
                       (await signInManager.UpdateExternalAuthenticationTokensAsync(info)).Succeeded)

                        logger.LogInformation("{ } logged in with { } provider.",
                                              info.Principal.Identity?.Name,
                                              info.LoginProvider);
                    break;
                }
            }
            var queryScheme = new Dictionary<string, string>
            {
                {
                    nameof(providerKey),
                    providerKey
                },
                {
                    Properties.Resources.ACCESS,
                    auth.Properties.GetTokenValue(Properties.Resources.ACCESS) ?? string.Empty
                },
                {
                    Properties.Resources.REFRESH,
                    auth.Properties.GetTokenValue(Properties.Resources.REFRESH) ?? string.Empty
                }
            };
            if (context.Integrations != null)

                foreach (var dao in from o in context.Integrations.AsNoTracking()

                                    where providerKey.Equals(o.ProviderKey) &&
                                          scheme.Equals(o.LoginProvider)

                                    select new
                                    {
                                        o.SerialNumber,
                                        o.AccountNumber,
                                        o.Id
                                    })
                {
                    var accKey = string.Concat(nameof(dao.AccountNumber), dao.Id);
                    var serialKey = string.Concat(nameof(dao.SerialNumber), dao.Id);

                    if (queryScheme.ContainsKey(accKey))
                    {
                        logger.LogWarning("Duplicate { } account key { } found.",
                                          dao.Id,
                                          dao.AccountNumber);
                    }
                    if (queryScheme.ContainsKey(serialKey))
                    {
                        logger.LogWarning("Duplicate { } serial key { } found.",
                                          dao.Id,
                                          dao.SerialNumber);
                    }
                    queryScheme[accKey] = dao.AccountNumber;
                    queryScheme[serialKey] = dao.SerialNumber;
                }
            var url = string.Concat(callbackScheme,
                                    "://#",
                                    string.Join("&",
                                                from o in queryScheme
                                                where string.IsNullOrEmpty(o.Value) is false
                                                select $"{WebUtility.UrlEncode(o.Key)}={WebUtility.UrlEncode(o.Value)}"));

            Request.HttpContext.Response.Redirect(url);
        }
        else
            await Request.HttpContext.ChallengeAsync(scheme);
    }
    [ApiExplorerSettings(GroupName = "account"),
     HttpGet]
    public async Task<IActionResult> GetUserLoginInfoAsync([FromQuery] string? stamp)
    {
        if (string.IsNullOrEmpty(stamp))
            return Ok((await signInManager.GetExternalAuthenticationSchemesAsync())
                                          .Select(o => new
                                          {
                                              o.Name,
                                              o.DisplayName
                                          }));
        var user = context.Users.AsNoTracking()
                                .Single(o => stamp.Equals(o.ConcurrencyStamp));

        if (user != null)
        {
            var userLoginInfo = (await userManager.GetLoginsAsync(user))
                                                  .Single(o => o.LoginProvider.Equals(user.LoginProvider));

            if (userLoginInfo != null)
                return Ok(new Models.UserLoginInfo
                {
                    LoginProvider = userLoginInfo.LoginProvider,
                    ProviderKey = userLoginInfo.ProviderKey
                });
        }
        logger.LogWarning("unable to retrieve { }.",
                          stamp);

        return NoContent();
    }
    public AuthController(IConfiguration configuration,
                          ILogger<AuthController> logger,
                          SignInManager<CoreUser> signInManager,
                          UserManager<CoreUser> userManager,
                          CoreContext context)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.context = context;
        this.logger = logger;

        callbackScheme = configuration["CallbackScheme"];
        name = configuration["Name"];
    }
    readonly string name;
    readonly string callbackScheme;
    readonly CoreContext context;
    readonly UserManager<CoreUser> userManager;
    readonly SignInManager<CoreUser> signInManager;
    readonly ILogger<AuthController> logger;
}