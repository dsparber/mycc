using System;
using MyCC.Core.Assets.Models;
using MyCC.Core.Assets.Utils;
using SQLite;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace MyCC.Core.Assets.Database
{
    [Table("AssetsSources")]
    internal class AssetsSourceDbm
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [Column("Name")] public string Name { get; set; }
        [Column("Type")] public int Type { get; set; }

        [Column("Amount")] public decimal Amount { get; set; }
        [Column("Currency")] public string CurrencyId { get; set; }
        [Column("Address")] public string Address { get; set; }
        [Column("PublicKey")] public string PublicKey { get; set; }
        [Column("PrivateKey")] public string PrivateKey { get; set; }


        public AssetsSourceDbm()
        {
        }

        public AssetsSourceDbm(AssetsSource source)
        {
            Id = source.Id ?? default(int);
            Name = source.Name;
            CurrencyId = source.CurrencyId;
            Amount = source.Amount ?? default(int);
            Address = source.Address;
            PublicKey = source.PublicKey;
            PrivateKey = source.PrivateKey;
            Type = (int) source.Type;
        }

        public AssetsSource ToAssetsSource()
        {
            var type = (AssetsSourceType) Type;

            switch (type)
            {
                case AssetsSourceType.WithAmount:
                    return AssetsFactory.CreateWithAmount(Id, Name, CurrencyId, Amount);
                case AssetsSourceType.WithAddress:
                    return AssetsFactory.CreateWithAddress(Id, Name, CurrencyId, Address);
                case AssetsSourceType.Bittrex:
                    return AssetsFactory.CreateForBittrex(Id, Name, PublicKey, PrivateKey);
                case AssetsSourceType.Poloniex:
                    return AssetsFactory.CreateForPoloniex(Id, Name, PublicKey, PrivateKey);
                default:
                    throw new ArgumentException("AssetsSourceDbm: Type " + type + " is unknown");
            }
        }
    }
}