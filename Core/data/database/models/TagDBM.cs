using SQLite;
using System.Threading.Tasks;
using data.database.interfaces;
using MyCryptos.models;

namespace data.database.models
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

