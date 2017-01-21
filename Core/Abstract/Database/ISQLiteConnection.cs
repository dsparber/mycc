using SQLite;

namespace MyCC.Core.Abstract.Database
{
    public interface ISQLiteConnection
    {
        SQLiteAsyncConnection GetConnection();
    }
}

