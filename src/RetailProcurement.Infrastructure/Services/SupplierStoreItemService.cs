using RetailProcurement.Core.DTOs.Statistics;
using RetailProcurement.Core.Entities;
using RetailProcurement.Core.Interfaces.Repositories;
using RetailProcurement.Core.Interfaces.Services;

namespace RetailProcurement.Infrastructure.Services;

public class SupplierStoreItemService : ISupplierStoreItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public SupplierStoreItemService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<SupplierStoreItemDto>> GetAllAsync()
    {
        var relations = await _unitOfWork.SupplierStoreItems.GetAllAsync();
        return relations.Select(MapToDto);
    }

    public async Task<SupplierStoreItemDto> CreateAsync(CreateSupplierStoreItemDto dto)
    {
        if (await _unitOfWork.SupplierStoreItems.ExistsAsync(dto.SupplierId, dto.StoreItemId))
            throw new InvalidOperationException("Relationship already exists.");

        if (!await _unitOfWork.Suppliers.ExistsAsync(dto.SupplierId))
            throw new KeyNotFoundException($"Supplier {dto.SupplierId} not found.");

        if (!await _unitOfWork.StoreItems.ExistsAsync(dto.StoreItemId))
            throw new KeyNotFoundException($"Store item {dto.StoreItemId} not found.");

        var entity = new SupplierStoreItem
        {
            SupplierId = dto.SupplierId,
            StoreItemId = dto.StoreItemId,
            SupplierPrice = dto.SupplierPrice,
            LeadTimeDays = dto.LeadTimeDays
        };

        await _unitOfWork.SupplierStoreItems.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        var saved = await _unitOfWork.SupplierStoreItems.GetAsync(dto.SupplierId, dto.StoreItemId);
        return MapToDto(saved!);
    }

    public async Task<bool> DeleteAsync(int supplierId, int storeItemId)
    {
        var entity = await _unitOfWork.SupplierStoreItems.GetAsync(supplierId, storeItemId);
        if (entity is null) return false;

        await _unitOfWork.SupplierStoreItems.DeleteAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static SupplierStoreItemDto MapToDto(SupplierStoreItem ssi) => new()
    {
        SupplierId = ssi.SupplierId,
        SupplierName = ssi.Supplier?.Name ?? string.Empty,
        StoreItemId = ssi.StoreItemId,
        StoreItemName = ssi.StoreItem?.Name ?? string.Empty,
        SupplierPrice = ssi.SupplierPrice,
        LeadTimeDays = ssi.LeadTimeDays,
        CreatedAt = ssi.CreatedAt
    };
}
