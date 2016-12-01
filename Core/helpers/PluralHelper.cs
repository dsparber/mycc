namespace MyCryptos.helpers
{
	public static class PluralHelper
	{
		public static string GetText(string noItems, string oneItem, string manyItems, int count)
		{
			switch (count)
			{
				case 0: return noItems;
				case 1: return oneItem;
				default: return $"{count} {manyItems}";
			}
		}
	}
}
