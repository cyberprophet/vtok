using Newtonsoft.Json.Linq;

namespace ShareInvest.Infrastructure;

public interface ICoreClient
{
    Task<string> PostAsync<T>(int index, string route, T param) where T : class;

    Task<object> PostAsync<T>(string route, T param) where T : class;

    Task<object> GetAsync(string route, JToken token);

    Task<object> GetAsync(string route);
}