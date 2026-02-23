using BestStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Infrastructure.Contexts.Seeds
{
    public static class BusinessDataSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (context.Categories.Any())
                return;

            var categories = new List<Category>
                    {
                        new() { Name = "Phones" },
                        new() { Name = "Computers" },
                        new() { Name = "Accessories" },
                        new() { Name = "Printers" },
                        new() { Name = "Cameras" },
                        new() { Name = "Other" }
                    };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }

}
