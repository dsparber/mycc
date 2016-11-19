using data.database.models;
using MyCryptos.models;
using MyCryptos.resources;
using System;

namespace MyCryptos.data.repositories.account
{
    class CryptoIdAccountRepository : AddressAndCoinAccountRepository
    {
        const string BASE_URL = "http://chainz.cryptoid.info/{0}/api.dws?q=getbalance&a={1}";

        public CryptoIdAccountRepository(string name, string data) : base(AccountRepositoryDBM.DB_TYPE_CRYPTOID_REPOSITORY, name, data) { }

        public CryptoIdAccountRepository(string name, Currency coin, string address) : base(AccountRepositoryDBM.DB_TYPE_CRYPTOID_REPOSITORY, name, coin, address) { }


        protected override string DescriptionName { get { return InternationalisationResources.CryptoId; } }

        protected override Uri GetUrl(Currency currency, string address)
        {
            return new Uri(string.Format(BASE_URL, currency.Code.ToLower(), address));
        }
    }
}
