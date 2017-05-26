using System.Diagnostics.CodeAnalysis;
using MyCC.Core.Currencies.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Account.Repositories.Base
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public abstract class AddressAndCoinAccountRepository : AddressAccountRepository
    {
        private readonly Currency _coin;
        public sealed override Currency Currency => _coin;

        public override string Data => JsonConvert.SerializeObject(new KeyData(_coin, Address));

        protected AddressAndCoinAccountRepository(int id, string name, string data) : base(id, name, null)
        {
            _coin = new Currency((string)(JObject.Parse(data)["Coin"] ?? JObject.Parse(data)["coin"])["Id"]);
            Address = (string)JObject.Parse(data)["address"];
        }

        protected AddressAndCoinAccountRepository(int id, string name, Currency coin, string address) : base(id, name, address)
        {
            if (coin == null) return;
            _coin = coin;
        }

        private class KeyData
        {

            // ReSharper disable once NotAccessedField.Local
            public string address;
            // ReSharper disable once NotAccessedField.Local
            public Currency Coin;

            public KeyData(Currency coin, string address)
            {
                Coin = coin;
                this.address = address;
            }
        }

        public sealed override string Description => $"{DescriptionName} ({_coin.Code})";
        public abstract string DescriptionName { get; }
    }
}
