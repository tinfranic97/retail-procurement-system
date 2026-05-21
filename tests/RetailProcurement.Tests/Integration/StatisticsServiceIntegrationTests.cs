using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RetailProcurement.Core.DTOs.Statistics;
using RetailProcurement.Core.Entities;
using RetailProcurement.Infrastructure.Data;
using RetailProcurement.Infrastructure.Patterns.BestOffer;
using RetailProcurement.Infrastructure.Repositories;
using RetailProcurement.Infrastructure.Services;

namespace RetailProcurement.Tests.Integration;

public class StatisticsServiceIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly StatisticsService _service;

    public StatisticsServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _service = new StatisticsService(new UnitOfWork(_context), _context, new LowestPriceStrategy());
    }

    [Fact]
    public async Task GetSupplierStatisticsAsync_WithSales_ReturnsCorrectTotals()
    {
        var supplier = new Supplier { Name = "Test Supplier", Email = "s@test.com" };
        var item = new StoreItem { Name = "Widget", Price = 10m, Category = "Tools", StockQuantity = 100 };
        _context.Suppliers.Add(supplier);
        _context.StoreItems.Add(item);
        await _context.SaveChangesAsync();

        _context.SalesRecords.AddRange(
            new SalesRecord { SupplierId = supplier.Id, StoreItemId = item.Id, Quantity = 5, UnitPrice = 10m, SaleDate = DateTime.UtcNow },
            new SalesRecord { SupplierId = supplier.Id, StoreItemId = item.Id, Quantity = 3, UnitPrice = 10m, SaleDate = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        var stats = await _service.GetSupplierStatisticsAsync(supplier.Id);

        stats.Should().NotBeNull();
        stats!.TotalItemsSold.Should().Be(8);
        stats.TotalRevenue.Should().Be(80m);
    }

    [Fact]
    public async Task GetBestOfferAsync_MultipleSuppliers_ReturnsLowestPrice()
    {
        var supplier1 = new Supplier { Name = "Cheap Supplier", Email = "cheap@test.com" };
        var supplier2 = new Supplier { Name = "Expensive Supplier", Email = "exp@test.com" };
        var item = new StoreItem { Name = "Widget", Price = 20m, Category = "Tools", StockQuantity = 50 };
        _context.Suppliers.AddRange(supplier1, supplier2);
        _context.StoreItems.Add(item);
        await _context.SaveChangesAsync();

        _context.SupplierStoreItems.AddRange(
            new SupplierStoreItem { SupplierId = supplier1.Id, StoreItemId = item.Id, SupplierPrice = 8m, LeadTimeDays = 7 },
            new SupplierStoreItem { SupplierId = supplier2.Id, StoreItemId = item.Id, SupplierPrice = 15m, LeadTimeDays = 2 }
        );
        await _context.SaveChangesAsync();

        var offer = await _service.GetBestOfferAsync(item.Id);

        offer.Should().NotBeNull();
        offer!.SupplierId.Should().Be(supplier1.Id);
        offer.SupplierPrice.Should().Be(8m);
        offer.SelectionCriteria.Should().Be("Lowest Price");
    }

    [Fact]
    public async Task CreateQuarterlyPlanAsync_ValidData_CreatesEntry()
    {
        var supplier = new Supplier { Name = "Quarterly Supplier", Email = "q@test.com" };
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        var dto = new CreateQuarterlyPlanDto
        {
            SupplierId = supplier.Id,
            Quarter = 2,
            Year = 2026,
            PlannedBudget = 50000m,
            Notes = "Q2 plan"
        };

        var result = await _service.CreateQuarterlyPlanAsync(dto);

        result.Should().NotBeNull();
        result.SupplierId.Should().Be(supplier.Id);
        result.Quarter.Should().Be(2);
        result.PlannedBudget.Should().Be(50000m);
    }

    [Fact]
    public async Task GetBestOfferAsync_NoOffers_ReturnsNull()
    {
        var item = new StoreItem { Name = "Orphan Item", Price = 5m, Category = "Misc", StockQuantity = 0 };
        _context.StoreItems.Add(item);
        await _context.SaveChangesAsync();

        var result = await _service.GetBestOfferAsync(item.Id);

        result.Should().BeNull();
    }

    public void Dispose() => _context.Dispose();
}
