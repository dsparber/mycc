using System;

namespace MyCryptos.Core.Models
{
	/// <summary>
	/// Model for a simple account
	/// </summary>
	public class Account : PersistableRepositoryElement<int>
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Models.Account"/> class.
		/// </summary>
		/// <param name="name">Name of the account</param>
		/// <param name="money">Money of the account to be initialised with</param>
		public Account(string name, Money money)
		{
			if (money == null)
			{
				throw new ArgumentNullException();
			}
			Name = name;
			Money = money;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:models.Account"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="name">Name.</param>
		/// <param name="money">Money.</param>
		public Account(int id, string name, Money money) : this(name, money)
		{
			Id = id;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MyCryptos.Core.Models.Account"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="repositoryId">Repository identifier.</param>
		/// <param name="name">Name.</param>
		/// <param name="money">Money.</param>
		public Account(int id, int repositoryId, string name, Money money) : this(id, name, money)
		{
			RepositoryId = repositoryId;
		}

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier of the repository</value>
		public int RepositoryId { get; set; }

		/// <summary>
		/// Money of the account
		/// </summary>
		public Money Money { get; }

		/// <summary>
		/// The name of the account
		/// </summary>
		public string Name
		{
			get { return name; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				name = value;
			}
		}

		private string name;

		public override bool Equals(object obj)
		{
			if (!(obj is Account))
			{
				return false;
			}
			var a = (Account)obj;

			if (Id != 0)
			{
				return Id == a.Id;
			}

			return Name.Equals(a.Name) && Money.Currency.Equals(a.Money.Currency);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return $"{Name}: {Money}";
		}
	}
}