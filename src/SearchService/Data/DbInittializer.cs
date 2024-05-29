using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync(
                "SearchDb",
                MongoClientSettings.FromConnectionString(
                    app.Configuration.GetConnectionString("MongoDbConnection")
                )
            );

            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();

            var count = await DB.CountAsync<Item>();
            if (count == 0)
            {
                System.Console.WriteLine("No data - will attempt to seed");
                // var itemData = await File.ReadAllTextAsync("Data/Auctions.json");

                // var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

                using var scope = app.Services.CreateScope();

                var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

                var items = await httpClient.GetItemsForSearchDb();

                System.Console.WriteLine($"Add {items.Count} items from Auction Service");

                await DB.SaveAsync(items);
            }else{
                System.Console.WriteLine($"Has {count} item in MongoDB");
            }
        }
    }
}
