using System;
using MyCC.Core.Account.Models.Base;

namespace MyCC.Core.Account.Models.Implementations
{
    public class LocalAccount : FunctionalAccount
    {
        public LocalAccount(int? id, string name, Money money, bool isEnabled, DateTime lastUpdate, int repositoryId) : base(id, repositoryId, name, money, lastUpdate, isEnabled) { }
    }
}
