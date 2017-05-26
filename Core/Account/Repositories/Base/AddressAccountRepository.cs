using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;

namespace MyCC.Core.Account.Repositories.Base
{
    public abstract class AddressAccountRepository : OnlineAccountRepository
    {
        public string Address;
        protected virtual decimal BalanceFactor => 1;
        protected virtual HttpContent PostContent => null;

        public abstract string WebUrl { get; }

        protected abstract Uri Url { get; }

        protected abstract Func<string, decimal> Balance { get; }

        public abstract Currency Currency { get; }
        public abstract IEnumerable<Currency> SupportedCurrencies { get; }

        private const int BufferSize = 256000;
        protected readonly HttpClient Client;

        public override string Data => Address;

        protected AddressAccountRepository(int id, string name, string address) : base(id, name)
        {
            Address = address;
            Client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = BufferSize };
        }

        private async Task<decimal?> GetBalance()
        {
            try
            {
                HttpResponseMessage response;
                if (PostContent == null)
                {
                    response = await Client.GetAsync(Url);
                }
                else
                {
                    response = await Client.PostAsync(Url, PostContent);
                }

                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                return Balance(content) / BalanceFactor;
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }


        public override async Task<bool> Test()
        {
            try
            {
                return (await GetBalance()).HasValue;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public sealed override async Task<bool> FetchOnline()
        {

            var balance = await GetBalance();

            if (!balance.HasValue) return false;

            var existing = Elements.FirstOrDefault();
            var money = new Money(balance.Value, Currency);
            var name = Name;

            var newAccount = GetAccount(existing?.Id ?? default(int), name, money, existing?.IsEnabled ?? true);

            if (existing != null)
            {
                await Update(newAccount);
            }
            else
            {
                await Add(newAccount);
            }

            await Task.WhenAll(Elements.Where(a => a.Id != newAccount.Id).Select(async a => await Remove(a)));


            LastFetch = DateTime.Now;
            return true;
        }

        protected abstract FunctionalAccount GetAccount(int? id, string name, Money money, bool isEnabled);

        private static IEnumerable<AddressAccountRepository> Repositories(string name, Currency currency, string address) => new AddressAccountRepository[]
        {
            new BlockchainAccountRepository(default(int), name, address),
            new EthereumAccountRepository(default(int), name, address),
            new EthereumClassicAccountRepository(default(int), name, address),
            new ReddCoinAccountRepository(default(int), name, address),
            new CryptoIdAccountRepository(default(int), name, currency, address),
            new BlockExpertsAccountRepository(default(int), name, currency, address)
        };

        public static AddressAccountRepository CreateAddressAccountRepository(string name, Currency currency, string address)
        {
            if (currency == null || string.IsNullOrWhiteSpace(address)) return null;

            if (Currencies.CurrencyConstants.Btc.Equals(currency) && address.StartsWith("xpub", StringComparison.CurrentCultureIgnoreCase))
            {
                return new BlockchainXpubAccountRepository(default(int), name, address);
            }
            return Repositories(name, currency, address).FirstOrDefault(r => r.SupportedCurrencies.Contains(currency));
        }

        public static IEnumerable<Currency> AllSupportedCurrencies
            => Repositories(null, null, null).SelectMany(r => r.SupportedCurrencies).Distinct();
    }
}
