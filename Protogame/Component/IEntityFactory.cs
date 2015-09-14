namespace Protogame
{
    public interface IEntityFactory
    {
        object HierarchyRoot { get; set; }

        void PlanForEntityCreation<T>() where T : IEntity;

        IEntity[] CreateEntities();
    }
}
