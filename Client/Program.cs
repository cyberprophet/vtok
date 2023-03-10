using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using ShareInvest.Client;
using ShareInvest.Client.Properties;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<HttpClient>(Resources.SERVER,
                                           client =>
                                           {
                                               client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                                           })
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped(o => o.GetRequiredService<IHttpClientFactory>()
                                 .CreateClient(Resources.SERVER));

builder.Services.AddApiAuthorization(o =>
{
    o.AuthenticationPaths.LogInPath = "account/login";
    o.AuthenticationPaths.LogOutPath = "account/logout";
    o.AuthenticationPaths.LogOutSucceededPath = "account/logged-out";
    o.AuthenticationPaths.ProfilePath = "account/profile";
    o.AuthenticationPaths.RegisterPath = "account/register";
});
builder.Logging.AddDebug();

if (builder.Build() is WebAssemblyHost host)
{
    await host.RunAsync();
}