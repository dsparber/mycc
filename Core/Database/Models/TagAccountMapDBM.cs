using System.Threading.Tasks;
using MyCryptos.Core.Database.Interfaces;
using MyCryptos.Core.Models;
using SQLite;

namespace MyCryptos.Core.Database.Models
{
    [Table("TagsAccountsMap")]
    public class TagAccountMapDBM : Persistable<int>, IEntityDBM<TagAccountMapDBM, int>
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public int TagId { get; set; }

        public int AccountId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TagAccountMapDBM))
            {
                return false;
            }
            var t = (TagAccountMapDBM)obj;

            return t.TagId == TagId && t.AccountId == AccountId;
        }

        public override int GetHashCode()
        {
            return TagId.GetHashCode() + AccountId.GetHashCode();
        }

        public Task<TagAccountMapDBM> Resolve()
        {
            return Task.Factory.StartNew(() => { return this; });
        }
    }
}

