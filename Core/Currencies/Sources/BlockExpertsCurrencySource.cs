using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Resources;

namespace MyCC.Core.Currencies.Sources
{
    public class BlockExpertsCurrencySource : ICurrencySource
    {

        public string Name => ConstantNames.BlockExperts;
        public IEnumerable<int> Flags => new[] { CurrencyConstants.FlagBlockExperts };


        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            var list = await Task.Factory.StartNew(() =>
             {
                 var currentElements = new List<Currency>
                 {
                     new Currency("2GIVE", "2GiveCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("FUN", "AlphabetCoinFund", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("AMBER", "Ambercoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("AMY", "Amygws", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("ABZC", "AsiaBizCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("ATT", "aTTis", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("ALX", "AvalonX", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("BEEZ", "BeezerCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("BEST", "BestChain", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("BTCD", "BitcoinDark", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("XBL", "Bitcoinlite", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("XBC", "BitcoinPlus", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("BCC", "BitConnect", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("BTM", "Bitmark", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("BOG", "Bogcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("CASH", "CashCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("4CHN", "ChanCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("CMX", "CoinMiningIndex", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("NOTE", "DNotes", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("DORF", "Dorfcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("EBST", "eBoost", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("ECN", "eCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("ENV", "Environ", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("ERY", "Eryllium", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("GBI", "GBICoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("GLT", "Globaltoken", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("GFC", "Guficoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("MGLC", "Gulfcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("HPC", "Happycoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("HAR", "HaraCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("HMC", "Harmony Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("THC", "Hempcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("ICOB", "ICOBID", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("ICASH", "Internet Cash", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("KASHH", "Kashh Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("KLC", "Kilocoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("KDC", "Kingdom Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("KRS", "Krypstal", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("LDC", "Landcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("LEA", "LeaCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("BASH", "LuckChain", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("LUNA", "LUNA", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("MARX", "Marx Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("MSCN", "Master Swiscoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("MCC", "MercyCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("MIU", "MIYU Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("MST", "Mustang Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("MBC", "My Big Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("PEC", "PeaceCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("PFTC", "Profitcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("PVC", "PuraVidaCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("RDC", "Round Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("RUP", "RUPEE", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("SAC", "SACoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("SH", "Shilling", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("SFC", "Solarflarecoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("STCN", "StakeCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("STDC", "StandardCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("STPC", "Stepcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("SIC", "SWISSCOIN", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("TCOIN", "T-coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("TAO", "Tao", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("TERA", "TeraCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("MAY", "Theresa May coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("TOA", "TOA Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("TRUMP", "TrumpCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("TYC", "TyroCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("ULA", "UlaTech", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("VUC", "VirtaUniqueCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("VOX", "VOXELS", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("VASH", "Vpncoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("WJK", "WojakCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("LOG", "Woodcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("XCO", "Xcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("YASH", "YashCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                     new Currency("ZUR", "Zurcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts}
                    };
                 return currentElements;
             });
            return list;
        }
    }
}

