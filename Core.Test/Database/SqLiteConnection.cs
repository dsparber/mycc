using System.IO;
using MyCC.Core.Helpers;
using SQLite;

namespace MyCC.Core.Test.Database
{
    public class SqLiteConnection : ISqLiteConnection
    {
        private static SQLiteAsyncConnection GetConnection(string sqliteFilename)
        {
            var path = Path.Combine(Path.GetTempPath(), sqliteFilename);
            return new SQLiteAsyncConnection(path);
        }

        public SQLiteAsyncConnection GetOldConnection() => GetConnection("MyCryptos.db");

        public SQLiteAsyncConnection Connection => GetConnection("MyCC.db");

    }
}