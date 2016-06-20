using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Protogame.ATFLevelEditor
{
    public class SpawningEditorQuery<T> : IEditorQuery<T> where T : IEntity
    {
        public EditorQueryMode Mode => EditorQueryMode.ManuallySpawned;

        public void MapTransform<TTarget>(TTarget @object, Action<ITransform> setTransform) where TTarget : T, IHasTransform
        {
        }

        public void MapCustom<TTarget, T2>(TTarget @object, string id, string name, Action<T2> setProperty, T2 @default) where TTarget : T
        {
        }

        public void DeclareAsComponent(T @object)
        {
        }

        public void DeclareAsEntity<TTarget>(TTarget @object) where TTarget : T, IEntity
        {
        }

        public void DeclareAsComponentizedEntity<TTarget>(TTarget @object) where TTarget : ComponentizedEntity, T
        {
        }

        public void AcceptsComponentsOfType<TComponent>(T @object)
        {
        }

        public void UsePrimitiveShapeForRendering(T @object, EditorPrimitiveShape shape)
        {
        }

        public void UseIconForRendering(T @object, string pngFilePathFromProjectRoot)
        {
        }

        public void MapStandardLightingModel(T @object, Action<Color> colorProperty, Action<Color> emissiveProperty, Action<Color> specularProperty, Action<float> specularPowerProperty, Action<string> diffuseTextureNameProperty, Action<string> normalTextureNameProperty, Action<Matrix> textureTransformProperty)
        {
        }

        public IEnumerable<string> GetRawResourceUris()
        {
            return new string[0];
        }
    }
}
