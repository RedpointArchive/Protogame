
namespace Protogame
{
    public interface IDynamicRenderingEntity : IEntity
    {
        bool ShouldRender(IWorld world);
    }
}
