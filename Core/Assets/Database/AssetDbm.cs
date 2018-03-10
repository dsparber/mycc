using MyCC.Core.Assets.Models;
using SQLite;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace MyCC.Core.Assets.Database
{
    [Table("Assets")]
    internal class AssetDbm
    {
        [PrimaryKey, Column("_id")] public string Id { get; set; }
        [Column("SourceId")] public int SourceId { get; set; }

        [Column("Amount")] public decimal Amount { get; set; }
        [Column("Currency")] public string CurrencyId { get; set; }

        public AssetDbm()
        {
        }

        public AssetDbm(Asset asset)
        {
            Id = $"{asset.Source.Id}_{asset.CurrencyId}";
            SourceId = asset.Source.Id ?? default(int);
            Amount = asset.Amount;
            CurrencyId = asset.CurrencyId;
        }

        public Asset ToAssets(AssetsSource source) => new Asset(source, Amount, CurrencyId);
    }
}