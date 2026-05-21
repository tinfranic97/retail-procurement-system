using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailProcurement.API.Controllers;
using RetailProcurement.Core.DTOs.Supplier;
using RetailProcurement.Core.Interfaces.Services;

namespace RetailProcurement.Tests.Unit.Controllers;

public class SuppliersControllerTests
{
    private readonly Mock<ISupplierService> _serviceMock = new();
    private readonly SuppliersController _controller;

    public SuppliersControllerTests() => _controller = new SuppliersController(_serviceMock.Object);

    [Fact]
    public async Task GetAll_ReturnsOkWithSuppliers()
    {
        var suppliers = new List<SupplierDto>
        {
            new() { Id = 1, Name = "Supplier A", Email = "a@test.com" },
            new() { Id = 2, Name = "Supplier B", Email = "b@test.com" }
        };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(suppliers);

        var result = await _controller.GetAll();

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(suppliers);
    }

    [Fact]
    public async Task GetById_ExistingSupplier_ReturnsOk()
    {
        var supplier = new SupplierDto { Id = 1, Name = "Supplier A", Email = "a@test.com" };
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(supplier);

        var result = await _controller.GetById(1);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(supplier);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((SupplierDto?)null);

        var result = await _controller.GetById(99);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ValidDto_ReturnsCreated()
    {
        var dto = new CreateSupplierDto { Name = "New Supplier", Email = "new@test.com" };
        var created = new SupplierDto { Id = 3, Name = "New Supplier", Email = "new@test.com" };
        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        var result = await _controller.Create(dto);

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Delete_ExistingSupplier_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
    }
}
