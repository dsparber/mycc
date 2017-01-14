﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.CoinInfo.Repositories;
using SQLite;
using Xamarin.Forms;

namespace MyCryptos.Core.CoinInfo
{
	public class CoinInfoStorage
	{
		public readonly List<CoinInfoData> Elements;

		private readonly SQLiteAsyncConnection _dbConnection;

		private readonly List<ICoinInfoRepository> _repositories;

		private CoinInfoStorage()
		{
			Elements = new List<CoinInfoData>();

			_dbConnection = DependencyService.Get<ISQLiteConnection>().GetConnection();


			Task.Run(async () =>
			{
				await _dbConnection.CreateTableAsync<CoinInfoData>();
				Elements.AddRange(await _dbConnection.Table<CoinInfoData>().ToListAsync());
			});

			_repositories = new List<ICoinInfoRepository> {
				new CryptoIdCoinInfoRepository(),
				new BlockExpertsCoinInfoRepository(),
				new BlockchainCoinInfoRepository(),
				new BlockrCoinInfoRepository()
			};
		}

		public async Task<CoinInfoData> FetchInfo(Currency.Model.Currency currency)
		{
			var info = new CoinInfoData(currency);

			foreach (var r in GetExplorer(currency))
			{
				info = info.AddUpdate(await r.GetInfo(currency));
			}

			if (Elements.Contains(info))
			{
				Elements.Remove(info);
				await _dbConnection.UpdateAsync(info);
			}
			else {
				await _dbConnection.InsertAsync(info);
			}
			Elements.Add(info);

			return info;
		}

		public List<ICoinInfoRepository> GetExplorer(Currency.Model.Currency currency) => _repositories.Where(r => r.SupportedCoins.Contains(currency)).ToList();

		public bool Contains(Currency.Model.Currency currency) => Elements.Any(e => string.Equals(e.CurrencyCode, currency.Code));

		public CoinInfoData Get(Currency.Model.Currency currency) => Elements.Find(e => string.Equals(e.CurrencyCode, currency.Code));

		public static CoinInfoStorage Instance = new CoinInfoStorage();
	}
}
