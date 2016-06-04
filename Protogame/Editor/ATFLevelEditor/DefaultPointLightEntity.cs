using Microsoft.Xna.Framework;
using Protogame.ATFLevelEditor;

namespace Protogame
{
    public class DefaultPointLightEntity : ComponentizedEntity
    {
        public DefaultPointLightEntity(
            IEditorQuery<DefaultPointLightEntity> editorQuery,
            StandardPointLightComponent standardPointLightComponent)
        {
            if (editorQuery.Mode != EditorQueryMode.BakingSchema)
            {
                RegisterPrivateComponent(standardPointLightComponent);

                editorQuery.MapMatrix(this, x => this.LocalMatrix = x);
                editorQuery.MapCustom(this, "diffuse", "diffuse", x => standardPointLightComponent.LightColor = x, Color.White);
                editorQuery.MapCustom(this, "attenuation", "attenuation", x => standardPointLightComponent.LightIntensity = x.Length(), Vector3.UnitY);
                editorQuery.MapCustom(this, "range", "range", x => standardPointLightComponent.LightRadius = x, 1f);
            }
        }
    }
}