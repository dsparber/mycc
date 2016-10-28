using System.Collections.Generic;

namespace MyCryptos.models
{
	/// <summary>
	/// Model for a simple account
	/// </summary>
	public class Account : PersistableRepositoryElement
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Models.Account"/> class.
		/// </summary>
		/// <param name="name">Name of the account</param>
		/// <param name="money">Money of the account to be initialised with</param>
		public Account(string name, Money money)
		{
			Name = name;
			Money = money;
			Tags = new List<Tag>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:models.Account"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="name">Name.</param>
		/// <param name="money">Money.</param>
		public Account(int id, string name, Money money)
		{
			Id = id;
			Name = name;
			Money = money;
			Tags = new List<Tag>();
		}

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public int? Id { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier of the repository</value>
		public int? RepositoryId { get; set; }

		/// <summary>
		/// Money of the account
		/// </summary>
		public Money Money;

		/// <summary>
		/// The name of the account
		/// </summary>
		public string Name;

		/// <summary>
		/// All saved Tags for the account
		/// </summary>
		/// <value>The tags.</value>
		public List<Tag> Tags { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Account))
			{
				return false;
			}
			var a = (Account)obj;

			if (Id.HasValue && a.Id.HasValue && Id.Value != 0)
			{
				return Id.Value == a.Id.Value;
			}

			return Name.Equals(a.Name) && Money.Currency.Equals(a.Money.Currency);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}