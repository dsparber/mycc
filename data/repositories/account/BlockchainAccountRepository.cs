using System;
using data.database.models;
using MyCryptos.models;
using MyCryptos.resources;
using Newtonsoft.Json.Linq;

namespace MyCryptos.data.repositories.account
{
    public class BlockchainAccountRepository : SimpleJsonAccountRepository
    {
        protected override string BaseUrl { get { return "https://blockchain.info/de/address/{0}?format=json&limit=0"; } }
        protected string JsonKeyBalance { get { return "final_balance"; } }
        protected override decimal BalanceFactor { get { return 1e8M; } }
        protected override Currency Currency { get { return Currency.BTC; } }
        public override string AccountName { get { return string.Format("{0} {1}", Name, I18N.Account); } }
        public override string Description { get { return I18N.Blockchain; } }

        public BlockchainAccountRepository(string name, string address) : base(AccountRepositoryDBM.DB_TYPE_BLOCKCHAIN_REPOSITORY, name, address) { }
        public BlockchainAccountRepository(string address) : this(I18N.Blockchain, address) { }

        protected override decimal GetBalance(JObject json)
        {
            return (decimal)json[JsonKeyBalance];
        }
    }
}