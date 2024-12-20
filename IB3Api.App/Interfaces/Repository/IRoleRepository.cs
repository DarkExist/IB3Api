﻿using ErrorOr;
using IB3Api.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Interfaces.Repository
{
	public interface IRoleRepository : IRepository<Role>
	{
		Task<ErrorOr<Role>> GetRoleByNameAsync(string name, CancellationToken cancellationToken);
	}
}
