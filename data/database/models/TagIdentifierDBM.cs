using SQLite;
using models;

namespace data.database.models
{
	[Table("TagIdentifiers")]
	public class TagIdentifierDBM
	{
		public TagIdentifierDBM() { }

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public TagIdentifier ToTagIdentifier()
		{
			return new TagIdentifier { Id = Id, Name = Name };
		}

		public TagIdentifierDBM(TagIdentifier identifier)
		{
			Id = identifier.Id;
			Name = identifier.Name;
		}
	}
}

