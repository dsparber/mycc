using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;
using Xamarin.Forms;

namespace MyCC.Ui.Edit
{
    public static class EditRepository
    {
        public static async Task Update(OnlineAccountRepository repository, string newAddress, string newCurrencyId, string newName, Dictionary<int, bool> newEnabledStates, Action onError = null)
        {
            // Test if data is valid
            var addressRepo = repository as AddressAccountRepository;
            if (addressRepo != null && (!addressRepo.Address.Equals(newAddress) || !addressRepo.Currency.Id.Equals(newCurrencyId)))
            {
                var testRepo = AddressAccountRepository.CreateAddressAccountRepository(addressRepo.Name, newCurrencyId.Find(), newAddress ?? string.Empty);

                if (testRepo == null || !await testRepo.Test())
                {
                    onError?.Invoke();
                    DependencyService.Get<IErrorDialog>().Display(StringUtils.TextResolver.FetchingNoSuccessText);
                    return;
                }
                if (!addressRepo.Currency.Id.Equals(newCurrencyId))
                {
                    await AccountStorage.Instance.Remove(repository);
                    await testRepo.FetchOnline();
                    await AccountStorage.Instance.Add(testRepo);
                }
                testRepo.Id = repository.Id;
                repository = testRepo;
                await repository.FetchOnline();
            }

            // Apply name and enabled status
            repository.Name = newName;
            foreach (var a in repository.Elements)
            {
                a.Name = repository.Name;
                if (newEnabledStates.ContainsKey(a.Id))
                {
                    a.IsEnabled = newEnabledStates[a.Id];
                }
            }

            // Save changes
            await AccountStorage.Instance.Update(repository);
            foreach (var a in repository.Elements.ToList())
            {
                await AccountStorage.Update(a);
            }
            UiUtils.AssetsRefresh.ResetCache();
            Messaging.Update.Balances.Send();
        }
    }
}