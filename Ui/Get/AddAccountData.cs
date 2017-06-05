using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Ui.Messages;

namespace MyCC.Ui.Get
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
            Messaging.Request.DataForNewAccount.Send();

            return true;
        }

        public static async Task Add(LocalAccount account)
        {
            await AccountStorage.Instance.LocalRepository.Add(account);
            Messaging.Request.DataForNewAccount.Send();
        }
    }
}