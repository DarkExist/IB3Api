using IB3Api.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Config
{
	public sealed class UsersConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("Users");
			builder.HasKey(t => t.Id);
			builder.HasMany(u => u.Posts)
				.WithMany(p => p.Users)
				.UsingEntity(j => j
				.ToTable("UsersPosts"));

			builder.HasMany(u => u.Roles)
				.WithMany(r => r.Users)
				.UsingEntity(j => j
				.ToTable("UsersRoles"));

			builder.HasOne(u => u.ProfileRef)
				.WithOne(p => p.UserRef);
		}
	}
}
