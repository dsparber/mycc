﻿using data.repositories.account;
using MyCryptos.data.repositories.account;

namespace MyCryptos.view.addrepositoryviews
{
	public sealed class AddBlockchainRepositoryView : AddAddressRepositoryView
    {
        protected override OnlineAccountRepository repository
        {
            get { return new BlockchainAccountRepository(null); }
        }

        protected override OnlineAccountRepository GetRepository(string name, string address)
        {
            return new BlockchainAccountRepository(name, address);
        }
    }
}
