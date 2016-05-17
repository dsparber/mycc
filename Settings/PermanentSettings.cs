using System;

namespace MyCryptos
{
	public class PermanentSettings
	{
		private static PermanentSettings instance{ get; set; }

		public static PermanentSettings Instance {
			get {
				if (instance == null) {
					instance = new PermanentSettings ();
				}
				return instance;
			}
		}

		private PermanentSettings ()
		{
			ReferenceCurrency = Currency.EUR;
		}

		public Currency ReferenceCurrency{ get; set; }
	}
}

