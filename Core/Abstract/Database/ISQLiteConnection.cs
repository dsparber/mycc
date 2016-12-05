using SQLite;

namespace MyCryptos.Core.Abstract.Database
{
    public interface ISQLiteConnection
    {
        SQLiteAsyncConnection GetConnection();
    }
}

