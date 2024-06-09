using iLista.Providers;

namespace iLista.Domain;

public class DBObject
{
    public DateTime CreatedDate { get; set; } = DateTimeProvider.Now;
    public int CreatedBy { get; set; }
    public DateTime UpdatedDate { get; set; } = DateTimeProvider.Now;
    public int UpdatedBy { get; set; }
}
