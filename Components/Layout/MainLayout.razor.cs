using Microsoft.AspNetCore.Components;

namespace iLista.Components.Layout;

public partial class MainLayout : IAsyncDisposable
{
    [Inject] private AppState appState { get; set; }
    [Inject] private NavigationManager navManager { get; set; }

    // List
    private List<Modules> modules { get; set; }

    protected override void OnInitialized()
    {
        modules ??= [
                new(){
                    Name = "Home",
                    Icon = "home",
                    Link = "/home",
                    Active = true
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
        setup();
        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if(modules.First(x => x.Active).Name != appState.CurrentPage) UpdateActiveModules(modules.IntersectBy([appState.CurrentPage], x => x.Name).ToList().First());
        base.OnAfterRender(firstRender);
    }

    private void setup()
    {
        appState.stateHasChanged += StateHasChanged;
    }

    private void NavigateTo(Modules module)
    {
        navManager.NavigateTo(module.Link);
    }

    private void UpdateActiveModules(Modules module)
    {
        modules.ForEach(x => x.Active = false);
        appState.CurrentPage = module.Name;
        module.Active = true;
        StateHasChanged();
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