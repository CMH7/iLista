using Microsoft.AspNetCore.Components;

namespace iLista.Components.Layout;

public partial class MainLayout : IAsyncDisposable
{
    [Inject] private AppState appState { get; set; }

    // List
    private List<Modules> modules { get; set; }
    private List<Account> accounts { get; set; }

    protected override void OnInitialized()
    {
        modules ??= [
                new(){
                    Name = "Home",
                    Icon = "home",
                    Link = "/home"
                },
                new(){
                    Name = "Add Transaction",
                    Icon = "add",
                    Link = "/transaction/add"
                },
                new(){
                    Name = "Settings",
                    Icon = "settings",
                    Link = "/settings"
                }
            ];
        accounts ??= [];
        setup();
        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if(firstRender)
        {
            UpdateActiveModules(modules[0]);
            StateHasChanged();
        }
        base.OnAfterRender(firstRender);
    }

    private void setup()
    {
        appState ??= new();
        appState.CurrentUser ??= new();
        appState.CurrentAccount ??= new();
        appState.stateHasChanged += StateHasChanged;
    }

    private void UpdateActiveModules(Modules module, bool rerender = false)
    {
        appState.CurrentPage = module.Name;
        module.Active = true;
        if (rerender) StateHasChanged();
    }

    public ValueTask DisposeAsync()
    {
        appState.stateHasChanged -= StateHasChanged;
        return new ValueTask();
    }

    private class Modules
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public bool Active { get; set; }
    }
}