using SupermarketReceipt;

namespace Interfaces
{
    public interface IOfferStrategy
    {
        Discount Apply(Offer offer, Product product, double quantity, double unitPrice);
    }
}