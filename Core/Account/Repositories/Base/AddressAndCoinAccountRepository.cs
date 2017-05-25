using System.Diagnostics;
using MyCC.Core.Currencies.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Account.Repositories.Base
{
    public abstract class AddressAndCoinAccountRepository : AddressAccountRepository
    {
        private readonly Currencies.Model.Currency _coin;
        public sealed override Currencies.Model.Currency Currency => _coin;

        public override string Data => JsonConvert.SerializeObject(new KeyData(_coin, Address));

        protected AddressAndCoinAccountRepository(int id, string name, string data) : base(id, name, null)
        {
            _coin = new Currency((string)(JObject.Parse(data)["Coin"] ?? JObject.Parse(data)["coin"])["Id"]);
            Address = (string)JObject.Parse(data)["address"];
        }

        protected AddressAndCoinAccountRepository(int id, string name, Currencies.Model.Currency coin, string address) : base(id, name, address)
        {
            if (coin == null) return;
            _coin = coin;
        }

        private class KeyData
        {

            public string address;
            public Currencies.Model.Currency Coin;

            public KeyData(Currencies.Model.Currency coin, string address)
            {
                Coin = coin;
                this.address = address;
            }
        }

        public sealed override string Description => $"{DescriptionName} ({_coin.Code})";
        public abstract string DescriptionName { get; }
    }
}
