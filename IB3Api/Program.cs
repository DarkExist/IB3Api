
using IB3Api.Api;
using IB3Api.App;
using Microsoft.EntityFrameworkCore;

namespace IB3Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			string connection = builder.Configuration.GetConnectionString("Connection");

			builder.Services.AddDbContext<ApplicationContext>(options =>
				options.UseNpgsql(connection));
			builder.Services.AddServices();

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowSpecificOrigins",
					policy =>
					{
						policy.WithOrigins("http://127.0.0.1:5500") // Укажите адрес клиента
							  .AllowAnyHeader()
							  .AllowAnyMethod()
							  .AllowCredentials()
							  .WithMethods("GET", "POST")
							  .WithOrigins("http://localhost:5500") // Укажите адрес клиента
							  .AllowAnyHeader()
							  .AllowAnyMethod()
							  .AllowCredentials()
							  .WithMethods("GET", "POST");
					});
			});



			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			app.UseCors("AllowSpecificOrigins");
			app.MapControllers();

			app.Run();
		}
	}
}
