using RestMng.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestMng.Domain
{
    public class Orders : IEntity
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int ClientID { get; set; }

        public float TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }


        public Clients Clients { get; set; }
        public Customers Customers { get; set; }
        public ICollection<OrderItems> OrderItems { get; set; }
    }
}
