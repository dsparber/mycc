using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MyCryptos
{
	public class Account
	{
		public string Name { get; set; }

		public string Id { get; set; }

		public List<Tag> Tags { get; set; }

		public Money Money { get; set; }

		public async Task LoadReferenceValue ()
		{ 
			referenceValue = await CurrencyConverter.convert (Money, Settings.ReferenceCurrency);
			Debug.WriteLine (referenceValue);
		}

		private Money referenceValue { get; set; }

		public Money ReferenceValue {
			get {
				return referenceValue;
			}
		}
	}
}

