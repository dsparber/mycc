using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Ui.Android.Messages;

namespace MyCC.Ui.Android.Data.Add
{
    public static class AddAccountData
    {
        public static async Task<bool> Add(OnlineAccountRepository repository, Action testingStarted = null, Action alreadyAdded = null, Action testingFailed = null)
        {
            if (AccountStorage.AlreadyExists(repository))
            {
                alreadyAdded?.Invoke();
                return false;
            }

            testingStarted?.Invoke();
            var success = await repository.Test();
            if (!success)
            {
                testingFailed?.Invoke();
                return false;
            }

            await AccountStorage.Instance.Add(repository);
            await repository.FetchOnline();
            // TODO Fetch new Rates
            Messaging.Update.Assets.Send();
            Messaging.Update.Rates.Send(); // Because the rates view shows the currency of every account

            return true;
        }

        public static async Task Add(LocalAccount account)
        {
            await AccountStorage.Instance.LocalRepository.Add(account);
            Messaging.Update.Assets.Send();
            // TODO Fetch new Rates
            Messaging.Update.Rates.Send(); // Because the rates view shows the currency of every account
        }
    }
}