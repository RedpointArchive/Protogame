using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;

namespace Protogame.ATFLevelEditor
{
    public class BakingEditorQuery
    {
        public Dictionary<string, PropertyEntry> Properties { get; set; }

        public DeclaredAs DeclaredAs { get; set; }

        public RenderMode RenderMode { get; set; }

        public bool CanContainComponents { get; set; }

        public bool CanContainEntities { get; set; }

        public bool HasMatrix { get; set; }

        public EditorPrimitiveShape PrimitiveShape { get; set; }
    }

    public enum DeclaredAs
    {
        Unknown,
        Component,
        Entity,
    }

    public enum RenderMode
    {
        None,
        PrimitiveShape,
        Model
    }

    public class PropertyEntry
    {
        public string ID { get; set; }

        public string LowerCamelCaseName { get; set; }

        public string CamelCaseName { get; set; }

        public string DisplayName { get; set; }

        public Type Type { get; set; }

        public string NativeType { get; set; }

        public string NativeAccess { get; set; }

        public string EditorCategory { get; set; }

        public string EditorDescription { get; set; }

        public string EditorType { get; set; }

        public string EditorConverterType { get; set; }

        public string XSDType { get; set; }

        public string XSDDefaultValue { get; set; }
    }

    public class BakingEditorQuery<T> : BakingEditorQuery, IEditorQuery<T> where T : IEntity
    {
        public EditorQueryMode Mode => EditorQueryMode.BakingSchema;

        public void MapMatrix<TTarget>(TTarget @object, Expression<Func<T, Matrix>> matrixProperty) where TTarget : T, IHasMatrix
        {
        }

        public void MapCustom<TTarget, T2>(TTarget @object, string id, string name, Expression<Func<T, T2>> property, T2 @default) where TTarget : T
        {
            Properties[id] = CreatePropertyFromDeclaration(
                id,
                (id.Substring(0, 1).ToLowerInvariant() + name.Substring(1)).Replace(" ", ""),
                (id.Substring(0, 1).ToUpperInvariant() + name.Substring(1)).Replace(" ", ""),
                name, 
                typeof(T2),
                @default);
        }

        private PropertyEntry CreatePropertyFromDeclaration(string id, string lowerCamelCaseName, string camelCaseName, string displayName, Type type, object @default)
        {
            return new PropertyEntry
            {
                ID = id,
                LowerCamelCaseName = lowerCamelCaseName,
                CamelCaseName = camelCaseName,
                DisplayName = displayName,
                NativeAccess = "set",
                NativeType = ConvertToNativeType(type),
                Type = type,
                EditorCategory = "Custom",
                EditorDescription = "Custom property",
                EditorType = GetEditorForType(type),
                EditorConverterType = GetConverterForType(type),
                XSDType = ConvertToXSDType(type),
                XSDDefaultValue = GetXSDDefaultValueForTypeAndObject(type, @default),
            };
        }

        private T? ConvertObjectToNumericType<T>(object o) where T : struct 
        {
            if (o == null)
            {
                return null;
            }

            if (o is int)
            {
                if (typeof (T) == typeof (int))
                {
                    return new T?((T)(object)(int)o);
                }
                if (typeof (T) == typeof (float))
                {
                    return new T?((T)(object)(float)(int)o);
                }

                throw new NotSupportedException("Can't cast int to " + typeof(T).FullName + "!");
            }

            if (o is uint)
            {
                if (typeof(T) == typeof(int))
                {
                    return new T?((T)(object)(int)(uint)o);
                }

                throw new NotSupportedException("Can't cast uint to " + typeof(T).FullName + "!");
            }

            if (o is float)
            {
                if (typeof(T) == typeof(float))
                {
                    return new T?((T)(object)(float)o);
                }
                if (typeof(T) == typeof(int))
                {
                    return new T?((T)(object)(int)(float)o);
                }

                throw new NotSupportedException("Can't cast floats to " + typeof(T).FullName + "!");
            }

            throw new NotSupportedException("Can't cast " + o.GetType().FullName + " to anything!");
        }

        private string GetXSDDefaultValueForTypeAndObject(Type type, object o)
        {
            if (type == typeof (Color))
            {
                if (o == null)
                {
                    return null;
                }
                else
                {
                    return ConvertObjectToNumericType<int>(((Color) o).PackedValue).ToString();
                }
            }
            if (type == typeof(uint))
            {
                if (o == null)
                {
                    return null;
                }
                else
                {
                    return ConvertObjectToNumericType<uint>(o).ToString();
                }
            }
            if (type == typeof (int))
            {
                if (o == null)
                {
                    return null;
                }
                else
                {
                    return ConvertObjectToNumericType<int>(o).ToString();
                }
            }
            if (type == typeof(float))
            {
                if (o == null)
                {
                    return null;
                }
                else
                {
                    return ConvertObjectToNumericType<float>(o).ToString();
                }
            }
            if (type == typeof(TextureAsset))
            {
                if (o == null)
                {
                    return null;
                }
                else
                {
                    return ((TextureAsset) o).Name;
                }
            }
            if (type == typeof(string))
            {
                return o as string;
            }
            if (type == typeof(Matrix))
            {
                // TODO
                return "1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 1";
            }
            throw new Exception("Unable to get XSD default value for type " + type + ".");
        }

