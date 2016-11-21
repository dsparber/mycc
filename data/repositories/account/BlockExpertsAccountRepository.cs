using System;
using data.database.models;
using MyCryptos.models;
using MyCryptos.resources;

using MyCryptos.data.repositories.account;

namespace data.repositories.account
{
    public class BlockExpertsAccountRepository : AddressAndCoinAccountRepository
    {
        readonly string address;
        Currency coin;

        const string BASE_URL = "https://www.blockexperts.com/api?coin={0}&action=getbalance&address={1}";

        public BlockExpertsAccountRepository(string name, string data) : base(AccountRepositoryDBM.DB_TYPE_BLOCK_EXPERTS_REPOSITORY, name, data) { }

        public BlockExpertsAccountRepository(string name, Currency coin, string address) : base(AccountRepositoryDBM.DB_TYPE_BLOCK_EXPERTS_REPOSITORY, name, coin, address) { }

        protected override Uri GetUrl(Currency currency, string address)
        {
            return new Uri(string.Format(BASE_URL, currency.Code, address));
        }

        protected override string DescriptionName { get { return I18N.BlockExperts; } }
    }
}