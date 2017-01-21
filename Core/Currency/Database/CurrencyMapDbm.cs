using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Abstract.Models;
using SQLite;

namespace MyCC.Core.Currency.Database
{
    [Table("CurrencyMap")]
    public class CurrencyMapDbm : IEntityRepositoryIdDBM<CurrencyMapDbm, string>, IPersistableWithParent<string>
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
            if (obj is CurrencyMapDbm)
            {
                var e = (CurrencyMapDbm)obj;
                return Code.Equals(e.Code) && e.Id == Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() + Id.GetHashCode();
        }
    }
}

