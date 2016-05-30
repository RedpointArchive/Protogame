using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class WorldViewProjectionEffectSemantic : IWorldViewProjectionEffectSemantic
    {
        private EffectWithSemantics _effectWithSemantics;

        private EffectParameter _worldParam;

        private EffectParameter _viewParam;

        private EffectParameter _projectionParam;

        private EffectParameter _worldViewProjParam;

        private Matrix _world = Matrix.Identity;

        private Matrix _view = Matrix.Identity;

        private Matrix _projection = Matrix.Identity;

        private bool _separatedMatrices;

        private bool _worldViewProjParamDirty = true;

        public Matrix World
        {
            get
            {
                return _world;
            }
            set
            {
                _world = value;
                _worldViewProjParamDirty = true;
            }
        }

        public Matrix View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
                _worldViewProjParamDirty = true;
            }
        }

        public Matrix Projection
        {
            get
            {
                return _projection;
            }
            set
            {
                _projection = value;
                _worldViewProjParamDirty = true;
            }
        }

        public bool ShouldAttachToEffect(EffectWithSemantics effectWithSemantics)
        {
            return effectWithSemantics.Parameters["WorldViewProj"] != null || (
                effectWithSemantics.Parameters["World"] != null &&
                effectWithSemantics.Parameters["View"] != null &&
                effectWithSemantics.Parameters["Projection"] != null);
        }

        public void AttachToEffect(EffectWithSemantics effectWithSemantics)
        {
            if (_effectWithSemantics != null)
            {
                throw new InvalidOperationException("This semantic is already attached.");
            }

            _effectWithSemantics = effectWithSemantics;
            CacheEffectParameters();
        }

        public IEffectSemantic Clone(EffectWithSemantics effectWithSemantics)
        {
            var clone = new WorldViewProjectionEffectSemantic();
            clone.AttachToEffect(effectWithSemantics);
            clone.World = World;
            clone.View = View;
            clone.Projection = Projection;
            return clone;
        }

        public void CacheEffectParameters()
        {
            if (_worldParam == null)
            {
                if (_effectWithSemantics.Parameters["WorldViewProj"] != null)
                {
                    _separatedMatrices = false;
                    _worldViewProjParam = _effectWithSemantics.Parameters["WorldViewProj"];
                }
                else
                {
                    _separatedMatrices = true;
                    _worldParam = _effectWithSemantics.Parameters["World"];
                    _viewParam = _effectWithSemantics.Parameters["View"];
                    _projectionParam = _effectWithSemantics.Parameters["Projection"];
                }
            }
        }

        public void OnApply()
        {
            if (_worldViewProjParamDirty)
            {
                if (_separatedMatrices)
                {
                    _worldParam.SetValue(_world);
                    _viewParam.SetValue(_view);
                    _projectionParam.SetValue(_projection);
                }
                else
                {
                    Matrix worldViewProj;
                    Matrix worldView;

                    Matrix.Multiply(ref _world, ref _view, out worldView);
                    Matrix.Multiply(ref worldView, ref _projection, out worldViewProj);

                    _worldViewProjParam.SetValue(worldViewProj);
                }

                _worldViewProjParamDirty = false;
            }
        }
    }
}
