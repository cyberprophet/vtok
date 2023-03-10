using System.Reflection;

namespace ShareInvest.Models.OpenAPI.Request;

public static class Constructer
{
    public static TR? GetInstance(string name, string scrNo)
    {
        if (store.Remove(scrNo, out TR? value))
        {
            return value;
        }
        var typeName = string.Concat(typeof(Constructer).Namespace, '.', name);

        return Assembly.GetExecutingAssembly()
                       .CreateInstance(typeName, true) as TR;
    }
    public static bool TryAdd(string key, TR value)
    {
        return store.TryAdd(key, value);
    }
    static readonly Dictionary<string, TR> store = new();
}