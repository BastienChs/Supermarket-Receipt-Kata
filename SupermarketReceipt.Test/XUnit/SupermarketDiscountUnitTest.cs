using System.Collections.Generic;
using Xunit;
using FluentAssertions;

namespace SupermarketReceipt.Test.XUnit
{
    public class SupermarketDiscountUnitTest
    {
        [Fact]
        public void TenPercentDiscount_WhenCartContainsKilosOfApples_ShouldReturnTenPercentDiscountOnApplesTotalPrice()
        {
            // ARRANGE
            double appleUnitPrice = 1.99;
            double appleQuantity = 2.5;
            double offerArgument = 10.0;

            SupermarketCatalog catalog = new FakeCatalog();

            var apples = new Product("apples", ProductUnit.Kilo);
            catalog.AddProduct(apples, appleUnitPrice);

            var cart = new ShoppingCart();
            cart.AddItemQuantity(apples, appleQuantity);

            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.TenPercentDiscount, apples, offerArgument);

            var expectedTotalPrice = appleQuantity * appleUnitPrice * 0.9;

            List<Discount> expectedDiscounts = new List<Discount>()
            {
                new Discount(apples, offerArgument + "% off", -appleQuantity * appleUnitPrice * offerArgument / 100.0)
            };

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            Assert.Equal(expectedTotalPrice, receipt.GetTotalPrice());
            receipt.GetDiscounts().Should().BeEquivalentTo(expectedDiscounts);
            Assert.Single(receipt.GetItems());
            var receiptItem = receipt.GetItems()[0];
            Assert.Equal(apples, receiptItem.Product);
            Assert.Equal(appleUnitPrice, receiptItem.Price);
            Assert.Equal(appleQuantity * appleUnitPrice, receiptItem.TotalPrice);
            Assert.Equal(appleQuantity, receiptItem.Quantity);
        }

        [Fact]
        public void TenPercentDiscount_WhenCartContainsKilosOfAppleAndOtherProduct_ShouldReturnTenPercentDiscountOnApplesTotalPrice()
        {
            // ARRANGE
            double appleUnitPrice = 1.99;
            double appleQuantity = 2.5;
            double appleOfferArgument = 10.0;
            double toothbrushUnitPrice = 0.99;
            double toothbrushQuantity = 3.0;

            SupermarketCatalog catalog = new FakeCatalog();

            var apples = new Product("apples", ProductUnit.Kilo);
            catalog.AddProduct(apples, appleUnitPrice);

            var other = new Product("toothbrush", ProductUnit.Each);
            catalog.AddProduct(other, toothbrushUnitPrice);

            var cart = new ShoppingCart();
            cart.AddItemQuantity(apples, appleQuantity);
            cart.AddItemQuantity(other, toothbrushQuantity);

            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.TenPercentDiscount, apples, appleOfferArgument);

            var expectedTotalPrice = appleQuantity * appleUnitPrice * 0.9 + toothbrushQuantity * toothbrushUnitPrice;

