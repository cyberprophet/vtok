namespace ShareInvest.Observers.Socket;

public class GroupEventArgs : MessageEventArgs
{
    public GroupEventArgs(string name)
    {
        Name = name;
    }
    public string Name
    {
        get;
    }
}