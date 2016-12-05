using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.Abstract.Models;
using SQLite;

namespace MyCryptos.Core.Currency.Database
{
    [Table("CurrencyRepositoryMapElements")]
    public class CurrencyMapDBM : IEntityRepositoryIdDBM<CurrencyMapDBM, string>, IPersistableWithParent<string>
    {
        [PrimaryKey, Column("_id")]
        public string Id
        {
            get { return Code + ParentId; }
            set { }
        }

        public string Code { get; set; }

        public int ParentId { get; set; }

        public Task<CurrencyMapDBM> Resolve()
        {
            return Task.Factory.StartNew(() => this);
        }

        public override bool Equals(object obj)
        {
            if (obj is CurrencyMapDBM)
            {
                var e = (CurrencyMapDBM)obj;
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

