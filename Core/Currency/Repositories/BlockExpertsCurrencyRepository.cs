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
                 var currentElements = new List<Model.Currency>
                 {
                     new Model.Currency("2GIVE", "2GiveCoin"),
                     new Model.Currency("AMBER", "Ambercoin"),
                     new Model.Currency("ASW", "AmericanStandardWallet"),
                     new Model.Currency("ANTI", "Anti Bitcoin"),
                     new Model.Currency("ARC", "AquariusCoin"),
                     new Model.Currency("ARG", "Argentum"),
                     new Model.Currency("ADCN", "Asiadigicoin"),
                     new Model.Currency("ADC", "AudioCoin"),
                     new Model.Currency("ALX", "AvalonX"),
                     new Model.Currency("BEEZ", "BeezerCoin"),
                     new Model.Currency("BLRY", "Billary Coin"),
                     new Model.Currency("BIOS", "Bios crypto"),
                     new Model.Currency("BTCD", "BitcoinDark"),
                     new Model.Currency("XBC", "BitcoinPlus"),
                     new Model.Currency("BSD", "BitSend"),
                     new Model.Currency("BITS", "Bitstar"),
                     new Model.Currency("BLU", "Bluecoin"),
                     new Model.Currency("BEC", "Bluecoin"),
                     new Model.Currency("CASH", "Cashcoin"),
                     new Model.Currency("CPC", "CereiPayCoin"),
                     new Model.Currency("CHESS", "ChessCoin"),
                     new Model.Currency("CMC", "CMI"),
                     new Model.Currency("COC", "CoCreateCoin"),
                     new Model.Currency("COIN", "COIN"),
                     new Model.Currency("CV2", "Colossuscoin2"),
                     new Model.Currency("CPI", "CPICoin"),
                     new Model.Currency("CRW", "Crowncoin"),
                     new Model.Currency("DICK", "Dickcoin"),
                     new Model.Currency("DIME", "Dimecoin"),
                     new Model.Currency("DOPE", "Dopecoin"),
                     new Model.Currency("DVC", "Dovecoin"),
                     new Model.Currency("ECC", "E-CurrencyCoin"),
                     new Model.Currency("ESP", "Espers"),
                     new Model.Currency("FEDS", "FedoraShare"),
                     new Model.Currency("FRWC", "Frankywillcoin"),
                     new Model.Currency("GBRC", "GlobalBusinessRevolution"),
                     new Model.Currency("XGR", "GoldReserve"),
                     new Model.Currency("GRT", "Grantcoin"),
                     new Model.Currency("HAM", "HamRadiocoin"),
                     new Model.Currency("HCC", "HCCoin"),
                     new Model.Currency("HEMP", "Hempcoin"),
                     new Model.Currency("HNC", "HunCoin"),
                     new Model.Currency("NKA", "IncaKoin"),
                     new Model.Currency("INC", "Incrementum"),
                     new Model.Currency("ICASH", "Internet Cash"),
                     new Model.Currency("ISL", "IslaCoin"),
                     new Model.Currency("XJO", "JouleCoin"),
                     new Model.Currency("KCC", "K-COIN"),
                     new Model.Currency("KNC", "KhanCoin"),
                     new Model.Currency("KUC", "Kuwaitcoin"),
                     new Model.Currency("LDC", "Landcoin"),
                     new Model.Currency("LEA", "LeaCoin"),
                     new Model.Currency("LEO", "LEOcoin"),
                     new Model.Currency("LKC2", "LinkedCoin"),
                     new Model.Currency("LKC", "Luckycoin"),
                     new Model.Currency("MBL", "MobileCash"),
                     new Model.Currency("MST", "Mustang Coin"),
                     new Model.Currency("NTC", "Neptunecoin"),
                     new Model.Currency("OK", "OKCash"),
                     new Model.Currency("ORLY", "Orlycoin"),
                     new Model.Currency("PAPAFR", "PAPAFRANCESCOCoin"),
                     new Model.Currency("PEC", "PeaceCoin"),
                     new Model.Currency("POST", "PostCoin"),
                     new Model.Currency("PVC", "PuraVidaCoin"),
                     new Model.Currency("QBK", "Qibuckcoin"),
                     new Model.Currency("RVC", "Revcoin"),
                     new Model.Currency("SFC", "Safecoin"),
                     new Model.Currency("SH", "Shilling"),
                     new Model.Currency("SPEC", "SPEC"),
                     new Model.Currency("SSC", "SSCoin"),
                     new Model.Currency("SSUC", "SSUCoin"),
                     new Model.Currency("STP", "Stepcoin"),
                     new Model.Currency("SUPER", "Supercoin"),
                     new Model.Currency("SCN", "Swiscoin"),
                     new Model.Currency("TCOIN", "T-coin"),
                     new Model.Currency("TAO", "Tao"),
                     new Model.Currency("TRNT", "Tarrant"),
                     new Model.Currency("TEK", "TEKcoin"),
                     new Model.Currency("TAGR", "Think and Get Rich Coin"),
                     new Model.Currency("TRUMP", "TrumpCoin"),
                     new Model.Currency("UNB", "UnbreakableCoin"),
                     new Model.Currency("UNC", "UNCoin"),
                     new Model.Currency("UPC", "UnionPlusCoin"),
                     new Model.Currency("XVC", "Vcash"),
                     new Model.Currency("VOX", "VOXELS"),
                     new Model.Currency("VPN", "Vpncoin"),
                     new Model.Currency("WAY", "WayGuide"),
                     new Model.Currency("XCO", "Xcoin"),
                     new Model.Currency("MI", "Xiaomicoin"),
                     new Model.Currency("XPAY", "xPAY"),
                     new Model.Currency("YOC", "Yocoin"),
                     new Model.Currency("ZUR", "Zurcoin")
                 };


                 return currentElements;
             });
            return list;
        }
    }
}

