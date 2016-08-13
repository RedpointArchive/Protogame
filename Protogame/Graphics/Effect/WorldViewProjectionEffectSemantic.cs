using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class WorldViewProjectionEffectSemantic : IWorldViewProjectionEffectSemantic
    {
        private IEffectParameterSet _parameterSet;

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

        public bool ShouldAttachToParameterSet(IEffectParameterSet parameterSet)
        {
            return parameterSet["WorldViewProj"] != null || (
                parameterSet["World"] != null &&
                parameterSet["View"] != null &&
                parameterSet["Projection"] != null);
        }

        public void AttachToParameterSet(IEffectParameterSet parameterSet)
        {
            if (_parameterSet != null)
            {
                throw new InvalidOperationException("This semantic is already attached.");
            }

            _parameterSet = parameterSet;
            CacheParameters();
        }

        public IEffectSemantic Clone(IEffectParameterSet parameterSet)
        {
            var clone = new WorldViewProjectionEffectSemantic();
            clone.AttachToParameterSet(parameterSet);
            if (_parameterSet != null)
            {
                clone.World = World;
                clone.View = View;
                clone.Projection = Projection;
            }
            return clone;
        }

        public void CacheParameters()
        {
            if (_worldParam == null)
            {
                if (_parameterSet["WorldViewProj"] != null)
                {
                    _separatedMatrices = false;
                    _worldViewProjParam = _parameterSet["WorldViewProj"].NativeParameter;
                }
                else
                {
                    _separatedMatrices = true;
                    _worldParam = _parameterSet["World"].NativeParameter;
                    _viewParam = _parameterSet["View"].NativeParameter;
                    _projectionParam = _parameterSet["Projection"].NativeParameter;
                }
            }
        }

        public void OnApply(IRenderContext renderContext)
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
