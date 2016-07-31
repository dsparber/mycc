using SQLite;
using models;

namespace data.database.models
{
	[Table("Tags")]
	public class TagDBM
	{
		public TagDBM() { }

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public int IdentifierId { get; set; }

		public decimal Units { get; set; }

		public Tag ToTag(TagIdentifier tagIdentifier)
		{
			return new Tag (Id, tagIdentifier, Units);
		}

		public TagDBM(Tag tag)
		{
			Id = tag.Id.Value;
			IdentifierId = tag.Identifier.Id;
			Units = tag.Units;
		}
	}
}

