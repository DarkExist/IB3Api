using ErrorOr;
using IB3Api.App;
using IB3Api.App.Interfaces.Repository;
using IB3Api.App.Interfaces.Services;
using IB3Api.Core.Models;
using IB3Api.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IRoleRepository _roleRepository;

		public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
		{
			_userRepository = userRepository;
			_roleRepository = roleRepository;
		}

		public async Task<ErrorOr<Success>> AddRoleAsync(Guid guid, string roleName, CancellationToken cancellationToken)
		{
			var errorOrRole = await _roleRepository.GetRoleByNameAsync(roleName, cancellationToken);
			if (errorOrRole.IsError) 
				return Error.Failure("Role doesnt exists");

			var errorOrUser = await _userRepository.GetByIdAsync(guid, cancellationToken);
			if (errorOrUser.IsError)
				return Error.Failure("User doesnt exists");

			User user = errorOrUser.Value;
			Role role = errorOrRole.Value;
			user.Roles.Add(role);

			return await _userRepository.UpdateAsync(user, cancellationToken);

		}

		public async Task<ErrorOr<Success>> CreateUserAsync(User user, CancellationToken cancellationToken)
		{
			return await _userRepository.AddAsync(user, cancellationToken);
		}

		public async Task<ErrorOr<Success>> DeleteRoleAsync(Guid guid, string roleName, CancellationToken cancellationToken)
		{
			var errorOrRole = await _roleRepository.GetRoleByNameAsync(roleName, cancellationToken);
			if (errorOrRole.IsError)
				return Error.Failure("Role doesnt exists");

			var errorOrUser = await _userRepository.GetByIdAsync(guid, cancellationToken);
			if (errorOrUser.IsError)
				return Error.Failure("User doesnt exists");

			User user = errorOrUser.Value;
			Role role = errorOrRole.Value;
			if (!user.Roles.Contains(role))
				return Error.Failure("user doesnt have such role");
			user.Roles.Remove(role);

			return await _userRepository.UpdateAsync(user, cancellationToken);
		}

		public async Task<ErrorOr<Success>> DeleteUserByIdAsync(Guid guid, CancellationToken cancellationToken)
		{
			return await _userRepository.DeleteByIdAsync(guid, cancellationToken);
		}

		public async Task<ErrorOr<List<Role>>> GetRolesListAsync(Guid guid, CancellationToken cancellationToken)
		{
			var errorOrUser = await _userRepository.GetByIdAsync(guid, cancellationToken);
			if (errorOrUser.IsError)
				return Error.Failure("user doesnt exists");

			User user = errorOrUser.Value;

			return user.Roles.ToList();
		}

		public async Task<ErrorOr<User>> GetUserByIdAsync(Guid guid, CancellationToken cancellationToken)
		{
			return await _userRepository.GetByIdAsync(guid, cancellationToken);
		}

		public async Task<ErrorOr<User>> GetUserByNameAsync(string name, CancellationToken cancellationToken)
		{
			return await _userRepository.GetByNameAsync(name, cancellationToken);
		}

		public async Task<ErrorOr<Success>> UpdateUser(User user, CancellationToken cancellationToken)
		{
			return await _userRepository.UpdateAsync(user, cancellationToken);
		}
	}
}
