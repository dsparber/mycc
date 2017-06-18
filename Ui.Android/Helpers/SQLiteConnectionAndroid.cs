using System.IO;
using MyCC.Core.Database;
using SQLite;

namespace MyCC.Ui.Android.Helpers
{
    public class SqLiteConnectionAndroid : ISqLiteConnection
    {
        private SQLiteAsyncConnection GetConnection(string sqliteFilename)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);

            return new SQLiteAsyncConnection(path);
        }

        public SQLiteAsyncConnection GetOldConnection() => GetConnection("MyCryptos.db");

        public SQLiteAsyncConnection Connection => GetConnection("MyCC.db");
    }
}

