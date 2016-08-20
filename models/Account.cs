using System.Collections.Generic;

namespace models
{
	/// <summary>
	/// Model for a simple account
	/// </summary>
	public class Account
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
	}
}