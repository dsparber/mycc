using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.tasks;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.view.overlays;

namespace MyCryptos.Forms.Tasks
{
	public static class AppTaskHelper
	{
		public static async Task FetchMissingRates(List<ExchangeRate> neededRates)
		{
			if (neededRates.Count > 0)
			{
				await ApplicationTasks.FetchMissingRates(neededRates, Messaging.FetchMissingRates.SendStarted, Messaging.FetchMissingRates.SendFinished, ErrorOverlay.Display);
			}
		}

		public static async Task FetchMissingRates()
		{
			var neededRates = AccountStorage.NeededRates;
			await FetchMissingRates(neededRates);
		}

		public static async Task FetchBalancesAndRates()
		{
			await ApplicationTasks.FetchBalancesAndRates(Messaging.UpdatingAccountsAndRates.SendStarted, Messaging.UpdatingAccountsAndRates.SendFinished, ErrorOverlay.Display);
		}

		public static async Task FetchBalanceAndRates(OnlineFunctionalAccount account)
		{
			await ApplicationTasks.FetchBalanceAndRates(account, Messaging.UpdatingAccountsAndRates.SendStarted, Messaging.UpdatingAccountsAndRates.SendFinished, ErrorOverlay.Display);
			await FetchMissingRates(AccountStorage.NeededRatesFor(account));
		}
		public static async Task FetchBalanceAndRates(Currency currency)
		{
			await ApplicationTasks.FetchBalanceAndRates(currency, Messaging.UpdatingAccountsAndRates.SendStarted, Messaging.UpdatingAccountsAndRates.SendFinished, ErrorOverlay.Display);
			await FetchMissingRates(AccountStorage.NeededRatesFor(currency));
		}
	}
}
