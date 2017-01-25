﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Settings;
using SQLite;

namespace MyCC.Core.Currency.Database
{
	public class CurrencyDatabase : AbstractDatabase<CurrencyDbm, Model.Currency, string>
	{
		public override async Task<IEnumerable<CurrencyDbm>> GetAllDbObjects()
		{
			return await (await Connection).Table<CurrencyDbm>().ToListAsync();
		}

		protected override async Task Create(SQLiteAsyncConnection connection)
		{
			await connection.CreateTableAsync<CurrencyDbm>();
			if (ApplicationSettings.VersionLastLaunch < new Version("0.5.4"))
			{
				await connection.ExecuteAsync("ALTER TABLE Currencies ADD COLUMN IsCrypto INTEGER;");
				await connection.ExecuteAsync("DELETE FROM Currencies;");
			}
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