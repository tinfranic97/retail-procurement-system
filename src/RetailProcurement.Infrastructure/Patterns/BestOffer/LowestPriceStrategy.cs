using RetailProcurement.Core.Entities;

namespace RetailProcurement.Infrastructure.Patterns.BestOffer;

public class LowestPriceStrategy : IBestOfferStrategy
{
    public string CriteriaName => "Lowest Price";

    public SupplierStoreItem? SelectBestOffer(IEnumerable<SupplierStoreItem> offers)
        => offers.OrderBy(o => o.SupplierPrice).FirstOrDefault();
}
