namespace ShareInvest.Observers.OpenAPI;

public class AxMessageEventArgs : MessageEventArgs
{
    public string? Title
    {
        get;
    }
    public string? Code
    {
        get;
    }
    public string? Screen
    {
        get;
    }
    public AxMessageEventArgs(string? title,
                              string? code,
                              string? screen)
    {
        Title = title;
        Code = code;
        Screen = screen;
    }
}