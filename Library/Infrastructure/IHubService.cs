using Microsoft.AspNetCore.SignalR.Client;

namespace ShareInvest.Infrastructure;

public interface IHubService
{
    HubConnectionState State
    {
        get;
    }
    Task StartAsync();

    Task StopAsync();

    Task AddToGroupAsync(string code);

    Task RemoveFromGroupAsync(string code);
}