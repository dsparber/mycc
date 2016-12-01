using System.IO;
using Windows.Storage;
using data.database.interfaces;
using MyCryptos.UWP.data.database;
using Xamarin.Forms;
using SQLite;

[assembly: Dependency(typeof(SqLiteConnectionUwp))]
namespace MyCryptos.UWP.data.database
{
    public class SqLiteConnectionUwp : ISQLiteConnection
    {
        public SQLiteAsyncConnection GetConnection()
        {
            const string sqliteFilename = "MyCryptos.db";
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);
            var conn = new SQLiteAsyncConnection(path);
            return conn;
        }
    }
}

