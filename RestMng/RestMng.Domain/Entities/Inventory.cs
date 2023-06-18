using RestMng.Core;
using System.ComponentModel.DataAnnotations;

namespace RestMng.Domain
{
    public class Inventory : IEntity
    {
        public int ItemID { get; set; }

        [Range(0, float.MaxValue)]
        public float Quantity { get; set; }


        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

        public MenuItems MenuItems { get; set; }
    }
}
