using System;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;

namespace Protogame.ATFLevelEditor
{
    /// <summary>
    /// This interface is used by the ATF Level Editor API to query information
    /// about editable entities.  When an entity has the [EditableEntity] attribute
    /// on it, it must accept this interface in it's public constructor.  The
    /// post-build step that prepares the schema file for the editor will
    /// set the <see cref="Mode"/> property to <c>BakingSchema</c>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity; it must match the type that this interface
    /// is being injected into.
    /// </typeparam>
    /// <module>Editor</module>
    public interface IEditorQuery<T> where T : IEntity
    {
        EditorQueryMode Mode { get; }

        /// <summary>
        /// Maps the specified property as the matrix for the entity in the hierarchy.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="matrixProperty"></param>
        void MapMatrix(T entity, Expression<Func<T, Matrix>> matrixProperty);

        /// <summary>
        /// Maps the specified property as a custom property on the entity.
        /// </summary>
        /// <typeparam name="T2">The type of the property.</typeparam>
        /// <param name="entity">The entity that's being mapped.</param>
        /// <param name="id">The ID of the property to use in level data.  Changing this will result in the data mapped against this ID in levels to no longer be loaded.</param>
        /// <param name="name">The name of the property to display in the editor.</param>
        /// <param name="property">The property to map onto.</param>
        void MapCustom<T2>(T entity, string id, string name, Expression<Func<T, T2>> property);
    }
}
