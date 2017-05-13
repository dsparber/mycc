using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Settings;
using SQLite;

namespace MyCC.Core.Currency.Database
{
    public class CurrencyDatabase : AbstractDatabase<CurrencyDbm, Model.Currency, string>
    {
        private static bool _executeAdditionalCommands;

        protected override async Task<IEnumerable<CurrencyDbm>> GetAllDbObjects()
        {
            return await (await Connection).Table<CurrencyDbm>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<CurrencyDbm>();

            if (_executeAdditionalCommands) return;
            _executeAdditionalCommands = true;

            if (ApplicationSettings.LastCoreVersion < new Version("0.5.49"))
            {
                await connection.ExecuteAsync("DELETE FROM Currencies;");
                await connection.ExecuteAsync("DELETE FROM CurrencyMap;");
                await connection.ExecuteAsync("DELETE FROM CurrencyRepositories;");
                await connection.ExecuteAsync("DELETE FROM CurrencyRepositoryMap;");
            }
        }

        protected override async Task Drop(SQLiteAsyncConnection connection)
        {
            await connection.DropTableAsync<CurrencyDbm>();
        }

        public override async Task<CurrencyDbm> GetDbObject(string id)
        {
            return await (await Connection).FindAsync<CurrencyDbm>(p => p.Id.Equals(id));
        }

        protected override CurrencyDbm Resolve(Model.Currency element)
        {
            return new CurrencyDbm(element);
        }
    }
}