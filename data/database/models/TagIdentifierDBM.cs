using SQLite;
using models;
using Xamarin.Forms;
using System.Threading.Tasks;
using data.database.interfaces;

namespace data.database.models
{
	[Table("TagIdentifiers")]
	public class TagIdentifierDBM : IDBM<TagIdentifier>
	{
		public TagIdentifierDBM() { }

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public double ColorR { get; set; }
		public double ColorG { get; set; }
		public double ColorB { get; set; }

		public Task<TagIdentifier> Resolve()
		{
			return Task.Factory.StartNew(() => new TagIdentifier { Id = Id, Name = Name, Color = new Color(ColorR, ColorG, ColorB) });
		}

		public int GetId()
		{
			return Id;
		}

		public void SetId(int id)
		{
			Id = id;
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

