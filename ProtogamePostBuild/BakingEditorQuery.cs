using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;

namespace Protogame.ATFLevelEditor
{
    public class BakingEditorQuery
    {
        public Dictionary<string, string> IDToName { get; set; }

        public Dictionary<string, Type> IDToType { get; set; }
    }

    public class BakingEditorQuery<T> : BakingEditorQuery, IEditorQuery<T> where T : IEntity
    {
        public EditorQueryMode Mode => EditorQueryMode.BakingSchema;

        public BakingEditorQuery()
        {
            IDToName = new Dictionary<string, string>();
            IDToType = new Dictionary<string, Type>();
        } 

        public void MapMatrix(T entity, Expression<Func<T, Matrix>> matrixProperty)
        {
        }

        public void MapCustom<T2>(T entity, string id, string name, Expression<Func<T, T2>> property)
        {
            IDToName[id] = name;
            IDToType[id] = typeof (T2);
        }
    }
}
