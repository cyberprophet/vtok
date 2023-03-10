using System.Runtime.Versioning;

namespace ShareInvest.Infrastructure.Kiwoom;

public class OpenAPI : CoreRegistry
{
    [SupportedOSPlatform("windows8.0")]
    public OpenAPI() : base(typeof(OpenAPI).Namespace?[^6..])
    {

    }
}