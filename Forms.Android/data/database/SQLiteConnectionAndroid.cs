using System.IO;
using MyCC.Core.Abstract.Database;
using MyCC.Forms.Android.data.database;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteConnectionAndroid))]
namespace MyCC.Forms.Android.data.database
{
    public class SQLiteConnectionAndroid : ISQLiteConnection
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var sqliteFilename = "MyCryptos.db";

            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);

            return new SQLiteAsyncConnection(path);
        }
    }
}

