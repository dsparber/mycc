using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Currency.Database;

namespace MyCC.Core.Currency.Repositories
{
    public class BlockExpertsCurrencyRepository : OnlineCurrencyRepository
    {
        public BlockExpertsCurrencyRepository(int id) : base(id) { }
        public override int RepositoryTypeId => CurrencyRepositoryDbm.DB_TYPE_BLOCK_EXPERTS_REPOSITORY;

        protected override async Task<IEnumerable<Model.Currency>> GetCurrencies()
        {
            var list = await Task.Factory.StartNew(() =>
             {
                 var currentElements = new List<Model.Currency>();

                 currentElements.Add(new Model.Currency("2GIVE", "2GiveCoin"));
                 currentElements.Add(new Model.Currency("AMBER", "Ambercoin"));
                 currentElements.Add(new Model.Currency("ASW", "AmericanStandardWallet"));
                 currentElements.Add(new Model.Currency("ANTI", "Anti Bitcoin"));
                 currentElements.Add(new Model.Currency("ARC", "AquariusCoin"));
                 currentElements.Add(new Model.Currency("ARG", "Argentum"));
                 currentElements.Add(new Model.Currency("ADCN", "Asiadigicoin"));
                 currentElements.Add(new Model.Currency("ADC", "AudioCoin"));
                 currentElements.Add(new Model.Currency("ALX", "AvalonX"));
                 currentElements.Add(new Model.Currency("BEEZ", "BeezerCoin"));
                 currentElements.Add(new Model.Currency("BLRY", "Billary Coin"));
                 currentElements.Add(new Model.Currency("BIOS", "Bios crypto"));
                 currentElements.Add(new Model.Currency("BTCD", "BitcoinDark"));
                 currentElements.Add(new Model.Currency("XBC", "BitcoinPlus"));
                 currentElements.Add(new Model.Currency("BSD", "BitSend"));
                 currentElements.Add(new Model.Currency("BITS", "Bitstar"));
                 currentElements.Add(new Model.Currency("BLU", "Bluecoin"));
                 currentElements.Add(new Model.Currency("BEC", "Bluecoin"));
                 currentElements.Add(new Model.Currency("CASH", "Cashcoin"));
                 currentElements.Add(new Model.Currency("CPC", "CereiPayCoin"));
                 currentElements.Add(new Model.Currency("CHESS", "ChessCoin"));
                 currentElements.Add(new Model.Currency("CMC", "CMI"));
                 currentElements.Add(new Model.Currency("COC", "CoCreateCoin"));
                 currentElements.Add(new Model.Currency("COIN", "COIN"));
                 currentElements.Add(new Model.Currency("CV2", "Colossuscoin2"));
                 currentElements.Add(new Model.Currency("CPI", "CPICoin"));
                 currentElements.Add(new Model.Currency("CRW", "Crowncoin"));
                 currentElements.Add(new Model.Currency("DICK", "Dickcoin"));
                 currentElements.Add(new Model.Currency("DIME", "Dimecoin"));
                 currentElements.Add(new Model.Currency("DOPE", "Dopecoin"));
                 currentElements.Add(new Model.Currency("DVC", "Dovecoin"));
                 currentElements.Add(new Model.Currency("ECC", "E-CurrencyCoin"));
                 currentElements.Add(new Model.Currency("ESP", "Espers"));
                 currentElements.Add(new Model.Currency("FEDS", "FedoraShare"));
                 currentElements.Add(new Model.Currency("FRWC", "Frankywillcoin"));
                 currentElements.Add(new Model.Currency("GBRC", "GlobalBusinessRevolution"));
                 currentElements.Add(new Model.Currency("XGR", "GoldReserve"));
                 currentElements.Add(new Model.Currency("GRT", "Grantcoin"));
                 currentElements.Add(new Model.Currency("HAM", "HamRadiocoin"));
                 currentElements.Add(new Model.Currency("HCC", "HCCoin"));
                 currentElements.Add(new Model.Currency("HEMP", "Hempcoin"));
                 currentElements.Add(new Model.Currency("HNC", "HunCoin"));
                 currentElements.Add(new Model.Currency("NKA", "IncaKoin"));
                 currentElements.Add(new Model.Currency("INC", "Incrementum"));
                 currentElements.Add(new Model.Currency("ICASH", "Internet Cash"));
                 currentElements.Add(new Model.Currency("ISL", "IslaCoin"));
                 currentElements.Add(new Model.Currency("XJO", "JouleCoin"));
                 currentElements.Add(new Model.Currency("KCC", "K-COIN"));
                 currentElements.Add(new Model.Currency("KNC", "KhanCoin"));
                 currentElements.Add(new Model.Currency("KUC", "Kuwaitcoin"));
                 currentElements.Add(new Model.Currency("LDC", "Landcoin"));
                 currentElements.Add(new Model.Currency("LEA", "LeaCoin"));
                 currentElements.Add(new Model.Currency("LEO", "LEOcoin"));
                 currentElements.Add(new Model.Currency("LKC2", "LinkedCoin"));
                 currentElements.Add(new Model.Currency("LKC", "Luckycoin"));
                 currentElements.Add(new Model.Currency("MBL", "MobileCash"));
                 currentElements.Add(new Model.Currency("MST", "Mustang Coin"));
                 currentElements.Add(new Model.Currency("NTC", "Neptunecoin"));
                 currentElements.Add(new Model.Currency("OK", "OKCash"));
                 currentElements.Add(new Model.Currency("ORLY", "Orlycoin"));
                 currentElements.Add(new Model.Currency("PAPAFR", "PAPAFRANCESCOCoin"));
                 currentElements.Add(new Model.Currency("PEC", "PeaceCoin"));
                 currentElements.Add(new Model.Currency("POST", "PostCoin"));
                 currentElements.Add(new Model.Currency("PVC", "PuraVidaCoin"));
                 currentElements.Add(new Model.Currency("QBK", "Qibuckcoin"));
                 currentElements.Add(new Model.Currency("RVC", "Revcoin"));
                 currentElements.Add(new Model.Currency("SFC", "Safecoin"));
                 currentElements.Add(new Model.Currency("SH", "Shilling"));
                 currentElements.Add(new Model.Currency("SPEC", "SPEC"));
                 currentElements.Add(new Model.Currency("SSC", "SSCoin"));
                 currentElements.Add(new Model.Currency("SSUC", "SSUCoin"));
                 currentElements.Add(new Model.Currency("STP", "Stepcoin"));
                 currentElements.Add(new Model.Currency("SUPER", "Supercoin"));
                 currentElements.Add(new Model.Currency("SCN", "Swiscoin"));
                 currentElements.Add(new Model.Currency("TCOIN", "T-coin"));
                 currentElements.Add(new Model.Currency("TAO", "Tao"));
                 currentElements.Add(new Model.Currency("TRNT", "Tarrant"));
                 currentElements.Add(new Model.Currency("TEK", "TEKcoin"));
                 currentElements.Add(new Model.Currency("TAGR", "Think and Get Rich Coin"));
                 currentElements.Add(new Model.Currency("TRUMP", "TrumpCoin"));
                 currentElements.Add(new Model.Currency("UNB", "UnbreakableCoin"));
                 currentElements.Add(new Model.Currency("UNC", "UNCoin"));
                 currentElements.Add(new Model.Currency("UPC", "UnionPlusCoin"));
                 currentElements.Add(new Model.Currency("XVC", "Vcash"));
                 currentElements.Add(new Model.Currency("VOX", "VOXELS"));
                 currentElements.Add(new Model.Currency("VPN", "Vpncoin"));
                 currentElements.Add(new Model.Currency("WAY", "WayGuide"));
                 currentElements.Add(new Model.Currency("XCO", "Xcoin"));
                 currentElements.Add(new Model.Currency("MI", "Xiaomicoin"));
                 currentElements.Add(new Model.Currency("XPAY", "xPAY"));
                 currentElements.Add(new Model.Currency("YOC", "Yocoin"));
                 currentElements.Add(new Model.Currency("ZUR", "Zurcoin"));

                 return currentElements;
             });
            return list;
        }
    }
}

