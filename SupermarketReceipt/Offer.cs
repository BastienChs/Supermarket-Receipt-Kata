namespace SupermarketReceipt
{
    public enum SpecialOfferType
    {
        ThreeForTwo,
        TenPercentDiscount,
        TwoForAmount,
        FiveForAmount,
        Bundle
    }

    public class Offer
    {
        private Product _product;
        public SpecialOfferType OfferType { get; }
        public double Argument { get; }
        public int MinimumQuantity { get; }

        public Offer(SpecialOfferType offerType, Product product, double argument)
        {
            OfferType = offerType;
            Argument = argument;
            _product = product;
        }

    }
}