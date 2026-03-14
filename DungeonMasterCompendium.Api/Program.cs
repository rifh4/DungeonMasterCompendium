
using DungeonMasterCompendium.Api.Integrations.Open5e;
using DungeonMasterCompendium.Api.Options;
using DungeonMasterCompendium.Api.Services;


namespace DungeonMasterCompendium.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.Configure<Open5eOptions>(builder.Configuration.GetSection("Open5e"));
            builder.Services.AddScoped<IMonstersService, MonstersService>();
            builder.Services.AddHttpClient<IOpen5eMonsterClient, Open5eMonsterClient>();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
