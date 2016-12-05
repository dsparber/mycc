﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Currency.Database;
using MyCryptos.Core.Currency.Storage;

namespace MyCryptos.Core.Currency.Repositories
{
	public abstract class OnlineCurrencyRepository : CurrencyRepository
	{
		protected OnlineCurrencyRepository(int id) : base(id) { }

		public override async Task<bool> LoadFromDatabase()
		{
			LastFastFetch = DateTime.Now;
			return await FetchFromDatabase();
		}

		protected abstract Task<IEnumerable<Model.Currency>> GetCurrencies();

		public override async Task<bool> FetchOnline()
		{
			var currentElements = await GetCurrencies();
			var mapElements = new List<CurrencyMapDbm>();

			if (currentElements == null) return false;
			foreach (var c in currentElements)
			{
				var existing = CurrencyStorage.Instance.LocalRepository.Elements.ToList().Find(c.Equals);

				if (existing != null)
				{
					c.Name = (c.Name.Equals(c.Code)) ? existing.Name : c.Name;
				}
				await CurrencyStorage.Instance.LocalRepository.AddOrUpdate(c);

				var mapEntry = new CurrencyMapDbm { Code = c.Code, ParentId = Id };
				var all = CurrencyRepositoryMapStorage.Instance.AllElements;
				var existingMapEntry = all.Find(e => e.Code.Equals(c.Code) && e.ParentId == Id);

				mapElements.Add(existingMapEntry ?? mapEntry);

				if (existingMapEntry == null)
				{
					await CurrencyRepositoryMapStorage.Instance.LocalRepository.Add(mapEntry);
				}
			}

			var toDelete = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.ParentId == Id).Where(e => !mapElements.Contains(e));
			await Task.WhenAll(toDelete.Select(e => CurrencyRepositoryMapStorage.Instance.LocalRepository.Remove(e)));
			await Task.WhenAll(Elements.Where(e => toDelete.Contains(new CurrencyMapDbm { Code = e.Code, ParentId = Id })).Select(Remove));

			LastFetch = DateTime.Now;
			return true;
		}
	}
}

