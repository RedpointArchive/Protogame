using System;
using System.Collections.Generic;
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
        /// Maps the specified property as the transform for the object in the hierarchy.
        /// </summary>
        /// <param name="object"></param>
        /// <param name="matrixProperty"></param>
        void MapTransform<TTarget>(TTarget @object, Action<ITransform> setTransform) where TTarget : T, IHasTransform;

        /// <summary>
        /// Maps the specified property as a custom property on the object.
        /// </summary>
        /// <typeparam name="T2">The type of the property.</typeparam>
        /// <typeparam name="TTarget">The same object type as the one on the interface.</typeparam>
        /// <param name="object">The object that's being mapped.</param>
        /// <param name="id">The ID of the property to use in level data.  Changing this will result in the data mapped against this ID in levels to no longer be loaded.</param>
        /// <param name="name">The name of the property to display in the editor.</param>
        /// <param name="setProperty"></param>
        /// <param name="default"></param>
        void MapCustom<TTarget, T2>(TTarget @object, string id, string name, Action<T2> setProperty, T2 @default) where TTarget : T;

        /// <summary>
        /// Declares that this object is a component which can be added to
        /// entities in the level.
        /// </summary>
        /// <param name="object">The object, which should be a component.</param>
        void DeclareAsComponent(T @object);

        /// <summary>
        /// Declares that this object is an entity.
        /// </summary>
        void DeclareAsEntity<TTarget>(TTarget @object) where TTarget : T, IEntity;

        /// <summary>
        /// Specifies that this object should use a primitive shape when rendered in the level editor.
        /// </summary>
        /// <param name="shape">The type of shape to render as.</param>
        void UsePrimitiveShapeForRendering(T @object, EditorPrimitiveShape shape);

        /// <summary>
        /// Specifies that this object should use a 2D icon when rendering in the level editor.
        /// </summary>
        /// <param name="pngFilePathFromProjectRoot">The path to the PNG file, relative to the project root.</param>
        void UseIconForRendering(T @object, string pngFilePathFromProjectRoot);

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
            Action<Color> colorProperty,
            Action<Color> emissiveProperty,
            Action<Color> specularProperty,
            Action<float> specularPowerProperty,
            Action<string> diffuseTextureNameProperty,
            Action<string> normalTextureNameProperty,
            Action<Matrix> textureTransformProperty);

        /// <summary>
        /// Returns a list of raw resource URIs that are attached to this object.
        /// </summary>
        /// <returns>A list of raw resource URIs.</returns>
        IEnumerable<string> GetRawResourceUris();
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
