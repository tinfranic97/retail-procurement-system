using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using RetailProcurement.API.Controllers;
using RetailProcurement.API.Hubs;
using RetailProcurement.Core.DTOs.StoreItem;
using RetailProcurement.Core.Interfaces.Services;

namespace RetailProcurement.Tests.Unit.Controllers;

public class StoreItemsControllerTests
{
    private readonly Mock<IStoreItemService> _serviceMock = new();
    private readonly Mock<IHubContext<ProcurementHub>> _hubMock = new();
    private readonly StoreItemsController _controller;

    public StoreItemsControllerTests()
    {
        var clientsMock = new Mock<IHubClients>();
        var clientProxyMock = new Mock<IClientProxy>();
        clientsMock.Setup(c => c.All).Returns(clientProxyMock.Object);
        _hubMock.Setup(h => h.Clients).Returns(clientsMock.Object);
        _controller = new StoreItemsController(_serviceMock.Object, _hubMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithItems()
    {
        var items = new List<StoreItemDto>
        {
            new() { Id = 1, Name = "Widget", Price = 9.99m, Category = "Tools" },
            new() { Id = 2, Name = "Gadget", Price = 19.99m, Category = "Electronics" }
        };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(items);

        var result = await _controller.GetAll();

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(items);
    }

    [Fact]
    public async Task GetById_ExistingItem_ReturnsOk()
    {
        var item = new StoreItemDto { Id = 1, Name = "Widget", Price = 9.99m };
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(item);

        var result = await _controller.GetById(1);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(item);
    }

    [Fact]
    public async Task GetById_NonExistingItem_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((StoreItemDto?)null);

        var result = await _controller.GetById(999);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ValidDto_ReturnsCreated()
    {
        var dto = new CreateStoreItemDto { Name = "New Item", Price = 5.00m, Category = "Food", StockQuantity = 10 };
        var created = new StoreItemDto { Id = 3, Name = "New Item", Price = 5.00m, Category = "Food" };
        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        var result = await _controller.Create(dto);

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Update_ExistingItem_ReturnsOk()
    {
        var dto = new UpdateStoreItemDto { Name = "Updated", Price = 15.00m, Category = "Tools", StockQuantity = 5 };
        var updated = new StoreItemDto { Id = 1, Name = "Updated", Price = 15.00m };
        _serviceMock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(updated);

        var result = await _controller.Update(1, dto);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(updated);
    }

    [Fact]
    public async Task Update_NonExistingItem_ReturnsNotFound()
    {
        var dto = new UpdateStoreItemDto { Name = "X", Price = 1m, Category = "X", StockQuantity = 0 };
        _serviceMock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync((StoreItemDto?)null);

        var result = await _controller.Update(999, dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ExistingItem_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_NonExistingItem_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);

        var result = await _controller.Delete(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
