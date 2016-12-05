using System;
using MyCryptos.Core.Abstract.Models;

namespace MyCryptos.Core.Account.Models.Base
{
    public class Transaction : IPersistableWithParent<string>
    {
        public readonly DateTime Timestamp;
        public readonly Money Money;

        public int ParentId { get; set; }
        public string Id { get; set; }

        public Transaction(string id, DateTime dateTime, Money money, int accountId)
        {
            Timestamp = dateTime;
            Money = money;
            Id = id;
            ParentId = accountId;
        }
    }
}
