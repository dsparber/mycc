using System;
using MyCC.Core.Abstract.Models;

namespace MyCC.Core.Account.Models.Base
{
    /// <summary>
    /// Model for a simple account
    /// </summary>
    public class Account : IPersistableWithParent<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MyCC.Core.Account.Models.Base.Account"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="repositoryId">Repository identifier.</param>
        /// <param name="name">Name.</param>
        /// <param name="money">Money.</param>
        /// <param name="isEnabeld">Wether the account is enabled for the overview</param>
        protected Account(int id, int repositoryId, string name, Money money, bool isEnabeld = true)
        {
            Name = name;
            Money = money;
            Id = id;
            ParentId = repositoryId;
            IsEnabled = isEnabeld;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier of the repository</value>
        public int ParentId { get; set; }

        /// <summary>
        /// Money of the account
        /// </summary>
        private Money _money;
        public Money Money
        {
            get
            {
                return _money;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                _money = value;
            }
        }

        /// <summary>
        /// The name of the account
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                _name = value;
            }
        }

        private string _name;

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