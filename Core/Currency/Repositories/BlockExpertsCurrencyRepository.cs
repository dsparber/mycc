using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Currency.Database;

namespace MyCC.Core.Currency.Repositories
{
    public class BlockExpertsCurrencyRepository : OnlineCurrencyRepository
    {
        public BlockExpertsCurrencyRepository(int id) : base(id) { }
        public override int RepositoryTypeId => CurrencyRepositoryDbm.DbTypeBlockExpertsRepository;

        protected override async Task<IEnumerable<Model.Currency>> GetCurrencies()
        {
            var list = await Task.Factory.StartNew(() =>
             {
                 var currentElements = new List<Model.Currency>
                 {
                    new Model.Currency("FUN", "AlphabetCoinFund", true),
                    new Model.Currency("AMBER", "Ambercoin", true),
                    new Model.Currency("ASW", "AmericanStandardWallet", true),
                    new Model.Currency("ANTI", "Anti Bitcoin", true),
                    new Model.Currency("ARG", "Argentum", true),
                    new Model.Currency("ART", "Artcoin", true),
                    new Model.Currency("ADCN", "Asiadigicoin", true),
                    new Model.Currency("ALX", "AvalonX", true),
                    new Model.Currency("BEEZ", "BeezerCoin", true),
                    new Model.Currency("BLRY", "Billary Coin", true),
                    new Model.Currency("BIOS", "Bios crypto", true),
                    new Model.Currency("BTCD", "BitcoinDark", true),
                    new Model.Currency("XBC", "BitcoinPlus", true),
                    new Model.Currency("BCC", "BitConnect", true),
                    new Model.Currency("BSD", "BitSend", true),
                    new Model.Currency("BITS", "Bitstar", true),
                    new Model.Currency("BEC", "Bluecoin", true),
                    new Model.Currency("CASH", "Cashcoin", true),
                    new Model.Currency("CASHH", "Cashh Coin", true),
                    new Model.Currency("CRPC", "CereiPayCoin", true),
                    new Model.Currency("CHESS", "ChessCoin", true),
                    new Model.Currency("CMC", "CMI", true),
                    new Model.Currency("COC", "CoCreateCoin", true),
                    new Model.Currency("COIN", "COIN", true),
                    new Model.Currency("CPI", "CPICoin", true),
                    new Model.Currency("DICK", "Dickcoin", true),
                    new Model.Currency("DOPE", "Dopecoin", true),
                    new Model.Currency("DVC", "Dovecoin", true),
                    new Model.Currency("ECC", "E-CurrencyCoin", true),
                    new Model.Currency("EBST", "eBoost", true),
                    new Model.Currency("ECN", "eCoin", true),
                    new Model.Currency("ERY", "Eryllium", true),
                    new Model.Currency("ESP", "Espers", true),
                    new Model.Currency("EXC", "EXCoin", true),
                    new Model.Currency("FEDS", "FedoraShare", true),
                    new Model.Currency("FRWC", "Frankywillcoin", true),
                    new Model.Currency("GBRC", "GlobalBusinessRevolution", true),
                    new Model.Currency("XGR", "GoldReserve", true),
                    new Model.Currency("GRT", "Grantcoin", true),
                    new Model.Currency("HCC", "HCCoin", true),
                    new Model.Currency("HEMP", "Hempcoin", true),
                    new Model.Currency("HNC", "HunCoin", true),
                    new Model.Currency("ICOB", "ICOBID", true),
                    new Model.Currency("IGO", "IGOCOIN", true),
                    new Model.Currency("INC", "Incrementum", true),
                    new Model.Currency("ICASH", "Internet Cash", true),
                    new Model.Currency("XJO", "JouleCoin", true),
                    new Model.Currency("KNC", "KhanCoin", true),
                    new Model.Currency("KLC", "Kilocoin", true),
                    new Model.Currency("KUC", "Kuwaitcoin", true),
                    new Model.Currency("LDC", "Landcoin", true),
                    new Model.Currency("LEA", "LeaCoin", true),
                    new Model.Currency("LEO", "LEOcoin", true),
                    new Model.Currency("LIN", "LineCoin", true),
                    new Model.Currency("LKC2", "LinkedCoin", true),
                    new Model.Currency("BASH", "LuckChain", true),
                    new Model.Currency("LKC", "Luckycoin", true),
                    new Model.Currency("LUNA", "LUNA", true),
                    new Model.Currency("MSCN", "Master Swiscoin", true),
                    new Model.Currency("MIU", "MIYU Coin", true),
                    new Model.Currency("MBL", "MobileCash", true),
                    new Model.Currency("MST", "Mustang Coin", true),
                    new Model.Currency("NTCC", "NeptuneClassic", true),
                    new Model.Currency("NTC", "Neptunecoin", true),
                    new Model.Currency("OK", "OKCash", true),
                    new Model.Currency("ORLY", "Orlycoin", true),
                    new Model.Currency("PAPAFR", "PAPAFRANCESCOCoin", true),
                    new Model.Currency("PEC", "PeaceCoin", true),
                    new Model.Currency("POST", "PostCoin", true),
                    new Model.Currency("PVC", "PuraVidaCoin", true),
                    new Model.Currency("RVC", "Revcoin", true),
                    new Model.Currency("RUPEE", "RUPEE", true),
                    new Model.Currency("SFC", "Safecoin", true),
                    new Model.Currency("SH", "Shilling", true),
                    new Model.Currency("SPEC", "SPEC", true),
                    new Model.Currency("SSC", "SSCoin", true),
                    new Model.Currency("SSUC", "SSUCoin", true),
                    new Model.Currency("STPC", "Stepcoin", true),
                    new Model.Currency("SUPER", "Supercoin", true),
                    new Model.Currency("SCN", "Swiscoin", true),
                    new Model.Currency("TCOIN", "T-coin", true),
                    new Model.Currency("TAO", "Tao", true),
                    new Model.Currency("TRNT", "Tarrant", true),
                    new Model.Currency("TEK", "TEKcoin", true),
                    new Model.Currency("TERA", "TeraCoin", true),
                    new Model.Currency("TAGR", "Think and Get Rich Coin", true),
                    new Model.Currency("TRUMP", "TrumpCoin", true),
                    new Model.Currency("UNB", "UnbreakableCoin", true),
                    new Model.Currency("UNC", "UNCoin", true),
                    new Model.Currency("XVC", "Vcash", true),
                    new Model.Currency("VOX", "VOXELS", true),
                    new Model.Currency("VPN", "Vpncoin", true),
                    new Model.Currency("WAY", "WayGuide", true),
                    new Model.Currency("WISC", "WisdomCoin", true),
                    new Model.Currency("XCO", "Xcoin", true),
                    new Model.Currency("MI", "Xiaomicoin", true),
                    new Model.Currency("XPAY", "xPAY", true),
                    new Model.Currency("ZUR", "Zurcoin", true)

                 };


                 return currentElements;
             });
            return list;
        }
    }
}

