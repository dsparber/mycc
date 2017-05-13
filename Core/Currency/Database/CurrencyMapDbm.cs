using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Abstract.Models;
using SQLite;

namespace MyCC.Core.Currency.Database
{
    [Table("CurrencyMap")]
    public class CurrencyMapDbm : IEntityRepositoryIdDbm<CurrencyMapDbm, string>, IPersistableWithParent<string>
    {
        [PrimaryKey, Column("_id")]
        public string Id
        {
            get { return Code + ParentId; }
            set { }
        }

        [Column("Code")]
        public string Code { get; set; }

        [Column("CurrencyRepository")]
        public int ParentId { get; set; }

        public Task<CurrencyMapDbm> Resolve()
        {
            return Task.Factory.StartNew(() => this);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CurrencyMapDbm)) return false;

            var e = (CurrencyMapDbm)obj;
            return string.Equals(Code, e.Code) && ParentId == e.ParentId;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

