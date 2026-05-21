using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailProcurement.API.Controllers;
using RetailProcurement.Core.DTOs.Statistics;
using RetailProcurement.Core.Interfaces.Services;

namespace RetailProcurement.Tests.Unit.Controllers;

public class StatisticsControllerTests
{
    private readonly Mock<IStatisticsService> _serviceMock = new();
    private readonly StatisticsController _controller;

    public StatisticsControllerTests() => _controller = new StatisticsController(_serviceMock.Object);

    [Fact]
    public async Task GetSupplierStatistics_ExistingSupplier_ReturnsOk()
    {
        var stats = new SupplierStatisticsDto { SupplierId = 1, SupplierName = "A", TotalItemsSold = 50 };
        _serviceMock.Setup(s => s.GetSupplierStatisticsAsync(1)).ReturnsAsync(stats);

        var result = await _controller.GetSupplierStatistics(1);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(stats);
    }

    [Fact]
    public async Task GetSupplierStatistics_NotFound_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetSupplierStatisticsAsync(99)).ReturnsAsync((SupplierStatisticsDto?)null);

        var result = await _controller.GetSupplierStatistics(99);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetBestOffer_ExistingProduct_ReturnsOk()
    {
        var offer = new BestOfferDto { StoreItemId = 1, SupplierId = 2, SupplierPrice = 9.99m };
        _serviceMock.Setup(s => s.GetBestOfferAsync(1)).ReturnsAsync(offer);

        var result = await _controller.GetBestOffer(1);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(offer);
    }

    [Fact]
    public async Task GetBestOffer_NoOffers_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetBestOfferAsync(99)).ReturnsAsync((BestOfferDto?)null);

        var result = await _controller.GetBestOffer(99);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetCurrentQuarterlyPlan_ReturnsOkWithPlans()
    {
        var plans = new List<QuarterlyPlanDto>
        {
            new() { Id = 1, SupplierId = 1, Quarter = 2, Year = 2026 }
        };
        _serviceMock.Setup(s => s.GetCurrentQuarterlyPlanAsync()).ReturnsAsync(plans);

        var result = await _controller.GetCurrentQuarterlyPlan();

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(plans);
    }
}
