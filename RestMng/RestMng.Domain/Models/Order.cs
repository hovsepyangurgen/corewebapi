using RestMng.Core;

namespace RestMng.Domain
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int ClientID { get; set; }

        public float TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }


        public List<OrderItem> OrderItems { get; set; }
    }
}
