using AxKHOpenAPILib;

using Newtonsoft.Json;

namespace ShareInvest.Tr;

class OPT10081 : TR
{
    internal override IEnumerable<string> OnReceiveTrData(AxKHOpenAPI ax,
                                                          _DKHOpenAPIEvents_OnReceiveTrDataEvent e,
                                                          Models.OpenAPI.TR? tr)
    {
        var arr = tr?.Single is not null ? new string[tr.Single.Length] : null;

        if (arr is not null && ax is not null)
            for (int i = 0; i < tr?.Single.Length; i++)
                arr[i] = ax.GetCommData(e.sTrCode, e.sRQName, 0, tr.Single[i]).Trim();

        if (tr?.Multiple is not null)
        {
            var data = ax?.GetCommDataEx(e.sTrCode, e.sRQName);

            if (data is not null)
            {
                int x, y,
                    lx = ((object[,])data).GetUpperBound(0),
                    ly = ((object[,])data).GetUpperBound(1);

                var name = ax?.GetMasterCodeName(arr?[0]);

                for (x = 0; x <= lx; x++)
                {
                    var dic = new Dictionary<string, string>
                    {
                        { nameof(Models.Chart.Name), name ?? string.Empty },
                        { tr.Multiple[0], arr?[0] ?? string.Empty }
                    };
                    for (y = 1; y <= ly; y++)
                    {
                        dic[tr.Multiple[y]] = ((string)((object[,])data)[x, y]).Trim();
                    }
                    yield return JsonConvert.SerializeObject(dic);
                }
                yield return e.sPrevNext;
            }
        }
    }
}