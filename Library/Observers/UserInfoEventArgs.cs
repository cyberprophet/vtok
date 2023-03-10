using ShareInvest.Models;

namespace ShareInvest.Observers;

public class UserInfoEventArgs : MessageEventArgs
{
    public UserInfoEventArgs(User param)
    {
        User = param;
    }
    public User User
    {
        get;
    }
}