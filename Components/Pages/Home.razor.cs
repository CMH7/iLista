using Microsoft.AspNetCore.Components;

namespace iLista.Components.Pages;

public partial class Home : IAsyncDisposable
{
    [Inject] private AppState appState { get; set; }

    protected override void OnInitialized()
    {
        setup();
        base.OnInitialized();
    }

    private void setup()
    {
        appState ??= new();
        appState.CurrentPage = "Home";
        appState.stateHasChanged += StateHasChanged;
    }

    public ValueTask DisposeAsync()
    {
        appState.stateHasChanged -= StateHasChanged;
        return new ValueTask();
    }
}