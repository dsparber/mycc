using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Currencies.Model;

namespace MyCC.Core.Currencies.Sources
{
    public class BlockExpertsCurrencySource : ICurrencySource
    {

        public string Name => "BlockExperts";


        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            var list = await Task.Factory.StartNew(() =>
             {
                 var currentElements = new List<Currency>
                 {
                    new Currency("FUN", "AlphabetCoinFund", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("AMBER", "Ambercoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ASW", "AmericanStandardWallet", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ANTI", "Anti Bitcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ARG", "Argentum", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ART", "Artcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ADCN", "Asiadigicoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ALX", "AvalonX", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("BEEZ", "BeezerCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("BLRY", "Billary Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("BIOS", "Bios crypto", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("BTCD", "BitcoinDark", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("XBC", "BitcoinPlus", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("BCC", "BitConnect", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("BSD", "BitSend", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("BITS", "Bitstar", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("BEC", "Bluecoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("CASH", "Cashcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("CASHH", "Cashh Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("CRPC", "CereiPayCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("CHESS", "ChessCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("CMC", "CMI", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("COC", "CoCreateCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("COIN", "COIN", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("CPI", "CPICoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("DICK", "Dickcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("DOPE", "Dopecoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("DVC", "Dovecoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ECC", "E-CurrencyCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("EBST", "eBoost", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ECN", "eCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ERY", "Eryllium", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ESP", "Espers", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("EXC", "EXCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("FEDS", "FedoraShare", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("FRWC", "Frankywillcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("GBRC", "GlobalBusinessRevolution", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("XGR", "GoldReserve", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("GRT", "Grantcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("HCC", "HCCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("HEMP", "Hempcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("HNC", "HunCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ICOB", "ICOBID", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("IGO", "IGOCOIN", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("INC", "Incrementum", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ICASH", "Internet Cash", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("XJO", "JouleCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("KNC", "KhanCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("KLC", "Kilocoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("KUC", "Kuwaitcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("LDC", "Landcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("LEA", "LeaCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("LEO", "LEOcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("LIN", "LineCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("LKC2", "LinkedCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("BASH", "LuckChain", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("LKC", "Luckycoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("LUNA", "LUNA", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("MSCN", "Master Swiscoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("MIU", "MIYU Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("MBL", "MobileCash", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("MST", "Mustang Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("NTCC", "NeptuneClassic", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("NTC", "Neptunecoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("OK", "OKCash", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ORLY", "Orlycoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("PAPAFR", "PAPAFRANCESCOCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("PEC", "PeaceCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("POST", "PostCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("PVC", "PuraVidaCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("RVC", "Revcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("RUPEE", "RUPEE", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("SFC", "Safecoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("SH", "Shilling", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("SPEC", "SPEC", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("SSC", "SSCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("SSUC", "SSUCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("STPC", "Stepcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("SUPER", "Supercoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("SCN", "Swiscoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("TCOIN", "T-coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("TAO", "Tao", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("TRNT", "Tarrant", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("TEK", "TEKcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("TERA", "TeraCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("TAGR", "Think and Get Rich Coin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("TRUMP", "TrumpCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("UNB", "UnbreakableCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("UNC", "UNCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("XVC", "Vcash", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("VOX", "VOXELS", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("VPN", "Vpncoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("WAY", "WayGuide", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("WISC", "WisdomCoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("XCO", "Xcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("MI", "Xiaomicoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("XPAY", "xPAY", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts},
                    new Currency("ZUR", "Zurcoin", true) { BalanceSourceFlags = CurrencyConstants.FlagBlockExperts}
                    };
                 return currentElements;
             });
            return list;
        }
    }
}

