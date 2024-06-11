using iLista.Domain;
using SQLite;

namespace iLista.Models;

public class Account : DBObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int Owner { get; set; }
    public string Name { get; set; }
    public bool CurrentAccount { get; set; }
}
