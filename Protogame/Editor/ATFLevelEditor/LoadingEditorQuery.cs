using System;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;

namespace Protogame.ATFLevelEditor
{
    public class LoadingEditorQuery<T> : IEditorQuery<T> where T : IEntity
    {
        public EditorQueryMode Mode => EditorQueryMode.LoadingConfiguration;

        public void MapMatrix<TTarget>(TTarget @object, Expression<Func<T, Matrix>> matrixProperty) where TTarget : T, IHasMatrix
        {
        }

        public void MapCustom<TTarget, T2>(TTarget @object, string id, string name, Expression<Func<T, T2>> property, T2 @default) where TTarget : T
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

        public void MapStandardLightingModel(T @object, Expression<Func<T, Color>> colorProperty, Expression<Func<T, Color>> emissiveProperty,
            Expression<Func<T, Color>> specularProperty, Expression<Func<T, float>> specularPowerProperty, Expression<Func<T, string>> diffuseTextureNameProperty,
            Expression<Func<T, string>> normalTextureNameProperty, Expression<Func<T, Matrix>> textureTransformProperty)
        {
        }
    }
}
