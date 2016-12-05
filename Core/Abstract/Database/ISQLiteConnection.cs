using SQLite;

namespace MyCryptos.Core.Database.Interfaces
{
    public interface ISQLiteConnection
    {
        SQLiteAsyncConnection GetConnection();
    }
}

