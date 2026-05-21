using RetailProcurement.Core.Entities;

namespace RetailProcurement.Infrastructure.Patterns.BestOffer;

public interface IBestOfferStrategy
{
    string CriteriaName { get; }
    SupplierStoreItem? SelectBestOffer(IEnumerable<SupplierStoreItem> offers);
}
