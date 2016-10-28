using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using MyCryptos.models;

namespace data.repositories.currency
{
	public class BlockExpertsCurrencyRepository : OnlineCurrencyRepository
	{
		public BlockExpertsCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_BLOCK_EXPERTS_REPOSITORY, name) { }

		protected async override Task<IEnumerable<Currency>> GetCurrencies()
		{
			var list = await Task.Factory.StartNew(() =>
			 {
				 var currentElements = new List<Currency>();

				 currentElements.Add(new Currency("2GIVE", "2GiveCoin"));
				 currentElements.Add(new Currency("AMBER", "Ambercoin"));
				 currentElements.Add(new Currency("ASW", "AmericanStandardWallet"));
				 currentElements.Add(new Currency("ANTI", "Anti Bitcoin"));
				 currentElements.Add(new Currency("ARC", "AquariusCoin"));
				 currentElements.Add(new Currency("ARG", "Argentum"));
				 currentElements.Add(new Currency("ADCN", "Asiadigicoin"));
				 currentElements.Add(new Currency("ADC", "AudioCoin"));
				 currentElements.Add(new Currency("ALX", "AvalonX"));
				 currentElements.Add(new Currency("BEEZ", "BeezerCoin"));
				 currentElements.Add(new Currency("BLRY", "Billary Coin"));
				 currentElements.Add(new Currency("BIOS", "Bios crypto"));
				 currentElements.Add(new Currency("BTCD", "BitcoinDark"));
				 currentElements.Add(new Currency("XBC", "BitcoinPlus"));
				 currentElements.Add(new Currency("BSD", "BitSend"));
				 currentElements.Add(new Currency("BITS", "Bitstar"));
				 currentElements.Add(new Currency("BLU", "Bluecoin"));
				 currentElements.Add(new Currency("BEC", "Bluecoin"));
				 currentElements.Add(new Currency("CASH", "Cashcoin"));
				 currentElements.Add(new Currency("CPC", "CereiPayCoin"));
				 currentElements.Add(new Currency("CHESS", "ChessCoin"));
				 currentElements.Add(new Currency("CMC", "CMI"));
				 currentElements.Add(new Currency("COC", "CoCreateCoin"));
				 currentElements.Add(new Currency("COIN", "COIN"));
				 currentElements.Add(new Currency("CV2", "Colossuscoin2"));
				 currentElements.Add(new Currency("CPI", "CPICoin"));
				 currentElements.Add(new Currency("CRW", "Crowncoin"));
				 currentElements.Add(new Currency("DICK", "Dickcoin"));
				 currentElements.Add(new Currency("DIME", "Dimecoin"));
				 currentElements.Add(new Currency("DOPE", "Dopecoin"));
				 currentElements.Add(new Currency("DVC", "Dovecoin"));
				 currentElements.Add(new Currency("ECC", "E-CurrencyCoin"));
				 currentElements.Add(new Currency("ESP", "Espers"));
				 currentElements.Add(new Currency("FEDS", "FedoraShare"));
				 currentElements.Add(new Currency("FRWC", "Frankywillcoin"));
				 currentElements.Add(new Currency("GBRC", "GlobalBusinessRevolution"));
				 currentElements.Add(new Currency("XGR", "GoldReserve"));
				 currentElements.Add(new Currency("GRT", "Grantcoin"));
				 currentElements.Add(new Currency("HAM", "HamRadiocoin"));
				 currentElements.Add(new Currency("HCC", "HCCoin"));
				 currentElements.Add(new Currency("HEMP", "Hempcoin"));
				 currentElements.Add(new Currency("HNC", "HunCoin"));
				 currentElements.Add(new Currency("NKA", "IncaKoin"));
				 currentElements.Add(new Currency("INC", "Incrementum"));
				 currentElements.Add(new Currency("ICASH", "Internet Cash"));
				 currentElements.Add(new Currency("ISL", "IslaCoin"));
				 currentElements.Add(new Currency("XJO", "JouleCoin"));
				 currentElements.Add(new Currency("KCC", "K-COIN"));
				 currentElements.Add(new Currency("KNC", "KhanCoin"));
				 currentElements.Add(new Currency("KUC", "Kuwaitcoin"));
				 currentElements.Add(new Currency("LDC", "Landcoin"));
				 currentElements.Add(new Currency("LEA", "LeaCoin"));
				 currentElements.Add(new Currency("LEO", "LEOcoin"));
				 currentElements.Add(new Currency("LKC2", "LinkedCoin"));
				 currentElements.Add(new Currency("LKC", "Luckycoin"));
				 currentElements.Add(new Currency("MBL", "MobileCash"));
				 currentElements.Add(new Currency("MST", "Mustang Coin"));
				 currentElements.Add(new Currency("NTC", "Neptunecoin"));
				 currentElements.Add(new Currency("OK", "OKCash"));
				 currentElements.Add(new Currency("ORLY", "Orlycoin"));
				 currentElements.Add(new Currency("PAPAFR", "PAPAFRANCESCOCoin"));
				 currentElements.Add(new Currency("PEC", "PeaceCoin"));
				 currentElements.Add(new Currency("POST", "PostCoin"));
				 currentElements.Add(new Currency("PVC", "PuraVidaCoin"));
				 currentElements.Add(new Currency("QBK", "Qibuckcoin"));
				 currentElements.Add(new Currency("RVC", "Revcoin"));
				 currentElements.Add(new Currency("SFC", "Safecoin"));
				 currentElements.Add(new Currency("SH", "Shilling"));
				 currentElements.Add(new Currency("SPEC", "SPEC"));
				 currentElements.Add(new Currency("SSC", "SSCoin"));
				 currentElements.Add(new Currency("SSUC", "SSUCoin"));
				 currentElements.Add(new Currency("STP", "Stepcoin"));
				 currentElements.Add(new Currency("SUPER", "Supercoin"));
				 currentElements.Add(new Currency("SCN", "Swiscoin"));
				 currentElements.Add(new Currency("TCOIN", "T-coin"));
				 currentElements.Add(new Currency("TAO", "Tao"));
				 currentElements.Add(new Currency("TRNT", "Tarrant"));
				 currentElements.Add(new Currency("TEK", "TEKcoin"));
				 currentElements.Add(new Currency("TAGR", "Think and Get Rich Coin"));
				 currentElements.Add(new Currency("TRUMP", "TrumpCoin"));
				 currentElements.Add(new Currency("UNB", "UnbreakableCoin"));
				 currentElements.Add(new Currency("UNC", "UNCoin"));
				 currentElements.Add(new Currency("UPC", "UnionPlusCoin"));
				 currentElements.Add(new Currency("XVC", "Vcash"));
				 currentElements.Add(new Currency("VOX", "VOXELS"));
				 currentElements.Add(new Currency("VPN", "Vpncoin"));
				 currentElements.Add(new Currency("WAY", "WayGuide"));
				 currentElements.Add(new Currency("XCO", "Xcoin"));
				 currentElements.Add(new Currency("MI", "Xiaomicoin"));
				 currentElements.Add(new Currency("XPAY", "xPAY"));
				 currentElements.Add(new Currency("YOC", "Yocoin"));
				 currentElements.Add(new Currency("ZUR", "Zurcoin"));

				 return currentElements;
			 });
			return list;
		}
	}
}

