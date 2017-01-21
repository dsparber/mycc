﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Currency.Repositories;
using SQLite;

namespace MyCC.Core.Currency.Database
{
    public class CurrencyMapRepositoryDatabase : AbstractDatabase<CurrencyMapRepositoryDbm, CurrencyRepositoryMap, int>
    {
        public override async Task<IEnumerable<CurrencyMapRepositoryDbm>> GetAllDbObjects()
        {
            return await (await Connection).Table<CurrencyMapRepositoryDbm>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<CurrencyMapRepositoryDbm>();
        }

        public override async Task<CurrencyMapRepositoryDbm> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<CurrencyMapRepositoryDbm>(p => p.Id == id);
        }

        protected override CurrencyMapRepositoryDbm Resolve(CurrencyRepositoryMap element)
        {
            return new CurrencyMapRepositoryDbm(element);
        }
    }
}

