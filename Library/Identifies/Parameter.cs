using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ShareInvest.Models;
using ShareInvest.Models.OpenAPI.Response;
using ShareInvest.Properties;

using System.Text;
using System.Text.RegularExpressions;

namespace ShareInvest.Identifies;

public static class Parameter
{
    public static string TransformQuery(JToken token)
    {
        StringBuilder query = new("?");

        foreach (var j in token.Children<JProperty>())

            if (JTokenType.Null != j.Value.Type)
            {
                query.Append(j.Path);
                query.Append('=');
                query.Append(j.Value);
                query.Append('&');
            }
        return query.Remove(query.Length - 1, 1)
                    .ToString();
    }
    public static string TransformQuery(JToken token,
                                        StringBuilder query)
    {
        query.Append('?');

        foreach (var j in token.Children<JProperty>())

            if (JTokenType.Null != j.Value.Type)
            {
                query.Append(j.Path);
                query.Append('=');
                query.Append(j.Value);
                query.Append('&');
            }
        return TransformOutbound(query.Remove(query.Length - 1, 1)
                                      .ToString());
    }
    public static string TransformOutbound(string route)
    {
        return Regex.Replace(route,
                             "([a-z])([A-Z])",
                             "$1-$2",
                             RegexOptions.CultureInvariant,
                             TimeSpan.FromMilliseconds(0x64))
                    .ToLowerInvariant();
    }
    public static string TransformInbound(string? query)
    {
        if (string.IsNullOrEmpty(query) is false)
        {
            var str = Regex.Replace(query,
                                    "-([a-z])",
                                    o => o.Groups[1].Value.ToUpper());

            return string.Concat(char.ToUpper(str[0]),
                                 str[1..]);
        }
        return string.Empty;
    }
    public static AccountBook? DeserializeOPW00004(string json)
    {
        var jEnumerable = JObject.Parse(json).AsJEnumerable();

        if (jEnumerable.Any(o => Resources.CODE.Equals(o.Path)))
        {
            return JsonConvert.DeserializeObject<BalanceOPW00004>(json);
        }
        return JsonConvert.DeserializeObject<AccountOPW00004>(json);
    }
    public static AccountBook? DeserializeOPW00005(string json)
    {
        var jEnumerable = JObject.Parse(json).AsJEnumerable();

        if (jEnumerable.Any(o => Resources.CODENUMBER.Equals(o.Path)))
        {
            return JsonConvert.DeserializeObject<BalanceOPW00005>(json);
        }
        return JsonConvert.DeserializeObject<AccountOPW00005>(json);
    }
}