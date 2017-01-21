using System;
using System.IO;
using MyCC.Core.Abstract.Database;
using MyCC.Forms.iOS.data.database;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteConnectionIOS))]
namespace MyCC.Forms.iOS.data.database
{
    public class SQLiteConnectionIOS : ISQLiteConnection
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var sqliteFilename = "MyCryptos.db";

            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = Path.Combine(libraryPath, sqliteFilename);

            return new SQLiteAsyncConnection(path);
        }
    }
}

