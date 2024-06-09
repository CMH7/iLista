using Microsoft.AspNetCore.Components;

namespace iLista.Components.Pages;

public partial class Signin : IAsyncDisposable
{
    [Inject] private AppState appState { get; set; }
    [Inject] private NavigationManager navManager { get; set; }

    private Creds creds { get; set; }

    protected override void OnInitialized()
    {
        creds ??= new();
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
        if (string.IsNullOrEmpty(creds.Password)) return false;
        if (creds.Password.Trim().Length < 8) return false;
        return true;
    }

    private void Submit()
    {
        navManager.NavigateTo("/home", true, true);
    }

    public ValueTask DisposeAsync()
    {
        appState.stateHasChanged -= StateHasChanged;
        return new ValueTask();
    }

    // Classes
    private class Creds
    {
        public string Password { get; set; }
    }
}