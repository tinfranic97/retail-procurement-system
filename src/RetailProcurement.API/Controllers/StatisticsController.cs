using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailProcurement.Core.DTOs.Statistics;
using RetailProcurement.Core.Interfaces.Services;

namespace RetailProcurement.API.Controllers;

[ApiController]
[Route("api/statistics")]
[Produces("application/json")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _service;

    public StatisticsController(IStatisticsService service) => _service = service;

    /// <summary>Retrieve sales statistics for a specific supplier.</summary>
    [HttpGet("supplier/{id:int}")]
    [ProducesResponseType(typeof(SupplierStatisticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSupplierStatistics(int id)
    {
        var stats = await _service.GetSupplierStatisticsAsync(id);
        return stats is null ? NotFound() : Ok(stats);
    }

    /// <summary>Retrieve the best offer for a specific product.</summary>
    [HttpGet("best-offer/{productId:int}")]
    [ProducesResponseType(typeof(BestOfferDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBestOffer(int productId)
    {
        var offer = await _service.GetBestOfferAsync(productId);
        return offer is null ? NotFound() : Ok(offer);
    }

    /// <summary>Plan suppliers for the upcoming quarter.</summary>
    [HttpPost("quarterly-plan")]
    [Authorize]
    [ProducesResponseType(typeof(QuarterlyPlanDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateQuarterlyPlan([FromBody] CreateQuarterlyPlanDto dto)
    {
        try
        {
            var plan = await _service.CreateQuarterlyPlanAsync(dto);
            return CreatedAtAction(nameof(GetCurrentQuarterlyPlan), plan);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Retrieve planned suppliers for the current quarter.</summary>
    [HttpGet("quarterly-plan")]
    [ProducesResponseType(typeof(IEnumerable<QuarterlyPlanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentQuarterlyPlan()
    {
        var plans = await _service.GetCurrentQuarterlyPlanAsync();
        return Ok(plans);
    }
}
