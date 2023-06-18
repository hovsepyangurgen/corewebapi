using RestMng.Core;

namespace RestMng.Domain
{
    public class OrderItems: IEntity
    {
        public int OrderItemID { get; set; }

        public int OrderID { get; set; }
        public int ItemID { get; set; }

        public float Quantity { get; set; }
        public float Subtotal { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }


        public Orders Orders { get; set; }
    }
}