        private string GetEditorForType(Type type)
        {
            if (type == typeof(Color))
            {
                return "Sce.Atf.Controls.PropertyEditing.ColorPickerEditor,Atf.Gui.WinForms:true";
            }

            return null;
        }

        private string GetConverterForType(Type type)
        {
            if (type == typeof(Color))
            {
                return "Sce.Atf.Controls.PropertyEditing.IntColorConverter";
            }

            return null;
        }

        private string ConvertToNativeType(Type type)
        {
            if (type.FullName == typeof (string).FullName)
            {
                return "wchar_t*";
            }
            if (type.FullName == typeof(int).FullName)
            {
                return "int";
            }
            if (type.FullName == typeof(float).FullName)
            {
                return "float";
            }
            if (type.FullName == typeof(Color).FullName)
            {
                return "int";
            }
            if (type.FullName == typeof(Matrix).FullName)
            {
                return "Matrix";
            }
            if (type.FullName == typeof(TextureAsset).FullName)
            {
                return "wchar_t*";
            }
            throw new Exception("Unable to convert type " + type + " to native C++ storage type.");
        }

        private string ConvertToXSDType(Type type)
        {
            if (type == typeof(int))
            {
                return "xs:int";
            }
            if (type == typeof(float))
            {
                return "xs:float";
            }
            if (type == typeof(string))
            {
                return "xs:string";
            }
            if (type == typeof(bool))
            {
                return "xs:boolean";
            }
            if (type == typeof(float[]))
            {
                return "floatListType";
            }
            if (type == typeof(Vector2))
            {
                return "vector2Type";
            }
            if (type == typeof(Vector3))
            {
                return "vector3Type";
            }
            if (type == typeof(Color))
            {
                return "xs:int";
            }
            if (type == typeof(Vector4))
            {
                return "vector4Type";
            }
            if (type == typeof(Matrix))
            {
                return "matrixType";
            }
            if (type == typeof(TextureAsset))
            {
                return "xs:string";
            }
            throw new Exception("Can't yet support type " + type);
        }

        public void DeclareAsComponent(T @object)
        {
            DeclaredAs = DeclaredAs.Component;

            HasMatrix = @object is IHasMatrix;
        }

        public void DeclareAsEntity<TTarget>(TTarget @object) where TTarget : T, IEntity
        {
            DeclaredAs = DeclaredAs.Entity;

            if (@object is ComponentizedEntity)
            {
                CanContainComponents = true;
            }
            
            if (@object is IContainsEntities)
            {
                CanContainEntities = true;
            }

            HasMatrix = true;
        }
        
        public void UsePrimitiveShapeForRendering(T @object, EditorPrimitiveShape shape)
        {
            RenderMode = RenderMode.PrimitiveShape;
            PrimitiveShape = shape;
        }

        public void MapStandardLightingModel(T @object, Expression<Func<T, Color>> colorProperty, Expression<Func<T, Color>> emissiveProperty,
            Expression<Func<T, Color>> specularProperty, Expression<Func<T, float>> specularPowerProperty, Expression<Func<T, string>> diffuseTextureNameProperty,
            Expression<Func<T, string>> normalTextureNameProperty, Expression<Func<T, Matrix>> textureTransformProperty)
        {
            Properties["color"] = CreatePropertyFromDeclaration("color", "color", "Color", "Diffuse Color", typeof (Color), Color.White);
            Properties["emissive"] = CreatePropertyFromDeclaration("emissive", "emissive", "Emissive", "Emissive Color", typeof(Color), Color.Black);
            Properties["specular"] = CreatePropertyFromDeclaration("specular", "specular", "Specular", "Specular Color", typeof(Color), Color.Black);
            Properties["specularPower"] = CreatePropertyFromDeclaration("specularPower", "specularPower", "SpecularPower", "Specular Power", typeof(float), 1);
            Properties["diffuse"] = CreatePropertyFromDeclaration("diffuse", "diffuse", "Diffuse", "Diffuse Texture", typeof(TextureAsset), null);
            Properties["normal"] = CreatePropertyFromDeclaration("normal", "normal", "Normal", "Normal Texture", typeof(TextureAsset), null);
            Properties["textureTransform"] = CreatePropertyFromDeclaration("textureTransform", "textureTransform", "TextureTransform", "Texture Transform", typeof(Matrix), Matrix.Identity);

            var ids = new[] {"color", "emissive", "specular", "specularPower", "diffuse", "normal", "textureTransform"};

            foreach (var id in ids)
            {
                Properties[id].EditorCategory = "StandardLightingModel";
            }
        }

        public BakingEditorQuery()
        {
            Properties = new Dictionary<string, PropertyEntry>();
            RenderMode = RenderMode.None;
            DeclaredAs = DeclaredAs.Unknown;
            HasMatrix = false;
        }
    }
}
