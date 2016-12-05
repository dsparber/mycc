using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.Account.Repositories.Base;
using SQLite;

namespace MyCryptos.Core.Account.Database
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

