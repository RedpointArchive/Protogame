using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class ProtogameEffect : IEffect
    {
        private readonly IEffectSemantic[] _availableSemantics;

        private readonly Effect _targetEffect;
        
        public ProtogameEffect(GraphicsDevice graphicsDevice, byte[] data, string name, IEffectSemantic[] availableSemantics)
        {
            _availableSemantics = availableSemantics;
            _targetEffect = new Effect(graphicsDevice, data);
            _targetEffect.Name = name;
            Name = name;
            Parameters = new ProtogameEffectParameterCollection(_targetEffect);
        }

        public ProtogameEffect(Effect targetEffect, IEffectSemantic[] availableSemantics)
        {
            _targetEffect = targetEffect;
            _availableSemantics = availableSemantics;
            Name = "DynamicEffect";
            Parameters = new ProtogameEffectParameterCollection(_targetEffect);
        }

        public ProtogameEffect(GraphicsDevice graphicsDevice, IEffectReader reader, string name, IEffectSemantic[] availableSemantics)
        {
            _availableSemantics = availableSemantics;
            _targetEffect = new Effect(graphicsDevice, reader);
            _targetEffect.Name = name;
            Name = name;
            Parameters = new ProtogameEffectParameterCollection(_targetEffect);
        }

        public string Name { get; }
        
        public Effect NativeEffect => _targetEffect;

        public IEffectParameterCollection Parameters { get; }

        public IEffectTechniqueCollection Techniques { get; }

        public IEffectTechnique CurrentTechnique { get; set; }

        public IEffectParameterSet CreateParameterSet()
        {
            return new ProtogameParameterSet(this);
        }

        public void LoadParameterSet(IRenderContext renderContext, IEffectParameterSet effectParameters, bool skipMatricSync)
        {
            if (!skipMatricSync)
            {
                if (effectParameters.HasSemantic<IWorldViewProjectionEffectSemantic>())
                {
                    var semantic = effectParameters.GetSemantic<IWorldViewProjectionEffectSemantic>();
                    semantic.World = renderContext.World;
                    semantic.Projection = renderContext.Projection;
                    semantic.View = renderContext.View;
                }
            }

            ((ProtogameParameterSet)effectParameters).Load(renderContext, _targetEffect);
        }
        
        private class ProtogameParameterSet : IEffectParameterSet
        {
            private OrderedDictionary _writableParameters;

            public bool _locked;

            private List<IEffectSemantic> _semantics;

            private Dictionary<Type, IEffectSemantic> _semanticsByType;

            public ProtogameParameterSet(ProtogameEffect protogameEffect)
            {
                _writableParameters = new OrderedDictionary();
                _semantics = new List<IEffectSemantic>();

#if DEBUG
                GraphicsMetricsProfilerVisualiser.ParameterSetsCreated++;
#endif

                for (var i = 0; i < protogameEffect.NativeEffect.Parameters.Count; i++)
                {
                    var nativeParameter = protogameEffect.NativeEffect.Parameters[i];
                    _writableParameters.Add(nativeParameter.Name, new ProtogameEffectWriteableParameter(
                        this, 
                        nativeParameter,
                        nativeParameter.Name,
                        nativeParameter.ParameterClass,
                        nativeParameter.ParameterType));
                }
                
                foreach (var semantic in protogameEffect._availableSemantics)
                {
                    if (semantic.ShouldAttachToParameterSet(this))
                    {
                        _semantics.Add(semantic.Clone(this));
                    }
                }
            }

            public void Load(IRenderContext renderContext, Effect targetEffect)
            {
                if (!IsLocked)
                {
                    foreach (var semantic in _semantics)
                    {
                        semantic.OnApply(renderContext);
                    }
                }

                for (var i = 0; i < _writableParameters.Count; i++)
                {
                    var parameter = (ProtogameEffectWriteableParameter) _writableParameters[i];
                    if (IsWorldViewProjectionParameter(parameter))
                    {
                        // World view projection parameters are managed by the batcher.
                        continue;
                    }
                    switch (parameter.InternalType)
                    {
                        case ProtogameEffectWriteableParameterInternalType.Unset:
                            break;
                        case ProtogameEffectWriteableParameterInternalType.Texture2D:
                            targetEffect.Parameters[parameter.Name].SetValue(parameter._texture2D);
                            break;
                        case ProtogameEffectWriteableParameterInternalType.Vector4:
                            targetEffect.Parameters[parameter.Name].SetValue(parameter._vector4);
                            break;
                        case ProtogameEffectWriteableParameterInternalType.Vector3:
                            targetEffect.Parameters[parameter.Name].SetValue(parameter._vector3);
                            break;
                        case ProtogameEffectWriteableParameterInternalType.Vector2:
                            targetEffect.Parameters[parameter.Name].SetValue(parameter._vector2);
                            break;
                        case ProtogameEffectWriteableParameterInternalType.Float:
                            targetEffect.Parameters[parameter.Name].SetValue(parameter._float);
                            break;
                        case ProtogameEffectWriteableParameterInternalType.Matrix:
                            targetEffect.Parameters[parameter.Name].SetValue(parameter._matrix);
                            break;
                        case ProtogameEffectWriteableParameterInternalType.MatrixArray:
                            targetEffect.Parameters[parameter.Name].SetValue(parameter._matrices);
                            break;
                        default:
                            throw new NotSupportedException(parameter.InternalType.ToString());
                    }
                }

                _locked = true;
            }

            public bool IsLocked => _locked;

            public void Lock(IRenderContext renderContext)
            {
                if (!_locked)
                {
                    foreach (var semantic in _semantics)
                    {
                        semantic.OnApply(renderContext);
                    }
                }

                _locked = true;
            }

            public void Unlock()
            {
                _locked = false;
            }

            private bool IsWorldViewProjectionParameter(ProtogameEffectWriteableParameter wp)
            {
                return wp.Name == "WorldViewProj" || wp.Name == "World" || wp.Name == "View" || wp.Name == "Projection";
            }

            public int GetStateHash()
            {
                var h = _writableParameters.Count ^ 397;
                for (var i = 0; i < _writableParameters.Count; i++)
                {
                    var wp = (ProtogameEffectWriteableParameter) _writableParameters[i];
                    if (IsWorldViewProjectionParameter(wp))
                    {
                        // Exclude parameters relating to view-world-projection matrices, these are managed by the batcher
                        // and don't make sense when considering state caching.
                        continue;
                    }
                    h += wp.GetStateHash();
                }
                return h;
            }

            public bool HasSemantic<T>() where T : IEffectSemantic
            {
                return _semantics.OfType<T>().Any();
            }

            public T GetSemantic<T>() where T : IEffectSemantic
            {
                return _semantics.OfType<T>().First();
            }

            private enum ProtogameEffectWriteableParameterInternalType
            {
                Unset,
                Texture2D,
                Vector2,
                Vector3,
                Vector4,
                Float,
                Matrix,
                MatrixArray,
            }

            private class ProtogameEffectWriteableParameter : IEffectWritableParameter
            {
                private readonly ProtogameParameterSet _protogameParameterSet;
                private readonly EffectParameter _nativeParameter;
                private readonly string _name;
                public Texture2D _texture2D;
                public Vector4 _vector4;
                public Vector3 _vector3;
                public Vector2 _vector2;
                public float _float;
                public Matrix _matrix;
                public Matrix[] _matrices;
                private int _valueStateHash;
                private readonly int _nameStateHash;

                public ProtogameEffectWriteableParameter(
                    ProtogameParameterSet protogameParameterSet,
                    EffectParameter nativeParameter, 
                    string name,
                    EffectParameterClass parameterClass,
                    EffectParameterType parameterType)
                {
                    _protogameParameterSet = protogameParameterSet;
                    _nativeParameter = nativeParameter;
                    _name = name;
                    _nameStateHash = _name.GetHashCode() ^ 397;
                }

                public EffectParameter NativeParameter => _nativeParameter;

                public string Name => _name;

                public Texture2D GetValueTexture2D()
                {
                    return _texture2D;
                }

                public Vector4 GetValueVector4()
                {
                    return _vector4;
                }

                public float GetValueSingle()
                {
                    return _float;
                }

                public Color GetValueColor()
                {
                    return new Color(_vector4);
                }

                public ProtogameEffectWriteableParameterInternalType InternalType { get; private set; }

                public int GetStateHash()
                {
                    return _nameStateHash + _valueStateHash;
                }

                public void SetValue(Texture2D texture)
                {
                    if (_protogameParameterSet._locked)
                    {
                        throw new InvalidOperationException(
                            "The effect parameter set is locked because it has been assigned to a " +
                            "render request.  Create a new parameter set instead of modifying this one.");
                    }

                    _texture2D = texture;
                    _valueStateHash = _texture2D.GetHashCode() ^ 397;
                    InternalType = ProtogameEffectWriteableParameterInternalType.Texture2D;
                }

                public void SetValue(Vector4 vector)
                {
                    if (_protogameParameterSet._locked)
                    {
                        throw new InvalidOperationException(
                            "The effect parameter set is locked because it has been assigned to a " +
                            "render request.  Create a new parameter set instead of modifying this one.");
                    }

                    _vector4 = vector;
                    _valueStateHash = _vector4.GetHashCode() ^ 397;
                    InternalType = ProtogameEffectWriteableParameterInternalType.Vector4;
                }

                public void SetValue(Vector3 vector)
                {
                    if (_protogameParameterSet._locked)
                    {
                        throw new InvalidOperationException(
                            "The effect parameter set is locked because it has been assigned to a " +
                            "render request.  Create a new parameter set instead of modifying this one.");
                    }

                    _vector3 = vector;
                    _valueStateHash = _vector3.GetHashCode() ^ 397;
                    InternalType = ProtogameEffectWriteableParameterInternalType.Vector3;
                }

                public void SetValue(Vector2 vector)
                {
                    if (_protogameParameterSet._locked)
                    {
                        throw new InvalidOperationException(
                            "The effect parameter set is locked because it has been assigned to a " +
                            "render request.  Create a new parameter set instead of modifying this one.");
                    }

                    _vector2 = vector;
                    _valueStateHash = _vector2.GetHashCode() ^ 397;
                    InternalType = ProtogameEffectWriteableParameterInternalType.Vector2;
                }

                public void SetValue(float value)
                {
                    if (_protogameParameterSet._locked)
                    {
                        throw new InvalidOperationException(
                            "The effect parameter set is locked because it has been assigned to a " +
                            "render request.  Create a new parameter set instead of modifying this one.");
                    }

                    _float = value;
                    _valueStateHash = _float.GetHashCode() ^ 397;
                    InternalType = ProtogameEffectWriteableParameterInternalType.Float;
                }

                public void SetValue(Matrix matrix)
                {
                    if (_protogameParameterSet._locked)
                    {
                        throw new InvalidOperationException(
                            "The effect parameter set is locked because it has been assigned to a " +
                            "render request.  Create a new parameter set instead of modifying this one.");
                    }

                    _matrix = matrix;
                    _valueStateHash = _matrix.GetHashCode() ^ 397;
                    InternalType = ProtogameEffectWriteableParameterInternalType.Matrix;
                }

                public void SetValue(Matrix[] matrices)
                {
                    if (_protogameParameterSet._locked)
                    {
                        throw new InvalidOperationException(
                            "The effect parameter set is locked because it has been assigned to a " +
                            "render request.  Create a new parameter set instead of modifying this one.");
                    }

                    _matrices = matrices;
                    _valueStateHash = _matrices.GetHashCode() ^ 397;
                    InternalType = ProtogameEffectWriteableParameterInternalType.MatrixArray;
                }

                public void SetValue(Color color)
                {
                    if (_protogameParameterSet._locked)
                    {
                        throw new InvalidOperationException(
                            "The effect parameter set is locked because it has been assigned to a " +
                            "render request.  Create a new parameter set instead of modifying this one.");
                    }

                    _vector4 = color.ToVector4();
                    _valueStateHash = _vector4.GetHashCode() ^ 397;
                    InternalType = ProtogameEffectWriteableParameterInternalType.Vector4;
                }
            }

            public IEffectWritableParameter this[int index]
            {
                get { return (IEffectWritableParameter)_writableParameters[index]; }
            }

            public IEffectWritableParameter this[string name]
            {
                get { return (IEffectWritableParameter)_writableParameters[name]; }
            }
        }
    }
}