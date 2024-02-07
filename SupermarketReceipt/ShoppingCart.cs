using System.Collections.Generic;
using System.Globalization;
using Interfaces;
using Strategies;

namespace SupermarketReceipt
{
    public class ShoppingCart
    {
        private readonly List<ProductQuantity> _items = new List<ProductQuantity>();
        private readonly Dictionary<Product, double> _productQuantities = new Dictionary<Product, double>();
        private static readonly CultureInfo Culture = CultureInfo.CreateSpecificCulture("en-GB");


        public List<ProductQuantity> GetItems()
        {
            return new List<ProductQuantity>(_items);
        }

        public void AddItem(Product product)
        {
            AddItemQuantity(product, 1.0);
        }


        public void AddItemQuantity(Product product, double quantity)
        {
            _items.Add(new ProductQuantity(product, quantity));
            if (_productQuantities.ContainsKey(product))
            {
                var newAmount = _productQuantities[product] + quantity;
                _productQuantities[product] = newAmount;
            }
            else
            {
                _productQuantities.Add(product, quantity);
            }
        }

        public void HandleOffers(Receipt receipt, Dictionary<Product, Offer> offers, SupermarketCatalog catalog)
        {
            foreach (var p in _productQuantities.Keys)
            {
                var quantity = _productQuantities[p];
                var quantityAsInt = (int) quantity;
                if (offers.ContainsKey(p))
                {
                    var offer = offers[p];
                    var unitPrice = catalog.GetUnitPrice(p);
                    
                    IOfferStrategy strategy = GetOfferStrategy(offer.OfferType);
                    Discount discount = strategy.Apply(offer, p, quantity, unitPrice);
                    
                    if (discount != null)
                        receipt.AddDiscount(discount);
                }
            }
        }


        private IOfferStrategy GetOfferStrategy(SpecialOfferType offerType)
        {
            switch (offerType)
            {
                case SpecialOfferType.ThreeForTwo:
                    return new ThreeForTwoOfferStrategy();
                case SpecialOfferType.TenPercentDiscount:
                    return new TenPercentDiscountStrategy();
                case SpecialOfferType.TwoForAmount:
                    return new NForAmountStrategy(2);
                case SpecialOfferType.FiveForAmount:
                    return new NForAmountStrategy(5);
                default:
                    return null;
            }
        }



        // private void ThreeForTwoOffer(ref Discount discount, Product product, int productQuantity, double productUnitPrice)
        // {
        //     if(productQuantity > 2)
        //     {
        //         var discountAmount = productQuantity * productUnitPrice - (productQuantity / 3 * 2 * productUnitPrice + productQuantity % 3 * productUnitPrice);
        //         discount = new Discount(product, "3 for 2", -discountAmount);
        //     }
        // }

        // private void NForAmountOffer(ref Discount discount, Offer offer, Product product, int n, int productQuantity, double productUnitPrice)
        // {
        //     if(productQuantity >= n)
        //     {
        //         var totalWithoutOffer = productUnitPrice * productQuantity;
        //         var totalWithOffer = offer.Argument * (productQuantity / n) + productUnitPrice * (productQuantity % n);
        //         var discountTotal = totalWithoutOffer - totalWithOffer;
        //         discount = new Discount(product, n + " for " + PrintPrice(offer.Argument), -discountTotal);
        //     }
        // }

        // private void TenPercentDiscountOffer(ref Discount discount, Offer offer, Product product, double productQuantity, double productUnitPrice)
        // {
        //     discount = new Discount(product, offer.Argument + "% off", -productQuantity * productUnitPrice * offer.Argument / 100.0);
        // }
        
        // private string PrintPrice(double price)
        // {
        //     return price.ToString("N2", Culture);
        // }
    }
}