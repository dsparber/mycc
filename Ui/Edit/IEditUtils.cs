using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;

namespace MyCC.Ui.Edit
{
    public interface IEditUtils
    {
        Task Update(FunctionalAccount account);
        Task Delete(FunctionalAccount account);
    }
}