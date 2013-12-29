namespace RavendbStalenessConsistency
{
    public class Invoice
    {
        public string Id { get; private set; }

        public decimal Price { get; private set; }
        public decimal Fees { get; private set; }
        public decimal TotalValue { get; private set; }

        public void SetPrices(decimal price, decimal fees)
        {
            Price = price;
            Fees = fees;

            TotalValue = Price + Fees;
        }
    }
}