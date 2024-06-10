namespace iLista;

public class AppState
{
    public Action stateHasChanged;

    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            stateHasChanged?.Invoke();
        }
    }

    public User CurrentUser { get; set; }

    public Account CurrentAccount { get; set; }

    public string CurrentPage { get; set; }
}
