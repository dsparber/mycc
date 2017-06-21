using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;
using Xamarin.Forms;

namespace MyCC.Ui.Edit
{
    internal static class EditAccounts
    {
        public static async Task<bool> Add(OnlineAccountRepository repository)
        {
            if (AccountStorage.AlreadyExists(repository))
            {
                DependencyService.Get<IErrorDialog>().Display(StringUtils.TextResolver.FetchingNoSuccessText);
                return false;
            }

            var success = await repository.Test();
            if (!success)
            {
                DependencyService.Get<IErrorDialog>().Display(StringUtils.TextResolver.VerifyInput);
                return false;
            }

            await AccountStorage.Instance.Add(repository);
            await repository.FetchOnline();
            UiUtils.Update.FetchNeededButNotLoadedRates();
            UiUtils.AssetsRefresh.ResetCache();
            UiUtils.RatesRefresh.ResetCache();
            Messaging.UiUpdate.Assets.Send();
            Messaging.UiUpdate.Rates.Send();

            return true;
        }

        public static async Task Add(LocalAccount account)
        {
            await AccountStorage.Instance.LocalRepository.Add(account);
            UiUtils.Update.FetchNeededButNotLoadedRates();
            UiUtils.AssetsRefresh.ResetCache();
            UiUtils.RatesRefresh.ResetCache();
            Messaging.UiUpdate.Assets.Send();
            Messaging.UiUpdate.Rates.Send();
        }
    }
}