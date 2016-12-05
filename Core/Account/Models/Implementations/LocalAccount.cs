using System;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Models.Base;

namespace MyCryptos.Core.Account.Models.Implementations
{
    public class LocalAccount : FunctionalAccount
    {
        public LocalAccount(int? id, string name, Money money, int repositoryId) : base(id, repositoryId, name, money) { }
    }
}