            List<Discount> expectedDiscounts = new List<Discount>()
            {
                new Discount(apples, appleOfferArgument + "% off", -appleQuantity * appleUnitPrice * appleOfferArgument / 100.0)
            };

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            Assert.Equal(expectedTotalPrice, receipt.GetTotalPrice());
            receipt.GetDiscounts().Should().BeEquivalentTo(expectedDiscounts);
            Assert.Equal(2, receipt.GetItems().Count);
            var receiptItem = receipt.GetItems()[0];
            Assert.Equal(apples, receiptItem.Product);
            Assert.Equal(appleUnitPrice, receiptItem.Price);
            Assert.Equal(appleQuantity * appleUnitPrice, receiptItem.TotalPrice);
            Assert.Equal(appleQuantity, receiptItem.Quantity);
            receiptItem = receipt.GetItems()[1];
            Assert.Equal(other, receiptItem.Product);
            Assert.Equal(toothbrushUnitPrice, receiptItem.Price);
            Assert.Equal(toothbrushQuantity * toothbrushUnitPrice, receiptItem.TotalPrice);
            Assert.Equal(toothbrushQuantity, receiptItem.Quantity);
        }

        [Fact]
        public void ThreeForTwo_WhenCartContainsThreeSameDiscountProducts_PriceShouldEqualTwo()
        {
            // ARRANGE
            int quantity = 3;
            double unitPrice = 0.99;

            SupermarketCatalog catalog = new FakeCatalog();
            var toothbrush = new Product("toothbrush", ProductUnit.Each);
            catalog.AddProduct(toothbrush, unitPrice);

            var cart = new ShoppingCart();
            cart.AddItemQuantity(toothbrush, quantity);

            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.ThreeForTwo, toothbrush, unitPrice);

            var numberOfXs = quantity / 3;
            var discountAmount = quantity * unitPrice - (numberOfXs * 2 * unitPrice + quantity % 3 * unitPrice);
            Discount discount = new Discount(toothbrush, "3 for 2", -discountAmount);
            List<Discount> expectedDiscounts = new List<Discount> { discount };

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            Assert.Equal(1.98, receipt.GetTotalPrice());
            receipt.GetDiscounts().Should().BeEquivalentTo(expectedDiscounts);
            Assert.Single(receipt.GetItems());
            var receiptItem = receipt.GetItems()[0];
            Assert.Equal(toothbrush, receiptItem.Product);
            Assert.Equal(0.99, receiptItem.Price);
            Assert.Equal(unitPrice * quantity, receiptItem.TotalPrice);
            Assert.Equal(3, receiptItem.Quantity);
        }

        [Fact]
        public void ThreeForTwo_WhenCartContainsThreeSameDiscountProductsAndAnotherProduct_PriceShouldEqualTwoPlusSumPriceOfOtherProducts()
        {
            // ARRANGE
            double toothbrushUnitPrice = 0.99;
            double toothbrushQuantity = 3.0;
            double appleKiloPrice = 1.99;
            double appleQuantity = 2.0;

            SupermarketCatalog catalog = new FakeCatalog();
            var toothbrush = new Product("toothbrush", ProductUnit.Each);
            catalog.AddProduct(toothbrush, toothbrushUnitPrice);
            var apples = new Product("apples", ProductUnit.Kilo);
            catalog.AddProduct(apples, appleKiloPrice);

            var cart = new ShoppingCart();
            cart.AddItemQuantity(toothbrush, toothbrushQuantity);
            cart.AddItemQuantity(apples, appleQuantity);

            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.ThreeForTwo, toothbrush, toothbrushUnitPrice);

            var numberOfXs = toothbrushQuantity / 3;
            var discountAmount = toothbrushQuantity * toothbrushUnitPrice - (numberOfXs * 2 * toothbrushUnitPrice + toothbrushQuantity % 3 * toothbrushUnitPrice);
            Discount discount = new Discount(toothbrush, "3 for 2", -discountAmount);
            List<Discount> expectedDiscounts = new List<Discount> { discount };

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            Assert.Equal(1.98 + appleQuantity * appleKiloPrice, receipt.GetTotalPrice(), 2);
            receipt.GetDiscounts().Should().BeEquivalentTo(expectedDiscounts);
            Assert.Equal(2, receipt.GetItems().Count);
            var receiptItem = receipt.GetItems()[0];
            Assert.Equal(toothbrush, receiptItem.Product);
            Assert.Equal(0.99, receiptItem.Price);
            Assert.Equal(toothbrushUnitPrice * toothbrushQuantity, receiptItem.TotalPrice);
            Assert.Equal(toothbrushQuantity, receiptItem.Quantity);
            receiptItem = receipt.GetItems()[1];
            Assert.Equal(apples, receiptItem.Product);
            Assert.Equal(appleKiloPrice, receiptItem.Price);
            Assert.Equal(appleKiloPrice * appleQuantity, receiptItem.TotalPrice);
            Assert.Equal(appleQuantity, receiptItem.Quantity);
        }

        [Fact]
        public void TwoForAmount_WhenCartContainsTwoSameDiscountProducts_PriceShouldEqualOfferArgument()
        {
            // ARRANGE
            int quantity = 2;
            double unitPrice = 0.99;
            double offerArgument = 1.5;

            SupermarketCatalog catalog = new FakeCatalog();
            var toothbrush = new Product("toothbrush", ProductUnit.Each);
            catalog.AddProduct(toothbrush, unitPrice);

            var cart = new ShoppingCart();
            cart.AddItemQuantity(toothbrush, quantity);

            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.TwoForAmount, toothbrush, offerArgument);

            var total = offerArgument * (quantity / 2) + quantity % 2 * unitPrice;
            var discountN = unitPrice * quantity - total;
            Discount discount = new Discount(toothbrush, "2 for 1.50", -discountN);
            List<Discount> expectedDiscounts = new List<Discount> { discount };

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            Assert.Equal(1.5, receipt.GetTotalPrice());
            receipt.GetDiscounts().Should().BeEquivalentTo(expectedDiscounts);
            Assert.Single(receipt.GetItems());
            var receiptItem = receipt.GetItems()[0];
            Assert.Equal(toothbrush, receiptItem.Product);
            Assert.Equal(0.99, receiptItem.Price);
            Assert.Equal(unitPrice * quantity, receiptItem.TotalPrice);
            Assert.Equal(2, receiptItem.Quantity);
        }

        [Fact]
        public void TwoForAmount_WhenCartContainsTwoSameDiscountProductsAndAnotherProduct_PriceShouldEqualOfferArgumentPlusSumPriceOfOtherProducts()
        {
            // ARRANGE
            double toothbrushUnitPrice = 0.99;
            double toothbrushQuantity = 2.0;
            double appleKiloPrice = 1.99;
            double appleQuantity = 2.0;

            SupermarketCatalog catalog = new FakeCatalog();
            var toothbrush = new Product("toothbrush", ProductUnit.Each);
            catalog.AddProduct(toothbrush, toothbrushUnitPrice);
            var apples = new Product("apples", ProductUnit.Kilo);
            catalog.AddProduct(apples, appleKiloPrice);

            var cart = new ShoppingCart();
            cart.AddItemQuantity(toothbrush, toothbrushQuantity);
            cart.AddItemQuantity(apples, appleQuantity);

            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.TwoForAmount, toothbrush, 1.5);

            var total = 1.5 * (toothbrushQuantity / 2) + toothbrushQuantity % 2 * toothbrushUnitPrice;
            var discountN = toothbrushUnitPrice * toothbrushQuantity - total;
            Discount discount = new Discount(toothbrush, "2 for 1.50", -discountN);
            List<Discount> expectedDiscounts = new List<Discount> { discount };

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            Assert.Equal(1.5 + appleQuantity * appleKiloPrice, receipt.GetTotalPrice(), 2);
            receipt.GetDiscounts().Should().BeEquivalentTo(expectedDiscounts);
            Assert.Equal(2, receipt.GetItems().Count);
            var receiptItem = receipt.GetItems()[0];
            Assert.Equal(toothbrush, receiptItem.Product);
            Assert.Equal(0.99, receiptItem.Price);
            Assert.Equal(toothbrushUnitPrice * toothbrushQuantity, receiptItem.TotalPrice);
            Assert.Equal(toothbrushQuantity, receiptItem.Quantity);
            receiptItem = receipt.GetItems()[1];
            Assert.Equal(apples, receiptItem.Product);
            Assert.Equal(appleKiloPrice, receiptItem.Price);
            Assert.Equal(appleKiloPrice * appleQuantity, receiptItem.TotalPrice);
            Assert.Equal(appleQuantity, receiptItem.Quantity);
        }

        [Fact]
        public void FiveForAmount_WhenCartContainsFiveSameDiscountProducts_PriceShouldEqualOfferArgument()
        {
            // ARRANGE
            int quantity = 5;
            double unitPrice = 0.99;
            double offerArgument = 4.5;

            SupermarketCatalog catalog = new FakeCatalog();
            var toothbrush = new Product("toothbrush", ProductUnit.Each);
            catalog.AddProduct(toothbrush, unitPrice);

            var cart = new ShoppingCart();
            cart.AddItemQuantity(toothbrush, quantity);

            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.FiveForAmount, toothbrush, offerArgument);

            var total = offerArgument * (quantity / 5) + quantity % 5 * unitPrice;
            var discountN = unitPrice * quantity - total;
            Discount discount = new Discount(toothbrush, "5 for 4.50", -discountN);
            List<Discount> expectedDiscounts = new List<Discount> { discount };

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            Assert.Equal(4.5, receipt.GetTotalPrice());
            receipt.GetDiscounts().Should().BeEquivalentTo(expectedDiscounts);
            Assert.Single(receipt.GetItems());
            var receiptItem = receipt.GetItems()[0];
            Assert.Equal(toothbrush, receiptItem.Product);
            Assert.Equal(0.99, receiptItem.Price);
            Assert.Equal(unitPrice * quantity, receiptItem.TotalPrice);
            Assert.Equal(5, receiptItem.Quantity);
        }

        [Fact]
        public void FiveForAmount_WhenCartContainsFiveSameDiscountProductsAndAnotherProduct_PriceShouldEqualOfferArgumentPlusSumPriceOfOtherProducts()
        {
            // ARRANGE
            double toothbrushUnitPrice = 0.99;
            double toothbrushQuantity = 5.0;
            double appleKiloPrice = 1.99;
            double appleQuantity = 2.0;

            SupermarketCatalog catalog = new FakeCatalog();
            var toothbrush = new Product("toothbrush", ProductUnit.Each);
            catalog.AddProduct(toothbrush, toothbrushUnitPrice);
            var apples = new Product("apples", ProductUnit.Kilo);
            catalog.AddProduct(apples, appleKiloPrice);

            var cart = new ShoppingCart();
            cart.AddItemQuantity(toothbrush, toothbrushQuantity);
            cart.AddItemQuantity(apples, appleQuantity);

            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.FiveForAmount, toothbrush, 4.5);

            var total = 4.5 * (toothbrushQuantity / 5) + toothbrushQuantity % 5 * toothbrushUnitPrice;
            var discountN = toothbrushUnitPrice * toothbrushQuantity - total;
            Discount discount = new Discount(toothbrush, "5 for 4.50", -discountN);
            List<Discount> expectedDiscounts = new List<Discount> { discount };

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            Assert.Equal(4.5 + appleQuantity * appleKiloPrice, receipt.GetTotalPrice(), 2);
            receipt.GetDiscounts().Should().BeEquivalentTo(expectedDiscounts);
            Assert.Equal(2, receipt.GetItems().Count);
            var receiptItem = receipt.GetItems()[0];
            Assert.Equal(toothbrush, receiptItem.Product);
            Assert.Equal(0.99, receiptItem.Price);
            Assert.Equal(toothbrushUnitPrice * toothbrushQuantity, receiptItem.TotalPrice);
            Assert.Equal(toothbrushQuantity, receiptItem.Quantity);
            receiptItem = receipt.GetItems()[1];
            Assert.Equal(apples, receiptItem.Product);
            Assert.Equal(appleKiloPrice, receiptItem.Price);
            Assert.Equal(appleKiloPrice * appleQuantity, receiptItem.TotalPrice);
            Assert.Equal(appleQuantity, receiptItem.Quantity);
        }

        [Fact]
        public void AddItem_WhenProductHasNoSpecialOffer_ShouldAddProductToCart()
        {
            // ARRANGE
            double productUnitPrice = 1.99;
            SupermarketCatalog catalog = new FakeCatalog();
            var product = new Product("product", ProductUnit.Each);
            catalog.AddProduct(product, productUnitPrice);
            var cart = new ShoppingCart();

            // ACT
            cart.AddItem(product);

            // ASSERT
            Assert.Single(cart.GetItems());
            var cartItem = cart.GetItems()[0];
            Assert.Equal(product, cartItem.Product);
            Assert.Equal(1.0, cartItem.Quantity);
        }

        [Fact]
        public void AddItemQuantity_WhenProductHasNoSpecialOffer_ShouldAddProductToCart()
        {
            // ARRANGE
            double productUnitPrice = 1.99;
            double productQuantity = 3.0;
            SupermarketCatalog catalog = new FakeCatalog();
            var product = new Product("product", ProductUnit.Each);
            catalog.AddProduct(product, productUnitPrice);
            var cart = new ShoppingCart();

            // ACT
            cart.AddItemQuantity(product, productQuantity);

            // ASSERT
            Assert.Single(cart.GetItems());
            var cartItem = cart.GetItems()[0];
            Assert.Equal(product, cartItem.Product);
            Assert.Equal(productQuantity, cartItem.Quantity);
        }

        [Fact]
        public void AddItemWithSpecialOfferTwoForAmount_WhenQuantityDoesNotMeetOfferRequirements_ShouldNotApplyDiscount()
        {
            // ARRANGE
            double productUnitPrice = 1.99;
            double productQuantity = 1.0;
            double offerArgument = 10.0;
            SupermarketCatalog catalog = new FakeCatalog();
            var product = new Product("product", ProductUnit.Each);
            catalog.AddProduct(product, productUnitPrice);
            var cart = new ShoppingCart();
            cart.AddItemQuantity(product, productQuantity);
            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.TwoForAmount, product, offerArgument);

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            Assert.Equal(productQuantity * productUnitPrice, receipt.GetTotalPrice());
            Assert.Empty(receipt.GetDiscounts());
        }

        [Fact]
        public void AddItemWithSpecialOfferFiveForAmount_WhenQuantityDoesNotMeetOfferRequirements_ShouldNotApplyDiscount()
        {
            // ARRANGE
            double productUnitPrice = 1.99;
            double productQuantity = 3.0;
            double offerArgument = 10.0;
            SupermarketCatalog catalog = new FakeCatalog();
            var product = new Product("product", ProductUnit.Each);
            catalog.AddProduct(product, productUnitPrice);
            var cart = new ShoppingCart();
            cart.AddItemQuantity(product, productQuantity);
            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.FiveForAmount, product, offerArgument);

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            Assert.Equal(productQuantity * productUnitPrice, receipt.GetTotalPrice());
            Assert.Empty(receipt.GetDiscounts());
        }

        [Fact]
        public void AddMultipleItemsWithDifferentSpecialOffers_ShouldApplyCorrectDiscounts()
        {
            // ARRANGE
            SupermarketCatalog catalog = new FakeCatalog();
            var product1 = new Product("product1", ProductUnit.Each);
            var product2 = new Product("product2", ProductUnit.Each);
            catalog.AddProduct(product1, 1.99);
            catalog.AddProduct(product2, 2.99);
            var cart = new ShoppingCart();
            cart.AddItemQuantity(product1, 3.0);
            cart.AddItemQuantity(product2, 3.0);
            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.ThreeForTwo, product1, 3.0);
            teller.AddSpecialOffer(SpecialOfferType.TenPercentDiscount, product2, 10.0);

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            double expectedTotal = (2 * 1.99) + (3 * 2.99 * 0.9); // 3 for 2 offer on product1 and 10% discount on product2
            Assert.Equal(expectedTotal, receipt.GetTotalPrice(), 2);
            Assert.Equal(2, receipt.GetDiscounts().Count);
        }

        [Fact]
        public void HandleOffers_WhenMultipleOffers_ShouldApplyAllDiscounts()
        {
            // ARRANGE
            double toothbrushUnitPrice = 1.99;
            double toothbrushQuantity = 3.0;
            double pastaUnitPrice = 2.99;
            double pastaQuantity = 3.0;
            double toothbrushOfferArgument = 3.0;
            double pastaOfferArgument = 10.0;

            SupermarketCatalog catalog = new FakeCatalog();
            var toothbrush = new Product("toothbrush", ProductUnit.Each);
            var pasta = new Product("pasta", ProductUnit.Each);
            catalog.AddProduct(toothbrush, toothbrushUnitPrice);
            catalog.AddProduct(pasta, pastaUnitPrice);
            var cart = new ShoppingCart();
            cart.AddItemQuantity(toothbrush, toothbrushQuantity);
            cart.AddItemQuantity(pasta, pastaQuantity);
            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.ThreeForTwo, toothbrush, toothbrushOfferArgument);
            teller.AddSpecialOffer(SpecialOfferType.TenPercentDiscount, pasta, pastaOfferArgument);

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            double expectedTotal = (2 * toothbrushUnitPrice) + (3 * pastaUnitPrice * 0.9); // 3 for 2 offer on toothbrush and 10% discount on pasta
            Assert.Equal(2, receipt.GetDiscounts().Count);
            Assert.Equal(expectedTotal, receipt.GetTotalPrice(), 2);
        }
    }
}