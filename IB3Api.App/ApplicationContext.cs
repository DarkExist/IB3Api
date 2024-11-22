using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IB3Api.App.Config;
using IB3Api.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace IB3Api.App
{
	public class ApplicationContext : DbContext
	{
		public DbSet<User> Users { get; set; } = null!;
		public DbSet<Post> Posts { get; set; } = null!;
		public DbSet<Profile> Profiles { get; set; } = null!;
		public DbSet<Role> Roles { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new PostsConfiguration());
			modelBuilder.ApplyConfiguration(new ProfileConfiguration());
			modelBuilder.ApplyConfiguration(new RolesConfiguration());
			modelBuilder.ApplyConfiguration(new UsersConfiguration());
		}
	}
}
