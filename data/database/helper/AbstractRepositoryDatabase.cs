﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace data.database.helper
{
	public abstract class AbstractRepositoryDatabase<T> : AbstractDatabase
	{
		public abstract Task<IEnumerable<T>> GetRepositories();
	}
}

