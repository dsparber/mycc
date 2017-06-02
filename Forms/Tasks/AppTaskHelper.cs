using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Tasks;
using MyCC.Forms.Messages;
using MyCC.Forms.View.Overlays;

namespace MyCC.Forms.Tasks
{
    public static class AppTaskHelper
    {
        public static async Task FetchMissingRates(double progressSpan = 1, double progressStart = 0)
        {
            await ApplicationTasks.FetchMissingRates(Messaging.FetchMissingRates.SendStarted, Messaging.FetchMissingRates.SendFinished, ErrorOverlay.Display, d => Messaging.Progress.Send(progressStart + progressSpan * d));
        }

        public static async Task FetchBalancesAndRates()
        {
            await ApplicationTasks.FetchAccounts(Messaging.UpdatingAccounts.SendStarted, Messaging.UpdatingAccounts.SendFinished, ErrorOverlay.Display, d => Messaging.Progress.Send(0.5 * d));
            await ApplicationTasks.FetchRates(Messaging.UpdatingRates.SendStarted, Messaging.UpdatingRates.SendFinished, ErrorOverlay.Display, d => Messaging.Progress.Send(0.5 + d * 0.4));
            Messaging.Progress.Send(1);

        }

        public static async Task FetchBalanceAndRates(FunctionalAccount account)
        {
            Messaging.Progress.Send(0.1);
            await ApplicationTasks.FetchBalance(account, Messaging.UpdatingAccountsAndRates.SendStarted, Messaging.UpdatingAccountsAndRates.SendFinished, ErrorOverlay.Display);
            await ApplicationTasks.FetchRates(account, Messaging.UpdatingAccountsAndRates.SendStarted, Messaging.UpdatingAccountsAndRates.SendFinished, ErrorOverlay.Display, d => Messaging.Progress.Send(0.3 + 0.4 * d));
            Messaging.Progress.Send(1);
        }

        public static async Task FetchBalanceAndRates(Currency currency)
        {
            Messaging.Progress.Send(0.1);
            await ApplicationTasks.FetchBalance(currency, Messaging.UpdatingAccountsAndRates.SendStarted, Messaging.UpdatingAccountsAndRates.SendFinished, ErrorOverlay.Display, d => Messaging.Progress.Send(d * 0.4));
            await ApplicationTasks.FetchRates(currency, Messaging.UpdatingAccountsAndRates.SendStarted, Messaging.UpdatingAccountsAndRates.SendFinished, ErrorOverlay.Display, d => Messaging.Progress.Send(0.4 + 0.3 * d));
            Messaging.Progress.Send(1);
        }


        public static async Task UpdateRates()
        {
            await ApplicationTasks.FetchRates(Messaging.UpdatingRates.SendStarted, Messaging.UpdatingRates.SendFinished, ErrorOverlay.Display, d => Messaging.Progress.Send(d * 0.8));
            Messaging.Progress.Send(1);
        }

        public static async Task FetchBtcUsdRates()
        {
            await ApplicationTasks.FetchBitcoinDollarRates(Messaging.UpdatingRates.SendStarted, Messaging.UpdatingRates.SendFinished, ErrorOverlay.Display, Messaging.Progress.Send);
        }

        public static async Task FetchCoinDetails(Currency currency)
        {
            Messaging.Progress.Send(0.2);
            await ApplicationTasks.FetchCoinInfo(currency, Messaging.FetchingCoinInfo.SendStarted, Messaging.FetchingCoinInfo.SendFinished, ErrorOverlay.Display);
            Messaging.Progress.Send(0.4);
            await ApplicationTasks.FetchRates(currency, Messaging.UpdatingAccountsAndRates.SendStarted, Messaging.UpdatingAccountsAndRates.SendFinished, ErrorOverlay.Display, d => Messaging.Progress.Send(0.4 + d * 0.6));
            Messaging.Progress.Send(1);

        }

        public static async Task FetchCoinInfo(Currency currency)
        {
            Messaging.Progress.Send(0.3);
            await ApplicationTasks.FetchCoinInfo(currency, Messaging.FetchingCoinInfo.SendStarted, Messaging.FetchingCoinInfo.SendFinished, ErrorOverlay.Display);
            Messaging.Progress.Send(1);

        }
    }
}
