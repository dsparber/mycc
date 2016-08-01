using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database.models;
using data.database.helper;
using models;
using SQLite;

namespace data.database
{
	public class AccountDatabase : AbstractRepositoryIdDatabase<AccountDBM, Account>
	{
		
		readonly TagDatabase tagDatabase;
		readonly TagAccountMapDatabase tagAccountMapDatabase;

		public AccountDatabase()
		{
			tagDatabase = new TagDatabase();
			tagAccountMapDatabase = new TagAccountMapDatabase();
		}

		public override async Task<IEnumerable<AccountDBM>> GetAllDbObjects()
		{
			return await Connection.Table<AccountDBM>().ToListAsync();
		}

		public override async Task<IEnumerable<Account>> GetAll()
		{
			var accounts = await base.GetAll();

			await Task.WhenAll(accounts.Select(async a =>
			{
				var tagIds = (await tagAccountMapDatabase.GetForAccountId(a.Id.Value)).Select(t => t.TagId);

				a.Tags = ((await tagDatabase.GetAll()).Where(t => tagIds.Contains(t.Id.Value))).ToList();
			}));

			return accounts;
		}

		public override async Task Write(IEnumerable<Account> data, int repositoryId)
		{
			await Task.WhenAll(data.Select(async a =>
			{
				var dbObj = new AccountDBM(a, repositoryId);
				await DatabaseHelper.InsertOrUpdate(this, dbObj);
				a.Id = dbObj.Id;

				await tagDatabase.Write(a.Tags);
				await tagAccountMapDatabase.DeleteWithAccountId(a.Id.Value);

				await Task.WhenAll(a.Tags.Select(async t =>
				{
					await tagAccountMapDatabase.Write(a, t);
				}));
			}));
		}

		public override Task<CreateTablesResult> Create()
		{
			return Connection.CreateTableAsync<AccountDBM>();
		}
	}
}