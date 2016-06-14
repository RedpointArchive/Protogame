using Microsoft.Xna.Framework;
using Protogame.ATFLevelEditor;

namespace Protogame
{
    public class DefaultDirectionalLightEntity : ComponentizedEntity
    {
        public DefaultDirectionalLightEntity(
            IEditorQuery<DefaultDirectionalLightEntity> editorQuery,
            StandardDirectionalLightComponent standardDirectionalLightComponent)
        {
            if (editorQuery.Mode != EditorQueryMode.BakingSchema)
            {
                RegisterPrivateComponent(standardDirectionalLightComponent);

                editorQuery.MapTransform(this, x => this.Transform = x);
                editorQuery.MapCustom(this, "diffuse", "diffuse", x => standardDirectionalLightComponent.LightColor = x, Color.White);
            }
        }
    }
}
