using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RetailProcurement.API.Hubs;
using RetailProcurement.Core.DTOs.StoreItem;
using RetailProcurement.Core.Interfaces.Services;

namespace RetailProcurement.API.Controllers;

[ApiController]
[Route("api/store-items")]
[Produces("application/json")]
public class StoreItemsController : ControllerBase
{
    private readonly IStoreItemService _service;
    private readonly IHubContext<ProcurementHub> _hubContext;

    public StoreItemsController(IStoreItemService service, IHubContext<ProcurementHub> hubContext)
    {
        _service = service;
        _hubContext = hubContext;
    }

    /// <summary>Retrieve all store items.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StoreItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    /// <summary>Retrieve a specific store item by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StoreItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>Add a new store item.</summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(StoreItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateStoreItemDto dto)
    {
        var created = await _service.CreateAsync(dto);
        await _hubContext.Clients.All.SendAsync("StoreItemCreated", created);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Update an existing store item.</summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(StoreItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStoreItemDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (updated is null) return NotFound();
        await _hubContext.Clients.All.SendAsync("StoreItemUpdated", updated);
        return Ok(updated);
    }

    /// <summary>Delete a store item.</summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        await _hubContext.Clients.All.SendAsync("StoreItemDeleted", id);
        return NoContent();
    }
}
