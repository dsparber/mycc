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
            if (repository == null)
			{
				DependencyService.Get<IErrorDialog>().Display(TextResolver.Instance.VerifyInput);
                return false;
			}
            if (AccountStorage.AlreadyExists(repository))
            {
                DependencyService.Get<IErrorDialog>().Display(TextResolver.Instance.FetchingNoSuccessText);
                return false;
            }

            var success = await repository.Test();
            if (!success)
            {
                DependencyService.Get<IErrorDialog>().Display(TextResolver.Instance.VerifyInput);
                return false;
            }

            await AccountStorage.Instance.Add(repository);
            await repository.FetchOnline();
            UiUtils.Update.FetchNeededButNotLoadedRates();
            UiUtils.AssetsRefresh.ResetCache();
            UiUtils.RatesRefresh.ResetCache();
            Messaging.Modified.Balances.Send();

            return true;
        }

        public static async Task Add(LocalAccount account)
        {
            if (account == null)
            {
                DependencyService.Get<IErrorDialog>().Display(TextResolver.Instance.VerifyInput);
                return;
            }
            await AccountStorage.Instance.LocalRepository.Add(account);
            UiUtils.Update.FetchNeededButNotLoadedRates();
            UiUtils.AssetsRefresh.ResetCache();
            UiUtils.RatesRefresh.ResetCache();
            Messaging.Modified.Balances.Send();
        }
    }
}