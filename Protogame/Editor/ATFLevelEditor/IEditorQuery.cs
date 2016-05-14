using System;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;

namespace Protogame.ATFLevelEditor
{
    /// <summary>
    /// This interface is used by the ATF Level Editor API to query information
    /// about editable objects.  When an object injects this interface in it's
    /// public constructor, it can be spawned in the ATF Level Editor.  The
    /// post-build step that prepares the schema file for the editor will
    /// set the <see cref="Mode"/> property to <c>BakingSchema</c>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity; it must match the type that this interface
    /// is being injected into.
    /// </typeparam>
    /// <module>Editor</module>
    public interface IEditorQuery<T>
    {
        EditorQueryMode Mode { get; }

        /// <summary>
        /// Maps the specified property as the matrix for the object in the hierarchy.
        /// </summary>
        /// <param name="object"></param>
        /// <param name="matrixProperty"></param>
        void MapMatrix<TTarget>(TTarget @object, Expression<Func<T, Matrix>> matrixProperty) where TTarget : T, IHasMatrix;

        /// <summary>
        /// Maps the specified property as a custom property on the object.
        /// </summary>
        /// <typeparam name="T2">The type of the property.</typeparam>
        /// <typeparam name="TTarget">The same object type as the one on the interface.</typeparam>
        /// <param name="object">The object that's being mapped.</param>
        /// <param name="id">The ID of the property to use in level data.  Changing this will result in the data mapped against this ID in levels to no longer be loaded.</param>
        /// <param name="name">The name of the property to display in the editor.</param>
        /// <param name="property">The property to map onto.</param>
        void MapCustom<TTarget, T2>(TTarget @object, string id, string name, Expression<Func<T, T2>> property) where TTarget : T;

        /// <summary>
        /// Declares that this object is a component which can be added to
        /// entities in the level.
        /// </summary>
        /// <param name="object">The object, which should be a component.</param>
        void DeclareAsComponent(T @object);

        /// <summary>
        /// Declares that this object is an entity which DOES NOT accept components
        /// in it's hierarchy.  Use <see cref="DeclareAsComponentizedEntity{TTarget}"/>
        /// for that.
        /// </summary>
        /// <param name="object"></param>
        void DeclareAsEntity<TTarget>(TTarget @object) where TTarget : T, IEntity;

        /// <summary>
        /// Declares that this object is an entity which accepts components in it's
        /// hierarchy.
        /// </summary>
        /// <param name="object"></param>
        void DeclareAsComponentizedEntity<TTarget>(TTarget @object) where TTarget : ComponentizedEntity, T;

        /// <summary>
        /// Declares that this object can accept components of a given type.
        /// </summary>
        /// <typeparam name="TComponent">The type of components that this object can accept.</typeparam>
        void AcceptsComponentsOfType<TComponent>(T @object);

        /// <summary>
        /// Specifies that this object should use a primitive shape when rendered in the level editor.
        /// </summary>
        /// <param name="shape">The type of shape to render as.</param>
        void UsePrimitiveShapeForRendering(T @object, EditorPrimitiveShape shape);

        /// <summary>
        /// Maps the standard lighting model properties onto this object.  The level editor uses a
        /// standard lighting model to render the scene.  In future we plan to support alternate
        /// shaders in the level editor.
        /// </summary>
        /// <param name="object"></param>
        /// <param name="colorProperty"></param>
        /// <param name="emissiveProperty"></param>
        /// <param name="specularProperty"></param>
        /// <param name="specularPowerProperty"></param>
        /// <param name="diffuseTextureNameProperty"></param>
        /// <param name="normalTextureNameProperty"></param>
        /// <param name="textureTransformProperty"></param>
        void MapStandardLightingModel(
            T @object,
            Expression<Func<T, Color>> colorProperty,
            Expression<Func<T, Color>> emissiveProperty,
            Expression<Func<T, Color>> specularProperty,
            Expression<Func<T, float>> specularPowerProperty,
            Expression<Func<T, string>> diffuseTextureNameProperty,
            Expression<Func<T, string>> normalTextureNameProperty,
            Expression<Func<T, Matrix>> textureTransformProperty);
    }

    public enum EditorPrimitiveShape
    {
        QuadLineStrip,
        Quad,
        AsteriskQuads,
        Cube,
        Sphere,
        Cylinder,
        Torus,
        Cone,
    }
}
