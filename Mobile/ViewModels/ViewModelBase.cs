using CommunityToolkit.Mvvm.ComponentModel;

using ShareInvest.Properties;

namespace ShareInvest.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    public bool IsNotBusy => isBusy is false;

    public abstract Task InitializeAsync();

    public abstract Task DisposeAsync();

    protected async Task DisplayAlert(string message)
    {
        await Shell.Current.DisplayAlert(title,
                                         message,
                                         Resources.OK);
    }
    [ObservableProperty]
    string? title;

    [ObservableProperty,
     NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;
}