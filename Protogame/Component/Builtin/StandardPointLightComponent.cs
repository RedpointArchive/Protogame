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
            _standardPointLight = lightFactory.CreateStandardPointLight(
                Vector3.Zero,
                Color.White,
                1,
                1);
            _standardPointLightArray = new ILight[] {_standardPointLight};
        }
        
        public IEnumerable<ILight> GetLights() => _standardPointLightArray;
        
        public Color LightColor
        {
            get { return _standardPointLight.LightColor; }
            set { _standardPointLight.LightColor = value; }
        }

        public float LightRadius
        {
            get { return _standardPointLight.LightRadius; }
            set { _standardPointLight.LightRadius = value; }
        }

        public float LightIntensity
        {
            get { return _standardPointLight.LightIntensity; }
            set { _standardPointLight.LightIntensity = value; }
        }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            var matrix = Matrix.Identity;
            var matrixComponent = _node.Parent?.UntypedValue as IHasMatrix;
            if (matrixComponent != null)
            {
                matrix *= matrixComponent.GetFinalMatrix();
            }

            _standardPointLight.LightPosition = Vector3.Transform(Vector3.Zero, matrix);
        }
    }
}
