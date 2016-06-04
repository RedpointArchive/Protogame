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
            _standardDirectionalLight = lightFactory.CreateStandardDirectionalLight(
                Vector3.One,
                Color.White);
            _standardDirectionalLightArray = new ILight[] { _standardDirectionalLight };
        }
        
        public IEnumerable<ILight> GetLights() => _standardDirectionalLightArray;

        public Vector3 LightDirection
        {
            get { return _standardDirectionalLight.LightDirection; }
            set { _standardDirectionalLight.LightDirection = value; }
        }

        public Color LightColor
        {
            get { return _standardDirectionalLight.LightColor; }
            set { _standardDirectionalLight.LightColor = value; }
        }
    }
}
