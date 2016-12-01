using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Helper;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Repositories.Account;
using SQLite;

namespace MyCryptos.Core.Database
{
    public class AccountRepositoryDatabase : AbstractDatabase<AccountRepositoryDBM, AccountRepository, int>
    {
        public async override Task<IEnumerable<AccountRepositoryDBM>> GetAllDbObjects()
        {
            return await (await Connection).Table<AccountRepositoryDBM>().ToListAsync();
        }

        public async override Task<AccountRepositoryDBM> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<AccountRepositoryDBM>(p => p.Id == id);
        }

        protected override Task Create(SQLiteAsyncConnection connection)
        {
            return connection.CreateTableAsync<AccountRepositoryDBM>();

        }

        protected override AccountRepositoryDBM Resolve(AccountRepository element)
        {
            return new AccountRepositoryDBM(element);
        }
    }
}

