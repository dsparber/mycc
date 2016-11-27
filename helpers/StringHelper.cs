using System.Linq;

namespace MyCryptos.helpers
{
	public static class StringHelper
	{
		public static string CapitalizeFirstLetter(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}

			return input.First().ToString().ToUpper() + string.Join("", input.Skip(1));
		}
	}
}
