using System.ComponentModel.DataAnnotations;

namespace RestMng.Domain
{
    public class Storage
    {
        public int ItemID { get; set; }

        [Range(0, float.MaxValue)]
        public float Quantity { get; set; }
    }
}
