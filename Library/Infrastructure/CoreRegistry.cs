using Microsoft.Win32;

using ShareInvest.Infrastructure.Kiwoom;
using ShareInvest.Properties;

using System.Runtime.Versioning;

namespace ShareInvest.Infrastructure;

[SupportedOSPlatform("windows8.0")]
public class CoreRegistry
{
    public bool IsNotInstalled
    {
        get
        {
            bool isNotInstalled = true;

            using (var subKey = Registry.CurrentUser.OpenSubKey(Resources.SW))
            {
                if (subKey is not null)
                    isNotInstalled = IsInstalled(subKey) is false;
            }
            using (var subKey = Registry.LocalMachine.OpenSubKey(Resources.SW))
            {
                if (subKey is not null && isNotInstalled)
                    isNotInstalled = IsInstalled(subKey) is false;
            }
            return isNotInstalled;
        }
    }
    public CoreRegistry(string? name)
    {
        this.name = name ?? string.Empty;
    }
    bool IsInstalled(RegistryKey subKey)
    {
        using (var api = subKey.OpenSubKey(name.ToLower()))
        {
            if (api is not null)
                foreach (string name in api.GetValueNames())
                {
                    if (nameof(OpenAPI).Equals(name))
                        return true;
                }
        }
        return false;
    }
    readonly string name;
}