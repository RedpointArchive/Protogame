using Ninject.Parameters;

namespace Protogame
{
    public interface IComponentHierarchyParameter : IParameter
    {
        void AddComponentAtPath(object[] path, object component);

        object[] GetComponentsUnderPath(object[] path, ComponentHierarchyPlannerDescendantMode includeDescendants);
    }

    public enum ComponentHierarchyPlannerDescendantMode
    {
        None,

        Immediate,

        Full
    }
}
