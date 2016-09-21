using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database.models;
using data.database.helper;
using models;
using SQLite;
using System;

namespace data.database
{
	public class AccountDatabase : AbstractEntityRepositoryIdDatabase<AccountDBM, Account>
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
			return await (await Connection()).Table<AccountDBM>().ToListAsync();
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
			var dbObjects = data.Select(a => new Tuple<Account, AccountDBM> (a, new AccountDBM(a, repositoryId))).ToList();

			await DatabaseHelper.InsertOrUpdate(this, dbObjects.Select(o => o.Item2));

			await Task.WhenAll(dbObjects.Select(async t =>
			{
				await tagDatabase.Write(t.Item1.Tags);
				await tagAccountMapDatabase.DeleteWithAccountId(t.Item2.Id);

				await Task.WhenAll(t.Item1.Tags.Select(async x =>
				{
					await tagAccountMapDatabase.Write(t.Item1, x);
				}));
			}));
		}

		protected override Task<CreateTablesResult> Create()
		{
			return ConnectionWithoutCreate.CreateTableAsync<AccountDBM>();
		}

		public async override Task Delete(Account element, int repositoryId)
		{
			await DatabaseHelper.Delete(this, new AccountDBM(element, repositoryId));
		}
	}
}