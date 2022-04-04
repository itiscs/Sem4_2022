using IdentityApp.Models;

namespace IdentityApp.Domain
{
    
    public class Cart
    {
        private int count;
        private List<CartLine> lineCollection = new List<CartLine>();

        public void AddItem(Product Product, int quantity)
        {
            CartLine? line = lineCollection
                .Where(g => g.Product.ProductId == Product.ProductId)
                .FirstOrDefault();

            if (line == null)
            {
                lineCollection.Add(new CartLine
                {
                    Product = Product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
            count++;
        }

        public void RemoveLine(Product Product)
        {
            lineCollection.RemoveAll(l => l.Product.ProductId == Product.ProductId);
        }

        public decimal ComputeTotalValue()
        {
            return lineCollection.Sum(e => e.Product.Price * e.Quantity);

        }
        public void Clear()
        {
            lineCollection.Clear();
        }

        public IEnumerable<CartLine> Lines
        {
            get { return lineCollection; }
            set { lineCollection = (List<CartLine>) value; }
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
        }
    }

    
    public class CartLine
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
