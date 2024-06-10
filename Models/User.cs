using SQLite;

namespace iLista.Models;

public class User
{
    [PrimaryKey,AutoIncrement]
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool AutoLogin { get; set; }
}
