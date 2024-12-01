using IB3Api.App.Interfaces.Repository;
using IB3Api.App.Interfaces.Services;
using IB3Api.Infrastructure.Repositories;
using IB3Api.Services;

namespace IB3Api.Api
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddTransient<IPostRepository, PostRepository>();
			services.AddTransient<IProfileRepository, ProfileRepository>();
			services.AddTransient<IRoleRepository, RoleRepository>();
			services.AddTransient<IUserRepository, UserRepository>();

			services.AddTransient<IPostService, PostService>();
			services.AddTransient<IProfileService, ProfileService>();
			services.AddTransient<IRoleService, RoleService>();
			services.AddTransient<IUserService, UserService>();

			services.AddTransient<IDiffieHelmanService, DiffieHelmanService>();
			services.AddTransient<IEncryptionService, EncryptorService>();
			
			return services;
		}
	}
}
