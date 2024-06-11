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
        base.OnInitialized();
    }

    protected override async Task OnInitializedAsync()
    {
        if (appState is not null && appState.CurrentUser is not null && appState.CurrentAccount is not null) creds.UserName = appState.CurrentUser.UserName;
        else
        {
            bool hasusers = await HasUsers();
            if (!hasusers)
            {
                appState = new();
                navManager.NavigateTo("/", replace: true);
            }

            await SetCurrentUserAndAccount();
        }
        await base.OnInitializedAsync();
    }

    private async Task SetCurrentUserAndAccount()
    {
        User currentUser = (await App.Db.GetByConditionAsyncList<User>(x => x.CurrentUser)).First();
        if(currentUser is null)
            currentUser = (await App.Db.GetAllAsync<User>()).First();
        
        // set as state only not as current user in db
        // setting as current user in db will only happen upon signing in
        appState.CurrentUser = currentUser;
        creds.UserName = appState.CurrentUser.UserName;
    }

    private async Task<bool> HasUsers()
    {
        List<User> users = await App.Db.GetAllAsync<User>();
        return users.Count > 0;
    }

    private void setup()
    {
        appState ??= new();
        appState.CurrentPage = "Sign in";
        appState.stateHasChanged += StateHasChanged;
    }

    private bool Validator(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        if (text.Trim().Length < 8) return false;
        return true;
    }

    private async Task Submit()
    {
        try
        {
            User? user = await GetUser();
            if (user is null) return;

            if (appState.CurrentUser.Id == user.Id) await UpdateCurrentUserAndAccount(user);
            else
            {
                await SwitchCurrentUsers(user);
                await SwitchCurrentAccounts(user);
            }

            navManager.NavigateTo("/home", replace: true);
        }
        catch (Exception ex)
        {
            notifService.Notify(NotificationSeverity.Info, detail: $"{ex.Message}", closeOnClick: true);
        }
    }

    private async Task UpdateCurrentUserAndAccount(User sameUser)
    {
        try
        {
            // User
            User oldSameUser = (await App.Db.GetByConditionAsyncList<User>(x => x.Id == sameUser.Id)).First();
            oldSameUser.CurrentUser = true;
            await App.Db.UpdateAsync(oldSameUser);

            // Account
            Account oldSameAccount = (await App.Db.GetByConditionAsyncList<Account>(x => x.Owner == sameUser.Id && x.CurrentAccount)).First();
            oldSameAccount.CurrentAccount = true;
            await App.Db.UpdateAsync(oldSameAccount);

            appState.CurrentUser = oldSameUser;
            appState.CurrentAccount = oldSameAccount;
        }
        catch (Exception ex)
        {
            notifService.Notify(NotificationSeverity.Error, detail: $"{ex.Message}", closeOnClick: true);
            throw;
        }
    }

    private async Task SwitchCurrentUsers(User newLoggedInUser)
    {
        try
        {
            // Update the newly logged in user as the current user and false to the old user
            User newlyLoggedInUser = (await App.Db.GetByConditionAsyncList<User>(x => x.Id == newLoggedInUser.Id)).First();
            newlyLoggedInUser.CurrentUser = true;
            await App.Db.UpdateAsync(newlyLoggedInUser);

            User oldLoggedInUser = (await App.Db.GetByConditionAsyncList<User>(x => x.Id == appState.CurrentUser.Id)).First();
            oldLoggedInUser.CurrentUser = false;
            await App.Db.UpdateAsync(oldLoggedInUser);

            appState.CurrentUser = newlyLoggedInUser;
        }
        catch (Exception ex)
        {
            notifService.Notify(NotificationSeverity.Error, detail: $"{ex.Message}", closeOnClick: true);
            throw;
        }
    }

    private async Task SwitchCurrentAccounts(User newLoggedInUser)
    {
        try
        {
            // Update the newly logged in user's account as the current account and false to the old user's account
            Account newlyLoggedInAccount = (await App.Db.GetByConditionAsyncList<Account>(x => x.Owner == newLoggedInUser.Id)).First();
            newlyLoggedInAccount.CurrentAccount = true;
            await App.Db.UpdateAsync(newlyLoggedInAccount);

            Account oldLoggedInAccount = (await App.Db.GetByConditionAsyncList<Account>(x => x.Id == appState.CurrentAccount.Id)).First();
            oldLoggedInAccount.CurrentAccount = false;
            await App.Db.UpdateAsync(oldLoggedInAccount);

            appState.CurrentAccount = newlyLoggedInAccount;
        }
        catch (Exception ex)
        {
            notifService.Notify(NotificationSeverity.Error, detail: $"{ex.Message}", closeOnClick: true);
            throw;
        }
    }

    private async Task<User?> GetUser()
    {
        List<User> users = await App.Db.GetByConditionAsyncList<User>(x => x.UserName == creds.UserName);
        if(users is null || users.Count == 0)
        {
            notifService.Notify(NotificationSeverity.Error, detail: "Username not found", closeOnClick: true);
            return null;
        }
        
        if(users.First().Password != creds.Password)
        {
            notifService.Notify(NotificationSeverity.Error, detail: "Wrong password", closeOnClick: true);
            return null;
        }

        return users.First();
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