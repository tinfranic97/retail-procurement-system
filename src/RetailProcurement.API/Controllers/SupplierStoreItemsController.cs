using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailProcurement.Core.DTOs.Statistics;
using RetailProcurement.Core.Interfaces.Services;

namespace RetailProcurement.API.Controllers;

[ApiController]
[Route("api/supplier-store-items")]
[Produces("application/json")]
public class SupplierStoreItemsController : ControllerBase
{
    private readonly ISupplierStoreItemService _service;

    public SupplierStoreItemsController(ISupplierStoreItemService service) => _service = service;

    /// <summary>Retrieve all supplier-store item relationships.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SupplierStoreItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var relations = await _service.GetAllAsync();
        return Ok(relations);
    }

    /// <summary>Create a relationship between a supplier and a store item.</summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(SupplierStoreItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateSupplierStoreItemDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return StatusCode(StatusCodes.Status201Created, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Remove a relationship between a supplier and a store item.</summary>
    [HttpDelete("{supplierId:int}/{storeItemId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int supplierId, int storeItemId)
    {
        var deleted = await _service.DeleteAsync(supplierId, storeItemId);
        return deleted ? NoContent() : NotFound();
    }
}
