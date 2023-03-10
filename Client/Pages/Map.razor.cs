using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

using Newtonsoft.Json;

using ShareInvest.Client.Components;
using ShareInvest.Client.Properties;
using ShareInvest.Client.Services;

using System.Net.Http.Json;

namespace ShareInvest.Client.Pages;

[Authorize]
public partial class MapBase : ConsoleLogger<Map>
{
    [JSInvokable]
    public void StateHasChanged(string state)
    {
        try
        {
            if (int.TryParse(state, out int id))
            {
                if (state.Length == 6)
                {

                }
                else
                {

                }
            }
            else
                IsLoading = Array.Exists(new[]
                {
                    Resources.ZOOMCHANGE
                },
                match => state.Equals(match));
        }
        catch (Exception ex)
        {
            LogError(ex.Message);
        }
        finally
        {
            StateHasChanged();
        }
    }
    protected override async Task OnInitializedAsync()
    {
        try
        {
            var query = string.Concat(Resources.COREAPI,
                                      '/',
                                      nameof(Models.Map));

            if (Http != null)
            {
                var res = await Http.GetFromJsonAsync<Models.Map[]>(query);

                var route = (await State!).User.Identities
                                               .FirstOrDefault(s => s.Claims.Any(p => Resources.ID.Equals(p.Type)))?
                                               .Claims
                                               .FirstOrDefault(p => Resources.ID.Equals(p.Type))?
                                               .Value;

                if (route != null)
                {
                    Id = route;

                    route = string.Concat(query, '/', route);

                    StartingPoint = await Http.GetFromJsonAsync<Models.Google.Location>(route);
                }
                Stocks = res?.ToDictionary(keySelector => keySelector.Code!,
                                           elementSelector => elementSelector);
            }
        }
        catch (Exception ex)
        {
            LogError(ex.Message);
        }
        finally
        {
            await base.OnInitializedAsync();
        }
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            switch (RenderingCount++)
            {
                case 3 when Http != null:

                    var route = string.Concat(Resources.API, '/', nameof(Resources.CoreMember));

                    route = string.Concat(Identifies.Parameter.TransformOutbound(route), '/', Id);

                    Members = await Http.GetFromJsonAsync<Models.Member[]>(route);

                    if (Members != null && Members.Length > 0)
                    {
                        await InvokeVoidAsync(Resources.CoreMember,
                                              new Pin().MakePins(Members));
                    }
                    break;

                case 2 when Stocks != null:

                    await InvokeVoidAsync(Resources.MARKERS,
                                          new
                                          {
                                              lat = StartingPoint.Latitude,
                                              lng = StartingPoint.Longitude
                                          },
                                          new Pin().MakePins(Stocks));
                    break;

                case 1 when await InvokeAsync<string>(Resources.GETCOORDINATE) is string json:

                    Crd = JsonConvert.DeserializeObject<Models.Google.Location>(json);

                    break;

                case 0:

                    await InvokeVoidAsync(Resources.SETCOORDINATE);

                    break;
            }
        }
        catch (Exception ex)
        {
            LogError(ex.Message);
        }
        finally
        {
            if (firstRender)
            {
                await InvokeVoidAsync(Resources.INITIALIZATION,
                                      nameof(Models.Map).ToLowerInvariant(),
                                      new
                                      {
                                          libraries = new[]
                                          {
                                              "drawing",
                                              "geometry",
                                              "places",
                                              "visualization"
                                          },
                                          apiKey = Resources.KEY
                                      },
                                      new
                                      {
                                          lng = Crd.Longitude == 0 ? 127.1059259 : Crd.Longitude,
                                          lat = Crd.Latitude == 0 ? 37.4001971 : Crd.Latitude
                                      },
                                      DotNetObjectReference.Create(this));

                IsLoading = firstRender;
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
    protected internal Dictionary<string, Models.Map>? Stocks
    {
        get; private set;
    }
    protected internal bool IsLoading
    {
        get; private set;
    }
    Models.Member[]? Members
    {
        get; set;
    }
    Models.Google.Location StartingPoint
    {
        get; set;
    }
    [Inject]
    HttpClient? Http
    {
        get; set;
    }
    [CascadingParameter]
    Task<AuthenticationState>? State
    {
        get; set;
    }
    uint RenderingCount
    {
        get; set;
    }
    string? Id
    {
        get; set;
    }
}