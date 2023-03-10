using Microsoft.AspNetCore.SignalR.Client;

namespace ShareInvest.Observers.Socket;

public class SignalEventArgs : MessageEventArgs
{
    public SignalEventArgs(HubConnectionState state)
    {
        State = state;
    }
    public HubConnectionState State
    {
        get;
    }
}