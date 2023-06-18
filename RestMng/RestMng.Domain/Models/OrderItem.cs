namespace RestMng.Domain
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }

        public int OrderID { get; set; }
        public int ItemID { get; set; }

        public float Quantity { get; set; }
        public float Subtotal { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
