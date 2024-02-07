using Interfaces;
using SupermarketReceipt;

namespace Strategies 
{
    public class TenPercentDiscountStrategy : IOfferStrategy
    {
        public Discount Apply(Offer offer, Product product, double quantity, double unitPrice)
        {
            return new Discount(product, offer.Argument + "% off", -quantity * unitPrice * offer.Argument / 100.0);
        }
    }
}