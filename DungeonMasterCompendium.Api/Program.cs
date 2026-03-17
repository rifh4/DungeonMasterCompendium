using DungeonMasterCompendium.Api.Integrations.Open5e.Items;
using DungeonMasterCompendium.Api.Integrations.Open5e.Monsters;
using DungeonMasterCompendium.Api.Integrations.Open5e.Spells;
using DungeonMasterCompendium.Api.Options;
using DungeonMasterCompendium.Api.Services;

namespace DungeonMasterCompendium.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<Open5eOptions>(
                builder.Configuration.GetSection(Open5eOptions.SectionName));

            builder.Services.AddScoped<IMonstersService, MonstersService>();
            builder.Services.AddHttpClient<IOpen5eMonsterClient, Open5eMonsterClient>();

            builder.Services.AddScoped<ISpellsService, SpellsService>();
            builder.Services.AddHttpClient<IOpen5eSpellClient, Open5eSpellClient>();

            builder.Services.AddScoped<IItemsService, ItemsService>();
            builder.Services.AddHttpClient<IOpen5eItemClient, Open5eItemClient>();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

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