using data.database.models;
using data.storage;
using MyCryptos.models;
using MyCryptos.resources;
using Newtonsoft.Json.Linq;

namespace MyCryptos.data.repositories.account
{
    public class EthereumAccountRepository : SimpleJsonAccountRepository
    {
        const string JsonKeyBalance = "balance";
        const string JsonKeyData = "data";

        protected override string BaseUrl { get { return "https://etherchain.org/api/account/{0}"; } }
        protected override decimal BalanceFactor { get { return 1e18M; } }
        protected override Currency Currency { get { return CurrencyStorage.Instance.AllElements.Find(c => c.Code.Equals("ETH")); } }
        public override string AccountName { get { return string.Format("{0} {1}", Name, I18N.Account); } }
        public override string Description { get { return I18N.Etherchain; } }

        public EthereumAccountRepository(string name, string address) : base(AccountRepositoryDBM.DB_TYPE_ETHEREUM_REPOSITORY, name, address) { }
        public EthereumAccountRepository(string address) : this(I18N.Etherchain, address) { }

        protected override decimal GetBalance(JObject json)
        {
            var data = json[JsonKeyData] as JArray;
            return (decimal)(data[0] as JObject)[JsonKeyBalance];
        }
    }
}
