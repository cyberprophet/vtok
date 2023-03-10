namespace ShareInvest.Mappers;

public interface ILoginService<T>
{
    string? ProviderKey
    {
        get;
    }
    string? LoginProvider
    {
        get;
    }
    string? AccessToken
    {
        get;
    }
    string? RefreshToken
    {
        get;
    }
    string? IdToken
    {
        get;
    }
    IAsyncEnumerable<T> AuthenticateAsync(string scheme);
}