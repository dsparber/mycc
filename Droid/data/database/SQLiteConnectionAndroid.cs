﻿using System.IO;
using SQLite;
using data.database.interfaces;
using Xamarin.Forms;
using data.database;

[assembly: Dependency(typeof(SQLiteConnectionAndroid))]
namespace data.database
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

