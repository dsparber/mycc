using System.IO;
using MyCC.Core.Abstract.Database;
using MyCC.Ui.Android.Helpers;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SqLiteConnectionAndroid))]
namespace MyCC.Ui.Android.Helpers
{
    public class SqLiteConnectionAndroid : ISqLiteConnection
    {
        public SQLiteAsyncConnection GetConnection()
        {
            const string sqliteFilename = "MyCryptos.db";

            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);

            return new SQLiteAsyncConnection(path);
        }
    }
}

