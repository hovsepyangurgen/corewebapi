using RestMng.Core;

namespace RestMng.Domain
{
    public class Customers: IEntity
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

        public ICollection<Orders> Orders { get; set; }
    }
}
