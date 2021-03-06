using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Account.Models.Base;
using SQLite;

namespace MyCC.Core.Account.Database
{
    public class AccountDatabase : AbstractDatabase<AccountDbm, FunctionalAccount, int>
    {
        protected override async Task<IEnumerable<AccountDbm>> GetAllDbObjects()
        {
            return await (await Connection).Table<AccountDbm>().ToListAsync();
        }

        protected override Task Create(SQLiteAsyncConnection connection)
        {
            return connection.CreateTableAsync<AccountDbm>();
        }

        protected override async Task<AccountDbm> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<AccountDbm>(p => p.Id == id);
        }

        protected override AccountDbm Resolve(FunctionalAccount element)
        {
            return new AccountDbm(element);
        }
    }
}