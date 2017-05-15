using SQLite;

namespace MyCC.Core.Helpers
{
    public interface ISqLiteConnection
    {
        SQLiteAsyncConnection GetOldConnection();
        SQLiteAsyncConnection Connection { get; }
    }
}

