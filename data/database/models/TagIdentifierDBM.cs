using SQLite;
using models;
using Xamarin.Forms;

namespace data.database.models
{
	[Table("TagIdentifiers")]
	public class TagIdentifierDBM
	{
		public TagIdentifierDBM() { }

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public double ColorR { get; set; }
		public double ColorG { get; set; }
		public double ColorB { get; set; }

		public TagIdentifier ToTagIdentifier()
		{
			return new TagIdentifier { Id = Id, Name = Name , Color = new Color(ColorR, ColorG, ColorB)};
		}

		public TagIdentifierDBM(TagIdentifier identifier)
		{
			Id = identifier.Id;
			Name = identifier.Name;
			ColorR = identifier.Color.R;
			ColorG = identifier.Color.G;
			ColorB = identifier.Color.B;
		}
	}
}

