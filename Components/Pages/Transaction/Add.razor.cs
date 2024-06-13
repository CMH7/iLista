using Microsoft.AspNetCore.Components;

namespace iLista.Components.Pages.Transaction;

public partial class Add : IAsyncDisposable
{
    [Inject] private AppState appState { get; set; }

    // Int
    private int _selectedIndex { get; set; }

    protected override void OnInitialized()
    {
        setup();
        base.OnInitialized();
    }

    private void setup()
    {
        appState ??= new();
        appState.CurrentPage = "Add Transaction";
        appState.stateHasChanged += StateHasChanged;
    }

    public ValueTask DisposeAsync()
    {
        appState.stateHasChanged -= StateHasChanged;
        return new ValueTask();
    }

    private void TabChanged(int index)
    {
        _selectedIndex = index;
        StateHasChanged();
    }
}