namespace RestMng.Core
{
    public interface IEntity
    {
        //int Id { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
