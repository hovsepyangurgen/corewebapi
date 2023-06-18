namespace ProManAPI
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Available { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
