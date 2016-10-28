namespace MyCryptos.models
{
	public class Tag : Persistable
	{
		public int? Id { get; set; }

		public TagIdentifier Identifier { get; set; }

		public decimal Units { get; set; }

		public bool Unlimited
		{
			get
			{
				return Units < 0;
			}
			set
			{
				if (value)
				{
					Units = -1;
				}
				else if (Unlimited)
				{
					Units = 0;
				}
			}
		}

		public Tag(TagIdentifier identifier) : this(identifier, -1) { }

		public Tag(TagIdentifier identifier, decimal units) : this(null, identifier, units) { }

		public Tag(int? id, TagIdentifier identifier, decimal units)
		{
			Id = id;
			Units = units;
			Identifier = identifier;
		}
	}
}

