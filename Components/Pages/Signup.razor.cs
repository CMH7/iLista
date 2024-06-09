using Microsoft.AspNetCore.Components;

namespace iLista.Components.Pages;

public partial class Signup : IAsyncDisposable
{
    [Inject] private AppState appState { get; set; }
    [Inject] private NavigationManager navManager { get; set; }

    private UserVM userVM { get; set; }

    protected override void OnInitialized()
    {
        userVM ??= new();
        setup();
        base.OnInitialized();
    }

    private void setup()
    {
        appState ??= new();
        appState.stateHasChanged += StateHasChanged;
    }

    private bool PassValidator()
    {
        if (string.IsNullOrEmpty(userVM.Password)) return false;
        if (userVM.Password.Trim().Length < 8) return false;
        return true;
    }

    private void Submit()
    {
        navManager.NavigateTo("/signin", true, true);
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
        public string Password { get; set; }
    }
}