using System;
using System.IO;
using MyCC.Core.Helpers;
using SQLite;

namespace MyCC.Forms.iOS.data.database
{
    public class SqLiteConnectionIos : ISqLiteConnection
    {
        public SQLiteAsyncConnection GetOldConnection() => GetConnection("MyCryptos.db");
        public SQLiteAsyncConnection Connection => GetConnection("MyCC.db");


        private static SQLiteAsyncConnection GetConnection(string dbName)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = Path.Combine(libraryPath, dbName);

            return new SQLiteAsyncConnection(path);
        }

    }
}

