using System.Threading.Tasks;
using MyCC.Core.Database;
using MyCC.Core.Settings;

namespace MyCC.Core.Preperation
{
    public static class Migrate
    {
        public static bool MigrationsNeeded => ApplicationSettings.LastCoreVersion < new CoreVersion(1, 1, 4);

        public static async Task ExecuteMigratations()
        {
            if (ApplicationSettings.LastCoreVersion < new CoreVersion(1, 1))
            {
                await MigrateTo_1_1();
            }
            if (ApplicationSettings.LastCoreVersion < new CoreVersion(1, 1, 4))
            {
                await MigrateTo_1_1_4();
            }
        }

        private static async Task MigrateTo_1_1()
        {
            try
            {
                var connection = DatabaseUtil.OldConnection;
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
        private static async Task MigrateTo_1_1_4()
        {
            try
            {
                var connection = DatabaseUtil.OldConnection;
                await connection.ExecuteAsync("DELETE FROM ExchangeRates;");
            }
            catch
            {
                // Do nothing -> Table was already deleted
            }

        }
    }
}