using RetailProcurement.Core.DTOs.StoreItem;
using RetailProcurement.Core.Entities;
using RetailProcurement.Core.Interfaces.Repositories;
using RetailProcurement.Core.Interfaces.Services;

namespace RetailProcurement.Infrastructure.Services;

public class StoreItemService : IStoreItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public StoreItemService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<StoreItemDto>> GetAllAsync()
    {
        var items = await _unitOfWork.StoreItems.GetAllAsync();
        return items.Select(MapToDto);
    }

    public async Task<StoreItemDto?> GetByIdAsync(int id)
    {
        var item = await _unitOfWork.StoreItems.GetByIdAsync(id);
        return item is null ? null : MapToDto(item);
    }

    public async Task<StoreItemDto> CreateAsync(CreateStoreItemDto dto)
    {
        var item = new StoreItem
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Category = dto.Category,
            StockQuantity = dto.StockQuantity
        };

        await _unitOfWork.StoreItems.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(item);
    }

    public async Task<StoreItemDto?> UpdateAsync(int id, UpdateStoreItemDto dto)
    {
        var item = await _unitOfWork.StoreItems.GetByIdAsync(id);
        if (item is null) return null;

        item.Name = dto.Name;
        item.Description = dto.Description;
        item.Price = dto.Price;
        item.Category = dto.Category;
        item.StockQuantity = dto.StockQuantity;

        await _unitOfWork.StoreItems.UpdateAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(item);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _unitOfWork.StoreItems.GetByIdAsync(id);
        if (item is null) return false;

        await _unitOfWork.StoreItems.DeleteAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static StoreItemDto MapToDto(StoreItem item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description,
        Price = item.Price,
        Category = item.Category,
        StockQuantity = item.StockQuantity,
        CreatedAt = item.CreatedAt,
        UpdatedAt = item.UpdatedAt
    };
}
