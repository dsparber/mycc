﻿using MyCryptos.Forms.Resources;

namespace MyCryptos.Forms.helpers
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

		public static string GetTextAccounts(int count) => GetText(I18N.NoAccounts, I18N.OneAccount, I18N.Accounts, count);
		public static string GetTextCoins(int count) => GetText(I18N.NoCoins, I18N.OneCoin, I18N.Coins, count);
		public static string GetTextSourcs(int count) => GetText(I18N.NoSources, I18N.OneSource, I18N.Sources, count);

	}
}
