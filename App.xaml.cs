using iLista.Services.DB;

namespace iLista;

public partial class App : Application
{
    private static SqliteDB db;
    private const string DatabaseFilename = "iLista.db3";

    public static SqliteDB Db
    {
        get
        {
            if (db == null)
            {
                try
                {
                    db = new SqliteDB();
                    string PathOfDB = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        DatabaseFilename);
                    _ = db.Init(PathOfDB);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return db;
        }
    }

    public App()
    {
        InitializeComponent();
        Init_db();
        MainPage = new MainPage();
    }

    private static void Init_db()
    {
        db = new SqliteDB();
        string PathOfDB = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            DatabaseFilename);
        _ = db.Init(PathOfDB);
    }
}
