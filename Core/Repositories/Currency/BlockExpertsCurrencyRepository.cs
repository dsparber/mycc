using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Models;

namespace MyCryptos.Core.Repositories.Currency
{
    public class BlockExpertsCurrencyRepository : OnlineCurrencyRepository
    {
        public BlockExpertsCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_BLOCK_EXPERTS_REPOSITORY, name) { }

        protected async override Task<IEnumerable<Models.Currency>> GetCurrencies()
        {
            var list = await Task.Factory.StartNew(() =>
             {
                 var currentElements = new List<Models.Currency>();

                 currentElements.Add(new Models.Currency("2GIVE", "2GiveCoin"));
                 currentElements.Add(new Models.Currency("AMBER", "Ambercoin"));
                 currentElements.Add(new Models.Currency("ASW", "AmericanStandardWallet"));
                 currentElements.Add(new Models.Currency("ANTI", "Anti Bitcoin"));
                 currentElements.Add(new Models.Currency("ARC", "AquariusCoin"));
                 currentElements.Add(new Models.Currency("ARG", "Argentum"));
                 currentElements.Add(new Models.Currency("ADCN", "Asiadigicoin"));
                 currentElements.Add(new Models.Currency("ADC", "AudioCoin"));
                 currentElements.Add(new Models.Currency("ALX", "AvalonX"));
                 currentElements.Add(new Models.Currency("BEEZ", "BeezerCoin"));
                 currentElements.Add(new Models.Currency("BLRY", "Billary Coin"));
                 currentElements.Add(new Models.Currency("BIOS", "Bios crypto"));
                 currentElements.Add(new Models.Currency("BTCD", "BitcoinDark"));
                 currentElements.Add(new Models.Currency("XBC", "BitcoinPlus"));
                 currentElements.Add(new Models.Currency("BSD", "BitSend"));
                 currentElements.Add(new Models.Currency("BITS", "Bitstar"));
                 currentElements.Add(new Models.Currency("BLU", "Bluecoin"));
                 currentElements.Add(new Models.Currency("BEC", "Bluecoin"));
                 currentElements.Add(new Models.Currency("CASH", "Cashcoin"));
                 currentElements.Add(new Models.Currency("CPC", "CereiPayCoin"));
                 currentElements.Add(new Models.Currency("CHESS", "ChessCoin"));
                 currentElements.Add(new Models.Currency("CMC", "CMI"));
                 currentElements.Add(new Models.Currency("COC", "CoCreateCoin"));
                 currentElements.Add(new Models.Currency("COIN", "COIN"));
                 currentElements.Add(new Models.Currency("CV2", "Colossuscoin2"));
                 currentElements.Add(new Models.Currency("CPI", "CPICoin"));
                 currentElements.Add(new Models.Currency("CRW", "Crowncoin"));
                 currentElements.Add(new Models.Currency("DICK", "Dickcoin"));
                 currentElements.Add(new Models.Currency("DIME", "Dimecoin"));
                 currentElements.Add(new Models.Currency("DOPE", "Dopecoin"));
                 currentElements.Add(new Models.Currency("DVC", "Dovecoin"));
                 currentElements.Add(new Models.Currency("ECC", "E-CurrencyCoin"));
                 currentElements.Add(new Models.Currency("ESP", "Espers"));
                 currentElements.Add(new Models.Currency("FEDS", "FedoraShare"));
                 currentElements.Add(new Models.Currency("FRWC", "Frankywillcoin"));
                 currentElements.Add(new Models.Currency("GBRC", "GlobalBusinessRevolution"));
                 currentElements.Add(new Models.Currency("XGR", "GoldReserve"));
                 currentElements.Add(new Models.Currency("GRT", "Grantcoin"));
                 currentElements.Add(new Models.Currency("HAM", "HamRadiocoin"));
                 currentElements.Add(new Models.Currency("HCC", "HCCoin"));
                 currentElements.Add(new Models.Currency("HEMP", "Hempcoin"));
                 currentElements.Add(new Models.Currency("HNC", "HunCoin"));
                 currentElements.Add(new Models.Currency("NKA", "IncaKoin"));
                 currentElements.Add(new Models.Currency("INC", "Incrementum"));
                 currentElements.Add(new Models.Currency("ICASH", "Internet Cash"));
                 currentElements.Add(new Models.Currency("ISL", "IslaCoin"));
                 currentElements.Add(new Models.Currency("XJO", "JouleCoin"));
                 currentElements.Add(new Models.Currency("KCC", "K-COIN"));
                 currentElements.Add(new Models.Currency("KNC", "KhanCoin"));
                 currentElements.Add(new Models.Currency("KUC", "Kuwaitcoin"));
                 currentElements.Add(new Models.Currency("LDC", "Landcoin"));
                 currentElements.Add(new Models.Currency("LEA", "LeaCoin"));
                 currentElements.Add(new Models.Currency("LEO", "LEOcoin"));
                 currentElements.Add(new Models.Currency("LKC2", "LinkedCoin"));
                 currentElements.Add(new Models.Currency("LKC", "Luckycoin"));
                 currentElements.Add(new Models.Currency("MBL", "MobileCash"));
                 currentElements.Add(new Models.Currency("MST", "Mustang Coin"));
                 currentElements.Add(new Models.Currency("NTC", "Neptunecoin"));
                 currentElements.Add(new Models.Currency("OK", "OKCash"));
                 currentElements.Add(new Models.Currency("ORLY", "Orlycoin"));
                 currentElements.Add(new Models.Currency("PAPAFR", "PAPAFRANCESCOCoin"));
                 currentElements.Add(new Models.Currency("PEC", "PeaceCoin"));
                 currentElements.Add(new Models.Currency("POST", "PostCoin"));
                 currentElements.Add(new Models.Currency("PVC", "PuraVidaCoin"));
                 currentElements.Add(new Models.Currency("QBK", "Qibuckcoin"));
                 currentElements.Add(new Models.Currency("RVC", "Revcoin"));
                 currentElements.Add(new Models.Currency("SFC", "Safecoin"));
                 currentElements.Add(new Models.Currency("SH", "Shilling"));
                 currentElements.Add(new Models.Currency("SPEC", "SPEC"));
                 currentElements.Add(new Models.Currency("SSC", "SSCoin"));
                 currentElements.Add(new Models.Currency("SSUC", "SSUCoin"));
                 currentElements.Add(new Models.Currency("STP", "Stepcoin"));
                 currentElements.Add(new Models.Currency("SUPER", "Supercoin"));
                 currentElements.Add(new Models.Currency("SCN", "Swiscoin"));
                 currentElements.Add(new Models.Currency("TCOIN", "T-coin"));
                 currentElements.Add(new Models.Currency("TAO", "Tao"));
                 currentElements.Add(new Models.Currency("TRNT", "Tarrant"));
                 currentElements.Add(new Models.Currency("TEK", "TEKcoin"));
                 currentElements.Add(new Models.Currency("TAGR", "Think and Get Rich Coin"));
                 currentElements.Add(new Models.Currency("TRUMP", "TrumpCoin"));
                 currentElements.Add(new Models.Currency("UNB", "UnbreakableCoin"));
                 currentElements.Add(new Models.Currency("UNC", "UNCoin"));
                 currentElements.Add(new Models.Currency("UPC", "UnionPlusCoin"));
                 currentElements.Add(new Models.Currency("XVC", "Vcash"));
                 currentElements.Add(new Models.Currency("VOX", "VOXELS"));
                 currentElements.Add(new Models.Currency("VPN", "Vpncoin"));
                 currentElements.Add(new Models.Currency("WAY", "WayGuide"));
                 currentElements.Add(new Models.Currency("XCO", "Xcoin"));
                 currentElements.Add(new Models.Currency("MI", "Xiaomicoin"));
                 currentElements.Add(new Models.Currency("XPAY", "xPAY"));
                 currentElements.Add(new Models.Currency("YOC", "Yocoin"));
                 currentElements.Add(new Models.Currency("ZUR", "Zurcoin"));

                 return currentElements;
             });
            return list;
        }
    }
}

