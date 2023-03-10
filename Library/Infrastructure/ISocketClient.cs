using Microsoft.AspNetCore.SignalR.Client;

namespace ShareInvest.Infrastructure;

public interface ISocketClient<T>
{
    HubConnection Hub
    {
        get;
    }
    IDisposable On(string name);

    event EventHandler<T> Send;
}