using RetailProcurement.Core.Entities;

namespace RetailProcurement.Infrastructure.Patterns.BestOffer;

public class FastestDeliveryStrategy : IBestOfferStrategy
{
    public string CriteriaName => "Fastest Delivery";

    public SupplierStoreItem? SelectBestOffer(IEnumerable<SupplierStoreItem> offers)
        => offers.OrderBy(o => o.LeadTimeDays).FirstOrDefault();
}
