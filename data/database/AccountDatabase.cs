using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database.models;
using data.database.helper;
using models;
using SQLite;
using Xamarin.Forms;

namespace data.database
{
	public class AccountDatabase
	{
		readonly SQLiteAsyncConnection database;
		readonly TagDatabase tagDatabase;
		readonly TagAccountMapDatabase tagAccountMapDatabase;
		readonly CurrencyDatabase currencyDatabase;

		public AccountDatabase()
		{
			database = DependencyService.Get<ISQLiteConnection>().GetConnection();
			database.CreateTableAsync<AccountDBM>().RunSynchronously();

			tagDatabase = new TagDatabase();
			tagAccountMapDatabase = new TagAccountMapDatabase();
			currencyDatabase = new CurrencyDatabase();
		}

		public async Task<IEnumerable<Account>> GetAccounts(int repositoryId)
		{
			var asyncList = database.Table<AccountDBM>().Where(a => a.RepositoryId == repositoryId).ToListAsync(); 
			var accounts = await Task.WhenAll((await asyncList).Select(async a => a.ToAccount(await currencyDatabase.Get(a.Id))));

			await Task.WhenAll(accounts.Select(async a =>
			{
				var tagIds = (await tagAccountMapDatabase.GetForAccountId(a.Id.Value)).Select(t => t.TagId);

				a.Tags = new List<Tag>((await tagDatabase.GetAll()).Where(t => tagIds.Contains(t.Id.Value)));
			}));

			return accounts;
		}

		public async Task WriteAccounts(int repositoryId, IEnumerable<Account> accounts)
		{
			await Task.WhenAll(accounts.Select(async a =>
			{
				var dbObj = new AccountDBM(a, repositoryId);
				await DatabaseHelper.InsertOrUpdate(database, dbObj);
				a.Id = dbObj.Id;

				await tagDatabase.Write(a.Tags);
				await tagAccountMapDatabase.DeleteWithAccountId(a.Id.Value);

				await Task.WhenAll(a.Tags.Select(async t =>
				{
					await tagAccountMapDatabase.Write(a, t);
				}));
			}));
		}
	}
}

