using models;
using SQLite;
namespace data.database.models
{
	public class CurrencyDBM
	{
		public CurrencyDBM() { }

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		[MaxLength(3)]
		public string Code { get; set; }

		public Currency ToCurrency()
		{
			return new Currency(Id, Code, Name);
		}

		public CurrencyDBM(Currency currency)
		{
			Id = currency.Id.Value;
			Name = currency.Name;
			Code = currency.Code;
		}
	}
}

