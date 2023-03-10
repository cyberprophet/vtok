using Geocoding.Google;

namespace ShareInvest.Server.Services;

public class GeoService : GoogleGeocoder
{
    public GeoService(IConfiguration configuration) :

        base(configuration[nameof(GoogleAddressType)[..^0xB]]) =>

        Language = "ko";
}