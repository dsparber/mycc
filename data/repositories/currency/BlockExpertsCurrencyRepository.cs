using System;
using System.Diagnostics;
using System.Threading.Tasks;
using data.database.models;
using models;

namespace data.repositories.currency
{
	public class BlockExpertsCurrencyRepository : OnlineCurrencyRepository
	{
		public BlockExpertsCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_BLOCK_EXPERTS_REPOSITORY, name) { }

		public override async Task<bool> Fetch()
		{
			Elements.Add(new Currency("2GIVE", "2GiveCoin"));
			Elements.Add(new Currency("AMBER", "Ambercoin"));
			Elements.Add(new Currency("ASW", "AmericanStandardWallet"));
			Elements.Add(new Currency("ANTI", "Anti Bitcoin"));
			Elements.Add(new Currency("ARC", "AquariusCoin"));
			Elements.Add(new Currency("ARG", "Argentum"));
			Elements.Add(new Currency("ADCN", "Asiadigicoin"));
			Elements.Add(new Currency("ADC", "AudioCoin"));
			Elements.Add(new Currency("ALX", "AvalonX"));
			Elements.Add(new Currency("BEEZ", "BeezerCoin"));
			Elements.Add(new Currency("BLRY", "Billary Coin"));
			Elements.Add(new Currency("BIOS", "Bios crypto"));
			Elements.Add(new Currency("BTCD", "BitcoinDark"));
			Elements.Add(new Currency("XBC", "BitcoinPlus"));
			Elements.Add(new Currency("BSD", "BitSend"));
			Elements.Add(new Currency("BITS", "Bitstar"));
			Elements.Add(new Currency("BLU", "Bluecoin"));
			Elements.Add(new Currency("BEC", "Bluecoin"));
			Elements.Add(new Currency("CASH", "Cashcoin"));
			Elements.Add(new Currency("CPC", "CereiPayCoin"));
			Elements.Add(new Currency("CHESS", "ChessCoin"));
			Elements.Add(new Currency("CMC", "CMI"));
			Elements.Add(new Currency("COC", "CoCreateCoin"));
			Elements.Add(new Currency("COIN", "COIN"));
			Elements.Add(new Currency("CV2", "Colossuscoin2"));
			Elements.Add(new Currency("CPI", "CPICoin"));
			Elements.Add(new Currency("CRW", "Crowncoin"));
			Elements.Add(new Currency("DICK", "Dickcoin"));
			Elements.Add(new Currency("DIME", "Dimecoin"));
			Elements.Add(new Currency("DOPE", "Dopecoin"));
			Elements.Add(new Currency("DVC", "Dovecoin"));
			Elements.Add(new Currency("ECC", "E-CurrencyCoin"));
			Elements.Add(new Currency("ESP", "Espers"));
			Elements.Add(new Currency("FEDS", "FedoraShare"));
			Elements.Add(new Currency("FRWC", "Frankywillcoin"));
			Elements.Add(new Currency("GBRC", "GlobalBusinessRevolution"));
			Elements.Add(new Currency("XGR", "GoldReserve"));
			Elements.Add(new Currency("GRT", "Grantcoin"));
			Elements.Add(new Currency("HAM", "HamRadiocoin"));
			Elements.Add(new Currency("HCC", "HCCoin"));
			Elements.Add(new Currency("HEMP", "Hempcoin"));
			Elements.Add(new Currency("HNC", "HunCoin"));
			Elements.Add(new Currency("NKA", "IncaKoin"));
			Elements.Add(new Currency("INC", "Incrementum"));
			Elements.Add(new Currency("ICASH", "Internet Cash"));
			Elements.Add(new Currency("ISL", "IslaCoin"));
			Elements.Add(new Currency("XJO", "JouleCoin"));
			Elements.Add(new Currency("KCC", "K-COIN"));
			Elements.Add(new Currency("KNC", "KhanCoin"));
			Elements.Add(new Currency("KUC", "Kuwaitcoin"));
			Elements.Add(new Currency("LDC", "Landcoin"));
			Elements.Add(new Currency("LEA", "LeaCoin"));
			Elements.Add(new Currency("LEO", "LEOcoin"));
			Elements.Add(new Currency("LKC2", "LinkedCoin"));
			Elements.Add(new Currency("LKC", "Luckycoin"));
			Elements.Add(new Currency("MBL", "MobileCash"));
			Elements.Add(new Currency("MST", "Mustang Coin"));
			Elements.Add(new Currency("NTC", "Neptunecoin"));
			Elements.Add(new Currency("OK", "OKCash"));
			Elements.Add(new Currency("ORLY", "Orlycoin"));
			Elements.Add(new Currency("PAPAFR", "PAPAFRANCESCOCoin"));
			Elements.Add(new Currency("PEC", "PeaceCoin"));
			Elements.Add(new Currency("POST", "PostCoin"));
			Elements.Add(new Currency("PVC", "PuraVidaCoin"));
			Elements.Add(new Currency("QBK", "Qibuckcoin"));
			Elements.Add(new Currency("RVC", "Revcoin"));
			Elements.Add(new Currency("SFC", "Safecoin"));
			Elements.Add(new Currency("SH", "Shilling"));
			Elements.Add(new Currency("SPEC", "SPEC"));
			Elements.Add(new Currency("SSC", "SSCoin"));
			Elements.Add(new Currency("SSUC", "SSUCoin"));
			Elements.Add(new Currency("STP", "Stepcoin"));
			Elements.Add(new Currency("SUPER", "Supercoin"));
			Elements.Add(new Currency("SCN", "Swiscoin"));
			Elements.Add(new Currency("TCOIN", "T-coin"));
			Elements.Add(new Currency("TAO", "Tao"));
			Elements.Add(new Currency("TRNT", "Tarrant"));
			Elements.Add(new Currency("TEK", "TEKcoin"));
			Elements.Add(new Currency("TAGR", "Think and Get Rich Coin"));
			Elements.Add(new Currency("TRUMP", "TrumpCoin"));
			Elements.Add(new Currency("UNB", "UnbreakableCoin"));
			Elements.Add(new Currency("UNC", "UNCoin"));
			Elements.Add(new Currency("UPC", "UnionPlusCoin"));
			Elements.Add(new Currency("XVC", "Vcash"));
			Elements.Add(new Currency("VOX", "VOXELS"));
			Elements.Add(new Currency("VPN", "Vpncoin"));
			Elements.Add(new Currency("WAY", "WayGuide"));
			Elements.Add(new Currency("XCO", "Xcoin"));
			Elements.Add(new Currency("MI", "Xiaomicoin"));
			Elements.Add(new Currency("XPAY", "xPAY"));
			Elements.Add(new Currency("YOC", "Yocoin"));
			Elements.Add(new Currency("ZUR", "Zurcoin"));

			try
			{
				await WriteToDatabase();
				LastFetch = DateTime.Now;
				return true;
			}
			catch (Exception e)
			{
				Elements.Clear();
				Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
				return false;
			}
		}
	}
}

