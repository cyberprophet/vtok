using Microsoft.AspNetCore.Authentication;

using Newtonsoft.Json;

using ShareInvest.Models.Kakao;
using ShareInvest.Server.Data;
using ShareInvest.Server.Data.Models;
using ShareInvest.Server.Services;

namespace ShareInvest.Server.Extensions;

public static class AuthExtensions
{
    public static WebApplicationBuilder ConfigureAuthenticates(this WebApplicationBuilder builder)
    {
        builder.Services
               .AddIdentityServer(o =>
               {
                   o.LicenseKey = builder.Configuration["DuendeLicenseKey"];
                   o.KeyManagement.Enabled = false;
               })
               .AddConfigurationStore(o =>
               {

               })
               .AddOperationalStore(o =>
               {

               })
               /*
               .AddAspNetIdentity<CoreUser>()
               */
               .AddApiAuthorization<CoreUser, CoreContext>(o =>
               {

               });
        builder.Services
               .AddAuthentication(o =>
               {

               })
               /*
               .AddCookie(o =>
               {
                   o.CookieManager = new ChunkingCookieManager
                   {
                       ThrowForPartialCookies = true
                   };
                   o.Cookie.SameSite = SameSiteMode.None;
                   o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                   o.Cookie.HttpOnly = true;
               })
               */
               .AddKakaoTalk(o =>
               {
                   o.Events.OnCreatingTicket = async o =>
                   {
                       var tokenList = o.Properties.GetTokens().ToList();

                       var kakaoUser = JsonConvert.DeserializeObject<KakaoUser>(o.User.GetRawText());

                       if (kakaoUser != null)
                           foreach (var token in new PropertyService().GetEnumerator(kakaoUser))
                           {
                               if (tokenList.Any(o => o.Name.Equals(token.Name)) ||
                                   string.IsNullOrEmpty(token.Value))
                                   continue;

                               tokenList.Add(token);
                           }
                       o.Properties.StoreTokens(tokenList);

                       await Task.CompletedTask;
                   };
                   o.ClientId = builder.Configuration["KakaoTalk:ClientId"];
                   o.ClientSecret = builder.Configuration["KakaoTalk:ClientSecret"];
                   o.SaveTokens = true;
               })
               .AddIdentityServerJwt();

        return builder;
    }
}