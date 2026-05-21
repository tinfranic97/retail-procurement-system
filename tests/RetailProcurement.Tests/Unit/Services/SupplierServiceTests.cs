using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RetailProcurement.Core.DTOs.Supplier;
using RetailProcurement.Infrastructure.Data;
using RetailProcurement.Infrastructure.Repositories;
using RetailProcurement.Infrastructure.Services;

namespace RetailProcurement.Tests.Unit.Services;

public class SupplierServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SupplierService _service;

    public SupplierServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _service = new SupplierService(new UnitOfWork(_context));
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsCreatedSupplier()
    {
        var dto = new CreateSupplierDto
        {
            Name = "Test Supplier",
            Email = "test@supplier.com",
            Phone = "123-456-7890",
            Address = "123 Main St",
            ContactPerson = "John Doe"
        };

        var result = await _service.CreateAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Test Supplier");
        result.Email.Should().Be("test@supplier.com");
    }

    [Fact]
    public async Task GetAllAsync_WithSuppliers_ReturnsAll()
    {
        await _service.CreateAsync(new CreateSupplierDto { Name = "A", Email = "a@a.com" });
        await _service.CreateAsync(new CreateSupplierDto { Name = "B", Email = "b@b.com" });

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ExistingSupplier_UpdatesData()
    {
        var created = await _service.CreateAsync(new CreateSupplierDto { Name = "Old", Email = "old@test.com" });

        var updateDto = new UpdateSupplierDto
        {
            Name = "New Name",
            Email = "new@test.com",
            Phone = "000",
            Address = "New Address",
            ContactPerson = "Jane"
        };

        var result = await _service.UpdateAsync(created.Id, updateDto);

        result.Should().NotBeNull();
        result!.Name.Should().Be("New Name");
        result.Email.Should().Be("new@test.com");
    }

    [Fact]
    public async Task DeleteAsync_ExistingSupplier_ReturnsTrue()
    {
        var created = await _service.CreateAsync(new CreateSupplierDto { Name = "Del", Email = "del@test.com" });

        var result = await _service.DeleteAsync(created.Id);

        result.Should().BeTrue();
    }

    public void Dispose() => _context.Dispose();
}
