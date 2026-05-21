using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RetailProcurement.Core.DTOs.StoreItem;
using RetailProcurement.Infrastructure.Data;
using RetailProcurement.Infrastructure.Repositories;
using RetailProcurement.Infrastructure.Services;

namespace RetailProcurement.Tests.Unit.Services;

public class StoreItemServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly StoreItemService _service;

    public StoreItemServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _service = new StoreItemService(new UnitOfWork(_context));
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        var result = await _service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_ValidDto_PersistsAndReturnsDto()
    {
        var dto = new CreateStoreItemDto
        {
            Name = "Test Item",
            Description = "A test item",
            Price = 9.99m,
            Category = "Electronics",
            StockQuantity = 10
        };

        var result = await _service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Test Item");
        result.Price.Should().Be(9.99m);

        var fromDb = await _context.StoreItems.FindAsync(result.Id);
        fromDb.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ExistingItem_ReturnsDto()
    {
        var dto = new CreateStoreItemDto { Name = "Widget", Price = 5m, Category = "Tools", StockQuantity = 5 };
        var created = await _service.CreateAsync(dto);

        var result = await _service.GetByIdAsync(created.Id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Widget");
    }

    [Fact]
    public async Task GetByIdAsync_NonExisting_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(9999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ExistingItem_UpdatesFields()
    {
        var createDto = new CreateStoreItemDto { Name = "Old Name", Price = 5m, Category = "Tools", StockQuantity = 5 };
        var created = await _service.CreateAsync(createDto);

        var updateDto = new UpdateStoreItemDto
        {
            Name = "New Name",
            Description = "Updated description",
            Price = 15m,
            Category = "Electronics",
            StockQuantity = 20
        };

        var result = await _service.UpdateAsync(created.Id, updateDto);

        result.Should().NotBeNull();
        result!.Name.Should().Be("New Name");
        result.Price.Should().Be(15m);
        result.Category.Should().Be("Electronics");
    }

    [Fact]
    public async Task DeleteAsync_ExistingItem_RemovesFromDatabase()
    {
        var dto = new CreateStoreItemDto { Name = "To Delete", Price = 1m, Category = "Misc", StockQuantity = 1 };
        var created = await _service.CreateAsync(dto);

        var deleted = await _service.DeleteAsync(created.Id);

        deleted.Should().BeTrue();
        var fromDb = await _context.StoreItems.FindAsync(created.Id);
        fromDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_ReturnsFalse()
    {
        var result = await _service.DeleteAsync(9999);

        result.Should().BeFalse();
    }

    public void Dispose() => _context.Dispose();
}
