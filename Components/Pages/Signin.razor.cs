using Microsoft.AspNetCore.Components;

namespace iLista.Components.Pages;

public partial class Signin : IAsyncDisposable
{
    [Inject] private AppState appState { get; set; }
    [Inject] private NavigationManager navManager { get; set; }
    [Inject] private NotificationService notifService { get; set; }

    private Creds creds { get; set; }

    protected override void OnInitialized()
    {
        creds ??= new();
        setup();
        if (appState is not null && appState.CurrentUser is not null && appState.CurrentAccount is not null) creds.UserName = appState.CurrentUser.UserName;
        base.OnInitialized();
    }

    private void setup()
    {
        appState ??= new();
        appState.CurrentPage ??= "Sign in";
        appState.stateHasChanged += StateHasChanged;
    }

    private bool PassValidator()
    {
        if (string.IsNullOrEmpty(creds.Password)) return false;
        if (creds.Password.Trim().Length < 8) return false;
        return true;
    }

    private async Task Submit()
    {
        try
        {
            User? user = await GetUser();
            if(user is not null)
            {
                if(appState.CurrentUser.Id != user.Id)
                {
                    appState.CurrentUser = user;
                    appState.CurrentAccount = (await App.Db.GetByConditionAsyncList<Account>(x => x.Owner == user.Id)).First();
                }

                navManager.NavigateTo("/home", replace: true);
            }
        }
        catch (Exception ex)
        {
            notifService.Notify(NotificationSeverity.Info, detail: $"{ex.Message}", closeOnClick: true);
        }
    }

    private async Task<User?> GetUser()
    {
        List<User> users = await App.Db.GetByConditionAsyncList<User>(x => x.UserName == creds.UserName);
        if(users is not null && users.Count > 0)
        {
            if(users.First().Password == creds.Password) return users.First();
            notifService.Notify(NotificationSeverity.Error, detail: "Wrong password", closeOnClick: true);
            return null;
        }
        notifService.Notify(NotificationSeverity.Error, detail: "Username not found", closeOnClick: true);
        return null;
    }

    public ValueTask DisposeAsync()
    {
        appState.stateHasChanged -= StateHasChanged;
        return new ValueTask();
    }

    // Classes
    private class Creds
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}