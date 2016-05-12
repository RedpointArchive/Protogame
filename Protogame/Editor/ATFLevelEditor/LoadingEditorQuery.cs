using System;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;

namespace Protogame.ATFLevelEditor
{
    public class LoadingEditorQuery<T> : IEditorQuery<T> where T : IEntity
    {
        public EditorQueryMode Mode => EditorQueryMode.LoadingConfiguration;

        public void MapMatrix(T entity, Expression<Func<T, Matrix>> matrixProperty)
        {
        }

        public void MapCustom<T2>(T entity, string id, string name, Expression<Func<T, T2>> property)
        {
        }
    }
}
