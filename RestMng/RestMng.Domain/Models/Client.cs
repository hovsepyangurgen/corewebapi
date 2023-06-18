using RestMng.Core;

namespace RestMng.Domain
{
    public class Client
    {
        public int ClientID { get; set; }
        public string Name { get; set; }
        public ClientType Role { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
