using System;
using SQLite;

namespace MyCC.Core.CoinInfo
{
    [Table("CoinInfo")]
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
        /// Gets or sets the coin supply. The coin supply is the total amount of coins available.
        /// </summary>
        /// <value>The coin supply.</value>
        [Column("CoinSupply")]
        public decimal? CoinSupply { get; set; }

        /// <summary>
        /// Gets or sets the max. coin supply. The max. coin supply is the total amount of coins possible.
        /// </summary>
        /// <value>The max. coin supply.</value>
        [Column("MaxSupply")]
        public decimal? MaxCoinSupply { get; set; }

        /// <summary>
        /// Gets or sets the block height. The block height represents the number of blocks existing within the chain.
        /// </summary>
        /// <value>The block height.</value>
        [Column("Height")]
        public int? BlockHeight { get; set; }

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

        /// <summary>
        /// Gets or sets the blockreward.
        /// </summary>
        /// <value>The blockreward.</value>
        [Column("Blockreward")]
        public decimal? Blockreward { get; set; }

        [Column("LastUpdate")]
        public long LastUpdateTicks { get; set; }

        [Ignore]
        public DateTime LastUpdate
        {
            get { return new DateTime(LastUpdateTicks); }
            set { LastUpdateTicks = value.Ticks; }
        }


        public CoinInfoData(Currencies.Model.Currency currency) : this(currency.Code) { }

        private CoinInfoData(string currency)
        {
            CurrencyCode = currency;
        }

        public CoinInfoData() { }

        public override bool Equals(object obj)
        {
            return CurrencyCode.Equals((obj as CoinInfoData)?.CurrencyCode);
        }

        public override int GetHashCode() => 1;

        /// <summary>
        /// Returns an updated info object, where the new informations are added to the existing
        /// </summary>
        /// <returns>The updated info.</returns>
        /// <param name="info">The new info data.</param>
        public CoinInfoData AddUpdate(CoinInfoData info)
        {
            return new CoinInfoData
            {
                CurrencyCode = info.CurrencyCode ?? CurrencyCode,
                Algorithm = info.Algorithm ?? Algorithm,
                Blocktime = info.Blocktime ?? Blocktime,
                CoinSupply = info.CoinSupply ?? CoinSupply,
                Difficulty = info.Difficulty ?? Difficulty,
                Hashrate = info.Hashrate ?? Hashrate,
                BlockHeight = info.BlockHeight ?? BlockHeight,
                IsProofOfWork = info.IsProofOfWork ?? IsProofOfWork,
                IsProofOfStake = info.IsProofOfStake ?? IsProofOfStake,
                Blockreward = info.Blockreward ?? Blockreward,
                MaxCoinSupply = info.MaxCoinSupply ?? MaxCoinSupply,
                LastUpdate = info.LastUpdate
            };
        }

        public override string ToString()
        {
            return $"[CoinInfoData: CurrencyCode={CurrencyCode}, Algorithm={Algorithm}, Hashrate={Hashrate}, Difficulty={Difficulty}, CoinSupply={CoinSupply}, MaxCoinSupply={MaxCoinSupply}, BlockHeight={BlockHeight}, IsProofOfStake={IsProofOfStake}, IsProofOfWork={IsProofOfWork}, Blocktime={Blocktime}]";
        }
    }
}
