using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Protogame.ATFLevelEditor
{
    public class LoadingEditorQuery<T> : IEditorQuery<T> where T : IEntity
    {
        private readonly ITransformUtilities _transformUtilities;
        private readonly XmlElement _element;

        public EditorQueryMode Mode => EditorQueryMode.LoadingConfiguration;

        public LoadingEditorQuery(ITransformUtilities transformUtilities, XmlElement element)
        {
            _transformUtilities = transformUtilities;
            _element = element;
        }

        public void MapTransform<TTarget>(TTarget @object, Action<ITransform> setTransform) where TTarget : T, IHasTransform
        {
            var scaleRawValue =
                _element.GetAttribute("scale").Split(' ')
                    .Select(x => float.Parse(x, CultureInfo.InvariantCulture))
                    .ToArray();
            var rotateRawValue =
                _element.GetAttribute("rotate").Split(' ')
                    .Select(x => float.Parse(x, CultureInfo.InvariantCulture))
                    .ToArray();
            var translateRawValue =
                _element.GetAttribute("translate").Split(' ')
                    .Select(x => float.Parse(x, CultureInfo.InvariantCulture))
                    .ToArray();
            if (scaleRawValue.Length == 3 && rotateRawValue.Length == 3 && translateRawValue.Length == 3)
            {
                var rotateMatrix = 
                    Matrix.CreateRotationX(rotateRawValue[0]) *
                    Matrix.CreateRotationY(rotateRawValue[1]) *
                    Matrix.CreateRotationZ(rotateRawValue[2]);
                var rotateValue = Quaternion.Identity;
                Vector3 scale, translation;
                Quaternion rotation;
                if (rotateMatrix.Decompose(out scale, out rotation, out translation))
                {
                    rotateValue = rotation;
                }
                
                var scaleValue = new Vector3(
                    scaleRawValue[0],
                    scaleRawValue[1],
                    scaleRawValue[2]);
                var translateValue = new Vector3(
                    translateRawValue[0],
                    translateRawValue[1],
                    translateRawValue[2]);
                var transform = _transformUtilities.CreateFromSRTMatrix(scaleValue, rotateValue, translateValue);
                setTransform(transform);

                return;
            }

            var transformRawValue =
                _element.GetAttribute("transform").Split(' ')
                 .Select(x => float.Parse(x, CultureInfo.InvariantCulture))
                    .ToArray();
            if (transformRawValue.Length == 16)
            {
                var matrixValue = new Matrix();
                for (var i = 0; i < 16; i++)
                {
                    matrixValue[i] = transformRawValue[i];
                }

                var transform = _transformUtilities.CreateFromCustomMatrix(matrixValue);
                setTransform(transform);
            }
        }

        public void MapVisibility(T @object, Action<bool> setVisibility)
        {
            var visibleRawValue =
                _element.GetAttribute("visible");
            if (visibleRawValue == "true" || visibleRawValue == "false")
            {
                setVisibility(visibleRawValue == "true");
            }
        }

        public void MapCustom<T2>(T @object, string id, string name, Action<T2> setProperty, T2 @default)
        {
            var propertyRawValue = _element.GetAttribute(id);
            object propertyValue = null;
            if (typeof(T2) == typeof(int))
            {
                int v = (int)(object)@default;
                int.TryParse(propertyRawValue, out v);
                propertyValue = v;
            }
            else if (typeof(T2) == typeof(uint))
            {
                uint v = (uint)(object)@default;
                uint.TryParse(propertyRawValue, out v);
                propertyValue = v;
            }
            else if (typeof(T2) == typeof(float))
            {
                float v = (float)(object)@default;
                float.TryParse(propertyRawValue, out v);
                propertyValue = v;
            }
            else if (typeof(T2) == typeof(double))
            {
                double v = (double)(object)@default;
                double.TryParse(propertyRawValue, out v);
                propertyValue = v;
            }
            else if (typeof(T2) == typeof(string))
            {
                propertyValue = propertyRawValue;
            }
            else if (typeof(T2) == typeof(bool))
            {
                propertyValue = propertyRawValue == "true";
            }
            else if (typeof(T2) == typeof(Color))
            {
                var c = int.Parse(propertyRawValue);

                var r = c & 0xFF;
                var g = (c >> 8) & 0xFF;
                var b = (c >> 16) & 0xFF;
                var a = (c >> 24) & 0xFF;

                propertyValue = new Color(r, g, b, a);
            }
            else if (typeof(T2) == typeof(Vector3))
            {
                var c = propertyRawValue.Split(' ')
                    .Select(x => float.Parse(x, CultureInfo.InvariantCulture))
                    .ToArray();

                propertyValue = new Vector3(
                    c[0],
                    c[1],
                    c[2]);
            }
            else if (typeof(T2) == typeof(Matrix))
            {
                var c = propertyRawValue.Split(' ')
                    .Select(x => float.Parse(x, CultureInfo.InvariantCulture))
                    .ToArray();

                var matrixValue = new Matrix();
                for (var i = 0; i < 16; i++)
                {
                    matrixValue[i] = c[i];
                }

                propertyValue = matrixValue;
            }
            else
            {
                throw new NotSupportedException("Unable to load type " + typeof(T2).FullName + " from level data.");
            }

            setProperty((T2)propertyValue);
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

        public void UseModelForRendering(T @object, string modelPathFromResourceRoot)
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
            return _element.ChildNodes.OfType<XmlElement>()
                .Where(x => x.LocalName == "resource")
                .Select(x => x.GetAttribute("uri"));
        }
    }
}
