using Geocoding;

using Microsoft.AspNetCore.Server.Kestrel.Core;

using ShareInvest.Infrastructure;
using ShareInvest.Mappers;
using ShareInvest.Server.Services;
using ShareInvest.Server.Services.Dart;
using System.Security.Cryptography.X509Certificates;

namespace ShareInvest.Server.Extensions;

public static class ServiceExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services

               .AddScoped<IPropertyService, PropertyService>()

               .AddScoped<IScopedProcessingService, OverViewService>()

               .AddScoped<IGeocoder, GeoService>()

               .AddSingleton<StockService>()

               .AddHostedService<ScopedBackgroundService>()

               .Configure<KestrelServerOptions>(o =>
               {
                   o.ListenAnyIP(0x23BF, o =>
                   {
                       o.UseHttps(StoreName.My,
                                  builder.Configuration["Certificate"],
                                  true)
                        .UseConnectionLogging();
                   });
                   o.Limits.MaxRequestBodySize = null;
               });
        return builder;
    }
}