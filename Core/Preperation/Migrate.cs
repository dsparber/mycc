using System.Threading.Tasks;
using MyCC.Core.Helpers;
using MyCC.Core.Settings;
using Xamarin.Forms;

namespace MyCC.Core.Preperation
{
    public static class Migrate
    {
        public static bool MigrationsNeeded => ApplicationSettings.LastCoreVersion < new Version(1, 1);

        public static async Task ExecuteMigratations()
        {
            if (ApplicationSettings.LastCoreVersion < new Version(1, 1))
            {
                await MigrateTo_1_1();
            }
        }

        private static async Task MigrateTo_1_1()
        {
            try
            {
                var connection = DependencyService.Get<ISqLiteConnection>().GetOldConnection();
                await connection.ExecuteAsync("DROP TABLE Currencies;");
                await connection.ExecuteAsync("DROP TABLE CurrencyMap;");
                await connection.ExecuteAsync("DROP TABLE CurrencyRepositories;");
                await connection.ExecuteAsync("DROP TABLE CurrencyRepositoryMap;");
            }
            catch
            {
                // Do nothing -> Table was already deleted
            }

        }
    }
}