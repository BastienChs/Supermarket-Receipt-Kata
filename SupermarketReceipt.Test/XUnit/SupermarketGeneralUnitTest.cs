using System.Collections.Generic;
using Xunit;
using FluentAssertions;

namespace SupermarketReceipt.Test.XUnit
{
    public class SupermarketGeneralUnitTest
    {
        [Fact]
        public void AddItem_ShouldAddProductToCart()
        {
            // ARRANGE
            var product = new Product("product", ProductUnit.Each);
            var cart = new ShoppingCart();

            // ACT
            cart.AddItem(product);

            // ASSERT
            var items = cart.GetItems();
            Assert.Single(items);
            Assert.Equal(product, items[0].Product);
            Assert.Equal(1.0, items[0].Quantity);
        }

        //TODO: It actually fails
        
        // [Fact]
        // public void AddItemQuantity_ShouldUpdateQuantityOfExistingProductInCart()
        // {
        //     // ARRANGE
        //     var product = new Product("product", ProductUnit.Each);
        //     var cart = new ShoppingCart();
        //     cart.AddItemQuantity(product, 1.0);

        //     // ACT
        //     cart.AddItemQuantity(product, 2.0);

        //     // ASSERT
        //     var items = cart.GetItems();
        //     Assert.Single(items);
        //     Assert.Equal(product, items[0].Product);
        //     Assert.Equal(3.0, items[0].Quantity);
        // }
    }
}