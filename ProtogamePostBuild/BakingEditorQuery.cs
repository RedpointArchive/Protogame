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

        public DeclaredAs DeclaredAs { get; set; }

        public RenderMode RenderMode { get; set; }

        public List<Type> AcceptsComponentsOfTypeList { get; set; }
    }

    public enum DeclaredAs
    {
        Unknown,
        Component,
        Entity,
        ComponentizedEntity
    }

    public enum RenderMode
    {
        None,
        PrimitiveShape,
        Model
    }

    public class BakingEditorQuery<T> : BakingEditorQuery, IEditorQuery<T> where T : IEntity
    {
        public EditorQueryMode Mode => EditorQueryMode.BakingSchema;

        public void MapMatrix<TTarget>(TTarget @object, Expression<Func<T, Matrix>> matrixProperty) where TTarget : T, IHasMatrix
        {
        }

        public void MapCustom<TTarget, T2>(TTarget @object, string id, string name, Expression<Func<T, T2>> property) where TTarget : T
        {
            IDToName[id] = name;
            IDToType[id] = typeof(T2);
        }

        public void DeclareAsComponent(T @object)
        {
            DeclaredAs = DeclaredAs.Component;
        }

        public void DeclareAsEntity<TTarget>(TTarget @object) where TTarget : T, IEntity
        {
            DeclaredAs = DeclaredAs.Entity;
        }

        public void DeclareAsComponentizedEntity<TTarget>(TTarget @object) where TTarget : ComponentizedEntity, T
        {
            DeclaredAs = DeclaredAs.ComponentizedEntity;
        }

        public void AcceptsComponentsOfType<TComponent>(T @object)
        {
            AcceptsComponentsOfTypeList.Add(typeof(TComponent));
        }

        public void UsePrimitiveShapeForRendering(T @object, EditorPrimitiveShape shape)
        {
            RenderMode = RenderMode.PrimitiveShape;
        }

        public void MapStandardLightingModel(T @object, Expression<Func<T, Color>> colorProperty, Expression<Func<T, Color>> emissiveProperty,
            Expression<Func<T, Color>> specularProperty, Expression<Func<T, float>> specularPowerProperty, Expression<Func<T, string>> diffuseTextureNameProperty,
            Expression<Func<T, string>> normalTextureNameProperty, Expression<Func<T, Matrix>> textureTransformProperty)
        {
            IDToName["color"] = "Diffuse Color"; IDToType["color"] = typeof(Color);
            IDToName["emissive"] = "Emissive Color"; IDToType["emissive"] = typeof(Color);
            IDToName["specular"] = "Specular Color"; IDToType["specular"] = typeof(Color);
            IDToName["specularPower"] = "Specular Power"; IDToType["specularPower"] = typeof(float);
            IDToName["diffuse"] = "Diffuse Texture"; IDToType["color"] = typeof(string);
            IDToName["normal"] = "Normal Texture"; IDToType["color"] = typeof(string);
            IDToName["textureTransform"] = "Texture Transform"; IDToType["color"] = typeof(Matrix);
        }

        public BakingEditorQuery()
        {
            IDToName = new Dictionary<string, string>();
            IDToType = new Dictionary<string, Type>();
            AcceptsComponentsOfTypeList = new List<Type>();
            RenderMode = RenderMode.None;
            DeclaredAs = DeclaredAs.Unknown;
        }
    }
}
