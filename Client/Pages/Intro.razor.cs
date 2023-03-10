using ShareInvest.Client.Components;
using ShareInvest.Client.Properties;

namespace ShareInvest.Client.Pages;

public partial class IntroBase : ConsoleLogger<string>
{
    protected override async Task OnInitializedAsync()
    {
        try
        {
            while (await InvokeAsync<bool>(Resources.DIV, Resources.INIT))
            {
                await Task.Delay(0x100);
            }
            var theme = await InvokeAsync<bool>(Resources.Theme);

            await InvokeVoidAsync(Resources.INITAILIZE,
                                  Resources.INIT,
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
                                      cameraOptions = new
                                      {
                                          center = new
                                          {
                                              lat = 37.40117195,
                                              lng = 127.10807595
                                          },
                                          tilt = 90,
                                          heading = -175,
                                          zoom = 5
                                      },
                                      zoomControl = false,
                                      fullscreenControl = false,
                                      mapTypeControl = false,
                                      minZoom = 5,
                                      mapId = theme ? Resources.DarkMap : Resources.GrayMAP
                                  });
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

        }
        catch (Exception ex)
        {
            LogError(ex.Message);
        }
        finally
        {
            if (firstRender)
            {
                await InvokeVoidAsync(Resources.SETCOORDINATE);
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}