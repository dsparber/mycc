using SQLite;

namespace MyCC.Core.Database
{
    public interface ISqLiteConnection
    {
        SQLiteAsyncConnection GetOldConnection();
        SQLiteAsyncConnection Connection { get; }
    }
}

