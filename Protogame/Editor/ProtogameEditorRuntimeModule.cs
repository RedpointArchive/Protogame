using Protogame.ATFLevelEditor;
using Protoinject;

namespace Protogame
{
    public class ProtogameEditorRuntimeModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind(typeof (IEditorQuery<>)).To(typeof (SpawningEditorQuery<>));
        }
    }
}
