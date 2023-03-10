﻿using Microsoft.AspNetCore.Http.Connections;

using Newtonsoft.Json;

using ShareInvest.Server.Hubs;

namespace ShareInvest.Server.Extensions;

public static class SignalRExtensions
{
    public static WebApplicationBuilder ConfigureHubs(this WebApplicationBuilder builder)
    {
        builder.Services
               .AddSignalR(o =>
               {
                   o.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                   o.HandshakeTimeout = TimeSpan.FromSeconds(30);
                   o.EnableDetailedErrors = true;
               })
               .AddNewtonsoftJsonProtocol(o =>
               {
                   o.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
               });
        return builder;
    }
    public static WebApplication ConfigureHubs(this WebApplication app)
    {
        app.MapHub<KiwoomHub>(app.Configuration["Hubs:Kiwoom"], o =>
        {
            o.Transports = HttpTransportType.WebSockets |
                           HttpTransportType.LongPolling;
        });
        return app;
    }
}