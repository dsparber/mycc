using System.Threading.Tasks;
using MyCryptos.Core.Database.Interfaces;
using MyCryptos.Core.Models;
using SQLite;

namespace MyCryptos.Core.Database.Models
{
    [Table("Tags")]
    public class TagDBM : IEntityDBM<Tag, int>
    {
        public TagDBM() { }

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public int IdentifierId { get; set; }

        public decimal Units { get; set; }

        public async Task<Tag> Resolve()
        {
            var db = new TagIdentifierDatabase();
            return new Tag(Id, await db.Get(Id), Units);
        }

        public int GetId()
        {
            return Id;
        }

        public void SetId(int id)
        {
            Id = id;
        }

        public TagDBM(Tag tag)
        {
            Id = tag.Id;
            IdentifierId = tag.Identifier.Id;
            Units = tag.Units;
        }


    }
}

