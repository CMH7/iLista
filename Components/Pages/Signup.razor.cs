using iLista.Providers;
using Microsoft.AspNetCore.Components;

namespace iLista.Components.Pages;

public partial class Signup : IAsyncDisposable
{
    [Inject] private AppState appState { get; set; }
    [Inject] private NavigationManager navManager { get; set; }
    [Inject] private NotificationService notifService { get; set; }

    private UserVM userVM { get; set; }

    protected override void OnInitialized()
    {
        setup();
        userVM ??= new();
        base.OnInitialized();
    }

    protected override async Task OnInitializedAsync()
    {
        bool hasUsers = await HasUsers();
        if(hasUsers)
        {
            navManager.NavigateTo("/signin", replace: true);
        }
        await base.OnInitializedAsync();
    }

    private async Task<bool> HasUsers()
    {
        List<User> users = await App.Db.GetAllAsync<User>();
        return users.Count > 0;
    }

    private void setup()
    {
        appState ??= new();
        appState.CurrentPage = "Sign up";
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
            bool existing = await ExistingInDB();
            if (existing)
            {
                notifService.Notify(NotificationSeverity.Error, detail: "Username exists. Sign in instead", closeOnClick: true);
                return;
            }
            appState.IsBusy = true;
            notifService.Notify(NotificationSeverity.Info, detail: "Creating account", closeOnClick: true);
            await App.Db.InsertAsync(userVM.Adapt<User>());
            User newUser = (await App.Db.GetByConditionAsyncList<User>(x => x.UserName == userVM.UserName)).First();
            appState.CurrentUser = newUser;

            Account newUserAccount = new()
            {
                Owner = newUser.Id,
                Name = "Cash",
                CurrentAccount = true,
                CreatedBy = newUser.Id,
                CreatedDate = DateTimeProvider.Now,
                UpdatedBy = newUser.Id,
                UpdatedDate = DateTimeProvider.Now
            };
            await App.Db.InsertAsync(newUserAccount);
            Account newAcount = (await App.Db.GetByConditionAsyncList<Account>(x => x.Owner == newUser.Id)).First();
            appState.CurrentAccount = newUserAccount;

            appState.IsBusy = false;
            notifService.Notify(NotificationSeverity.Success, detail: "Account created!", closeOnClick: true);
            navManager.NavigateTo("/signin", replace: true);
        }
        catch (Exception ex)
        {
            notifService.Notify(NotificationSeverity.Info, detail: $"{ex.Message}", closeOnClick: true);
        }
    }

    private async Task<bool> ExistingInDB()
    {
        List<User> users = await App.Db.GetByConditionAsyncList<User>(x => x.UserName == userVM.UserName);
        return users is not null && users.Count > 0;
    }

    public ValueTask DisposeAsync()
    {
        appState.stateHasChanged -= StateHasChanged;
        return new ValueTask();
    }

    // Classes
    private class UserVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool CurrentUser { get; set; } = true;
    }
}