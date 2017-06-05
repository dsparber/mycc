using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;

namespace MyCC.Ui.Edit
{
    public class EditUtils : IEditUtils
    {
        public async Task Update(FunctionalAccount account)
        {
            await AccountStorage.Update(account);
            UiUtils.Update.CreateAssetsData();
        }

        public async Task Delete(FunctionalAccount account)
        {
            await AccountStorage.Instance.LocalRepository.Remove(account);
            UiUtils.Update.CreateAssetsData();
        }
    }
}