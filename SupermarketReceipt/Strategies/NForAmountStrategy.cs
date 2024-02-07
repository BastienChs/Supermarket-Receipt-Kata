using System;
using System.Globalization;
using Interfaces;
using SupermarketReceipt;

namespace Strategies
{
    public class NForAmountStrategy : IOfferStrategy
    {
        private static readonly CultureInfo Culture = CultureInfo.CreateSpecificCulture("en-GB");
        private readonly int minimumQuantity;

        public NForAmountStrategy(int minimumQuantity)
        {
            this.minimumQuantity = minimumQuantity;
        }

        /// <summary>
        /// Strategy to define a global price for a certain amount of a product.
        /// </summary>
        /// <param name="offer">Actual offer</param>
        /// <param name="product">Product subject to discount</param>
        /// <param name="n">Minmum quantity to apply the discount</param>
        /// <param name="quantity">Product quantity bought by the customer</param>
        /// <param name="unitPrice">Product price bought by the customer</param>
        /// <returns></returns>
        public Discount Apply(Offer offer, Product product, double quantity, double unitPrice)
        {
            //If we have enough quantity to apply the discount
            int quantityAsInt = (int) quantity;
            if(quantityAsInt >= minimumQuantity)
            {
                var totalWithoutOffer = unitPrice * quantityAsInt;
                var totalWithOffer = offer.Argument * (quantityAsInt / minimumQuantity) + unitPrice * (quantityAsInt % minimumQuantity);
                var discountTotal = totalWithoutOffer - totalWithOffer;
                return new Discount(product, minimumQuantity + " for " + PrintPrice(offer.Argument), -discountTotal);
            }
            return null;
        }

        private string PrintPrice(double price)
        {
            return price.ToString("N2", Culture);
        }
    }
}