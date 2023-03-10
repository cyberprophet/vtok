using Newtonsoft.Json;

using ShareInvest.Identifies;
using ShareInvest.Models.OpenAPI;
using ShareInvest.Models.OpenAPI.Response;

namespace ShareInvest.Observers;

public class JsonMessageEventArgs : MessageEventArgs
{
    public object? Convey
    {
        get;
    }
    public JsonMessageEventArgs(TR? tr, string json)
    {
        Convey = tr switch
        {
            Models.OpenAPI.Request.OPW00004 =>

                Parameter.DeserializeOPW00004(json),

            Models.OpenAPI.Request.OPW00005 =>

                Parameter.DeserializeOPW00005(json),

            Models.OpenAPI.Request.OPTKWFID =>

                JsonConvert.DeserializeObject<OPTKWFID>(json),

            Models.OpenAPI.Request.OPT10081 =>

                JsonConvert.DeserializeObject<OPT10081>(json),

            _ => throw new ArgumentNullException(json)
        };
    }
    public JsonMessageEventArgs(TR? ctor)
    {
        Convey = ctor;
    }
}