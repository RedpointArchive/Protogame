using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class StandardDirectionalLightComponent : ILightableComponent
    {
        private readonly IStandardDirectionalLight _standardDirectionalLight;
        private readonly ILight[] _standardDirectionalLightArray;

        public StandardDirectionalLightComponent(
            ILightFactory lightFactory)
        {
            try
            {
                _standardDirectionalLight = lightFactory.CreateStandardDirectionalLight(
                    Vector3.One,
                    Color.White);
                _standardDirectionalLightArray = new ILight[] {_standardDirectionalLight};
            }
            catch (NotSupportedException)
            {
                _standardDirectionalLight = null;
                _standardDirectionalLightArray = new ILight[0];
            }
        }
        
        public IEnumerable<ILight> GetLights() => _standardDirectionalLightArray;

        public Vector3 LightDirection
        {
            get
            {
                if (_standardDirectionalLight == null)
                {
                    return Vector3.Zero;
                }

                return _standardDirectionalLight.LightDirection;
            }
            set
            {
                if (_standardDirectionalLight == null)
                {
                    return;
                }

                _standardDirectionalLight.LightDirection = value;
            }
        }

        public Color LightColor
        {
            get
            {
                if (_standardDirectionalLight == null)
                {
                    return Color.Black;
                }

                return _standardDirectionalLight.LightColor;
            }
            set
            {
                if (_standardDirectionalLight == null)
                {
                    return;
                }

                _standardDirectionalLight.LightColor = value;
            }
        }
    }
}
