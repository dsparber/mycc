using System;

namespace MyCryptos
{
	public class Tag
	{
		public string Name { get; set; }

		public double Units { get; set; }

		public bool IsUnlimited {
			get { return Units < 0; }
			set {
				if (value)
					Units = -1;
				else
					Units = 0;
			}
		}



		public Tag (string name)
		{
			this.Name = name;
			this.IsUnlimited = true;
		}
	}
}

