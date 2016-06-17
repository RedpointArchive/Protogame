using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class StandardPointLightComponent : ILightableComponent, IUpdatableComponent
    {
        private readonly INode _node;
        private readonly IStandardPointLight _standardPointLight;
        private readonly ILight[] _standardPointLightArray;

        public StandardPointLightComponent(
            INode node,
            ILightFactory lightFactory)
        {
            _node = node;

            try
            {
                _standardPointLight = lightFactory.CreateStandardPointLight(
                    Vector3.Zero,
                    Color.White,
                    1,
                    1);
                _standardPointLightArray = new ILight[] {_standardPointLight};
            }
            catch (NotSupportedException)
            {
                _standardPointLight = null;
                _standardPointLightArray = new ILight[0];
            }
        }
        
        public IEnumerable<ILight> GetLights() => _standardPointLightArray;
        
        public Color LightColor
        {
            get
            {
                if (_standardPointLight == null)
                {
                    return Color.Black;
                }

                return _standardPointLight.LightColor;
            }
            set
            {
                if (_standardPointLight == null)
                {
                    return;
                }

                _standardPointLight.LightColor = value;
            }
        }

        public float LightRadius
        {
            get
            {
                if (_standardPointLight == null)
                {
                    return 0f;
                }

                return _standardPointLight.LightRadius;
            }
            set
            {
                if (_standardPointLight == null)
                {
                    return;
                }

                _standardPointLight.LightRadius = value;
            }
        }

        public float LightIntensity
        {
            get
            {
                if (_standardPointLight == null)
                {
                    return 0f;
                }

                return _standardPointLight.LightIntensity;
            }
            set
            {
                if (_standardPointLight == null)
                {
                    return;
                }

                _standardPointLight.LightIntensity = value;
            }
        }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            if (_standardPointLight == null)
            {
                throw new InvalidOperationException(
                    "The light was null (presumably because this code is executing on a server), " +
                    "but the Update method that accepts a game context was called.");
            }

            var matrix = Matrix.Identity;
            var matrixComponent = _node.Parent?.UntypedValue as IHasTransform;
            if (matrixComponent != null)
            {
                matrix *= matrixComponent.FinalTransform.AbsoluteMatrix;
            }

            _standardPointLight.LightPosition = Vector3.Transform(Vector3.Zero, matrix);
        }
    }
}
