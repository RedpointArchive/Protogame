using System;
using System.Collections.Generic;
using System.Linq;
using Protogame.ATFLevelEditor;

namespace Protogame
{
    public class LocatorEntity : ComponentizedEntity
    {
        private readonly Render3DModelComponent _modelComponent;

        public LocatorEntity(
            IEditorQuery<LocatorEntity> editorQuery,
            IAssetManager assetManager,
            Render3DModelComponent modelComponent)
        {
            if (editorQuery.Mode != EditorQueryMode.BakingSchema)
            {
                _modelComponent = modelComponent;
                RegisterComponent(_modelComponent);
                
                editorQuery.MapTransform(this, Transform.Assign);

                var modelUri = editorQuery.GetRawResourceUris().FirstOrDefault();
                if (modelUri != null)
                {
                    var extIndex = modelUri.LastIndexOf(".", StringComparison.InvariantCulture);
                    if (extIndex != -1)
                    {
                        modelUri = modelUri.Substring(0, extIndex);
                    }
                    var pathComponents = modelUri.Split('/').ToList();
                    while (pathComponents[0] == "..")
                    {
                        pathComponents.RemoveAt(0);
                    }

                    var paths = new List<string>();
                    while (pathComponents.Count > 0)
                    {
                        var attemptAsset = string.Join(".", pathComponents);
                        paths.Add(attemptAsset);
                        pathComponents.RemoveAt(0);
                    }

                    _modelComponent.Model = assetManager.GetPreferred<ModelAsset>(paths.ToArray());
                }
            }
        }
    }
}
