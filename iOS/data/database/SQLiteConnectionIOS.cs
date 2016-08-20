using System;
using System.IO;
using SQLite;
using data.database.interfaces;
using Xamarin.Forms;
using data.database;

[assembly: Dependency(typeof(SQLiteConnectionIOS))]
namespace data.database
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

