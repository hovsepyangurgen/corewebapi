using RestMng.Core;

namespace RestMng.Domain
{
    public class MenuItems: IEntity
    {
        public int ItemID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }


        public Inventory Inventory { get; set; }
    }
}
