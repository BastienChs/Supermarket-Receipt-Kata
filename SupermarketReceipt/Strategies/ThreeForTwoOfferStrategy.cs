using Interfaces;
using SupermarketReceipt;

namespace Strategies
{
    public class ThreeForTwoOfferStrategy : IOfferStrategy
    {
        public Discount Apply(Offer offer, Product product, double quantity, double unitPrice)
        {
            if(quantity > 2)
            {
                var discountAmount = quantity * unitPrice - (quantity / 3 * 2 * unitPrice + quantity % 3 * unitPrice);
                return new Discount(product, "3 for 2", -discountAmount);
            }
            return null;
        }
    }
}