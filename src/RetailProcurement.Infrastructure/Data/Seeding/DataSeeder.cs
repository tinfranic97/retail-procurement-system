using Bogus;
using Microsoft.EntityFrameworkCore;
using RetailProcurement.Core.Entities;

namespace RetailProcurement.Infrastructure.Data.Seeding;

public static class DataSeeder
{
    private static readonly string[] Categories = ["Electronics", "Clothing", "Food", "Furniture", "Tools", "Sports", "Books"];

    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Suppliers.AnyAsync()) return;

        Randomizer.Seed = new Random(42);

        var supplierFaker = new Faker<Supplier>()
            .RuleFor(s => s.Name, f => f.Company.CompanyName())
            .RuleFor(s => s.Email, (f, s) => f.Internet.Email(s.Name.ToLower().Replace(" ", ".")))
            .RuleFor(s => s.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(s => s.Address, f => f.Address.FullAddress())
            .RuleFor(s => s.ContactPerson, f => f.Name.FullName())
            .RuleFor(s => s.CreatedAt, f => f.Date.Past(2).ToUniversalTime())
            .RuleFor(s => s.UpdatedAt, (f, s) => s.CreatedAt);

        var suppliers = supplierFaker.Generate(10);

        var storeItemFaker = new Faker<StoreItem>()
            .RuleFor(si => si.Name, f => f.Commerce.ProductName())
            .RuleFor(si => si.Description, f => f.Commerce.ProductDescription())
            .RuleFor(si => si.Price, f => decimal.Parse(f.Commerce.Price(5, 500)))
            .RuleFor(si => si.Category, f => f.PickRandom(Categories))
            .RuleFor(si => si.StockQuantity, f => f.Random.Int(0, 200))
            .RuleFor(si => si.CreatedAt, f => f.Date.Past(2).ToUniversalTime())
            .RuleFor(si => si.UpdatedAt, (f, si) => si.CreatedAt);

        var storeItems = storeItemFaker.Generate(30);

        await context.Suppliers.AddRangeAsync(suppliers);
        await context.StoreItems.AddRangeAsync(storeItems);
        await context.SaveChangesAsync();

        var random = new Random(42);
        var supplierStoreItems = new List<SupplierStoreItem>();
        var addedPairs = new HashSet<(int, int)>();

        foreach (var item in storeItems)
        {
            var itemSuppliers = suppliers.OrderBy(_ => random.Next()).Take(random.Next(1, 4)).ToList();
            foreach (var supplier in itemSuppliers)
            {
                if (addedPairs.Add((supplier.Id, item.Id)))
                {
                    supplierStoreItems.Add(new SupplierStoreItem
                    {
                        SupplierId = supplier.Id,
                        StoreItemId = item.Id,
                        SupplierPrice = Math.Round(item.Price * (decimal)(0.6 + random.NextDouble() * 0.4), 2),
                        LeadTimeDays = random.Next(1, 30),
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        await context.SupplierStoreItems.AddRangeAsync(supplierStoreItems);

        var salesFaker = new Faker<SalesRecord>()
            .RuleFor(sr => sr.StoreItemId, f => f.PickRandom(storeItems).Id)
            .RuleFor(sr => sr.SupplierId, f => f.PickRandom(suppliers).Id)
            .RuleFor(sr => sr.Quantity, f => f.Random.Int(1, 50))
            .RuleFor(sr => sr.UnitPrice, f => decimal.Parse(f.Commerce.Price(5, 500)))
            .RuleFor(sr => sr.SaleDate, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(sr => sr.CreatedAt, (f, sr) => sr.SaleDate)
            .RuleFor(sr => sr.UpdatedAt, (f, sr) => sr.SaleDate);

        var salesRecords = salesFaker.Generate(100);
        await context.SalesRecords.AddRangeAsync(salesRecords);
        await context.SaveChangesAsync();
    }
}
