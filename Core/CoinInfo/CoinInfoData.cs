using SQLite;

namespace MyCryptos.Core.CoinInfo
{
	[Table("CoinInfos")]
	public class CoinInfoData
	{
		/// <summary>
		/// The Identifier of the coin info data. Referes to the code of the related currency.
		/// </summary>
		/// <value>The currency code</value>
		[PrimaryKey, Column("Code")]
		public string CurrencyCode { get; set; }

		/// <summary>
		/// The algorithm used for accepting blocks.
		/// </summary>
		/// <value>The algorithm</value>
		[Column("Algorithm")]
		public string Algorithm { get; set; }

		/// <summary>
		/// The current hash rate of the coin. Given as GHash/s
		/// </summary>
		/// <value>The hashrate (GHash/s)</value>
		[Column("Hashrate")]
		public decimal? Hashrate { get; set; }

		/// <summary>
		/// Gets or sets the difficulty.
		/// </summary>
		/// <value>The difficulty.</value>
		[Column("Difficulty")]
		public decimal? Difficulty { get; set; }

		/// <summary>
		/// Gets or sets the coin supply. The coin supply is the total amount of coins available
		/// </summary>
		/// <value>The coin supply.</value>
		[Column("CoinSupply")]
		public decimal? CoinSupply { get; set; }

		/// <summary>
		/// Gets or sets the block height. The block height represents the number of blocks existing within the chain.
		/// </summary>
		/// <value>The height.</value>
		[Column("Height")]
		public int? Height { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this coin is working as proof of stake.
		/// </summary>
		/// <value><c>true</c> if the coin is proof of stake; otherwise, <c>false</c>.</value>
		[Column("PoS")]
		public bool? IsProofOfStake { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this coin is working as proof of work.
		/// </summary>
		/// <value><c>true</c> if the coin is proof of work; otherwise, <c>false</c>.</value>
		[Column("PoW")]
		public bool? IsProofOfWork { get; set; }

		/// <summary>
		/// Gets or sets the blocktime. This is the timespan between two blocks being accepted. The time is measured in seconds
		/// </summary>
		/// <value>The blocktime in seconds</value>
		[Column("Blocktime")]
		public decimal? Blocktime { get; set; }


		public CoinInfoData(Currency.Model.Currency currency) : this(currency.Code) { }

		public CoinInfoData(string currency)
		{
			CurrencyCode = currency;
		}

		public CoinInfoData() { }

		public override bool Equals(object obj)
		{
			return CurrencyCode.Equals((obj as CoinInfoData)?.CurrencyCode);
		}

		public override int GetHashCode()
		{
			return CurrencyCode.GetHashCode();
		}
	}
}
