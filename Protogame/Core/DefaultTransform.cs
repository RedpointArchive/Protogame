using System;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class DefaultTransform : ITransform
    {
        private bool _isSRTMatrix = false;

        private Vector3 _srtLocalPosition;

        private Quaternion _srtLocalRotation;

        private Vector3 _srtLocalScale;

        private Matrix _customLocalMatrix;

        private Matrix _cachedSRTLocalMatrix;

        private bool _isCachedSRTLocalMatrixUpToDate;

        private Matrix _cachedRTLocalMatrix;

        private bool _isCachedRTLocalMatrixUpToDate;

        public DefaultTransform()
        {
            _isSRTMatrix = true;
            _srtLocalPosition = Vector3.Zero;
            _srtLocalRotation = Quaternion.Identity;
            _srtLocalScale = Vector3.One;
            _cachedSRTLocalMatrix = Matrix.Identity;
            _isCachedSRTLocalMatrixUpToDate = true;
            _cachedRTLocalMatrix = Matrix.Identity;
            _isCachedRTLocalMatrixUpToDate = true;
        }

        #region Local Properties

        public Vector3 LocalPosition
        {
            get
            {
                if (_isSRTMatrix)
                {
                    return _srtLocalPosition;
                }
                else
                {
                    throw new InvalidOperationException("Local position can only be retrieved on transforms derived from SRT (scale-rotate-transform) matrices.");
                }
            }
            set
            {
                if (_isSRTMatrix)
                {
                    _srtLocalPosition = value;
                    _isCachedSRTLocalMatrixUpToDate = false;
                    _isCachedRTLocalMatrixUpToDate = false;
                }
                else
                {
                    throw new InvalidOperationException("Local position can only be assigned on transforms derived from SRT (scale-rotate-transform) matrices.");
                }
            }
        }

        public Quaternion LocalRotation
        {
            get
            {
                if (_isSRTMatrix)
                {
                    return _srtLocalRotation;
                }
                else
                {
                    throw new InvalidOperationException("Local rotation can only be retrieved on transforms derived from SRT (scale-rotate-transform) matrices.");
                }
            }
            set
            {
                if (_isSRTMatrix)
                {
                    _srtLocalRotation = value;
                    _isCachedSRTLocalMatrixUpToDate = false;
                    _isCachedRTLocalMatrixUpToDate = false;
                }
                else
                {
                    throw new InvalidOperationException("Local rotation can only be assigned on transforms derived from SRT (scale-rotate-transform) matrices.");
                }
            }
        }

        public Vector3 LocalScale
        {
            get
            {
                if (_isSRTMatrix)
                {
                    return _srtLocalScale;
                }
                else
                {
                    throw new InvalidOperationException("Local scale can only be retrieved on transforms derived from SRT (scale-rotate-transform) matrices.");
                }
            }
            set
            {
                if (_isSRTMatrix)
                {
                    _srtLocalScale = value;
                    _isCachedSRTLocalMatrixUpToDate = false;
                }
                else
                {
                    throw new InvalidOperationException("Local scale can only be assigned on transforms derived from SRT (scale-rotate-transform) matrices.");
                }
            }
        }

        public Matrix LocalMatrix
        {
            get
            {
                if (_isSRTMatrix)
                {
                    if (!_isCachedSRTLocalMatrixUpToDate)
                    {
                        RecalculateSRTMatrixCache();
                    }

                    return _cachedSRTLocalMatrix;
                }
                else
                {
                    return _customLocalMatrix;
                }
            }
        }

        public Matrix LocalMatrixWithoutScale
        {
            get
            {
                if (_isSRTMatrix)
                {
                    if (!_isCachedRTLocalMatrixUpToDate)
                    {
                        RecalculateRTMatrixCache();
                    }

                    return _cachedRTLocalMatrix;
                }
                else
                {
                    throw new InvalidOperationException("It is not possible to remove the scaling component from a non-SRT matrix.");
                }
            }
        }

        public bool IsSRTMatrix
        {
            get { return _isSRTMatrix; }
        }

        #endregion

        #region Type of Matrix Methods

        public void ResetAsSRTMatrix()
        {
            _isSRTMatrix = true;
            _srtLocalPosition = Vector3.Zero;
            _srtLocalRotation = Quaternion.Identity;
            _srtLocalScale = Vector3.One;
        }

        public void SetFromSRTMatrix(Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
        {
            _isSRTMatrix = true;
            _srtLocalPosition = localPosition;
            _srtLocalRotation = localRotation;
            _srtLocalScale = localScale;
        }

        public void ResetAsCustomMatrix()
        {
            _isSRTMatrix = false;
            _customLocalMatrix = Matrix.Identity;
        }

        public void SetFromCustomMatrix(Matrix localMatrix)
        {
            _isSRTMatrix = false;
            _customLocalMatrix = localMatrix;
        }

        #endregion

        #region Internal Caching

        private void RecalculateSRTMatrixCache()
        {
            if (!_isSRTMatrix)
            {
                throw new InvalidOperationException("Attempted to update SRT matrix cache for non-SRT matrix.");
            }

            if (_isCachedSRTLocalMatrixUpToDate)
            {
                throw new InvalidOperationException("Attempted to update SRT matrix cache when it's already up-to-date.  This would incur a performance penalty if the operation continued.");
            }

            _cachedSRTLocalMatrix =
                Matrix.CreateScale(_srtLocalScale) *
                Matrix.CreateFromQuaternion(_srtLocalRotation) *
                Matrix.CreateTranslation(_srtLocalPosition);
            _isCachedSRTLocalMatrixUpToDate = true;
        }

        private void RecalculateRTMatrixCache()
        {
            if (!_isSRTMatrix)
            {
                throw new InvalidOperationException("Attempted to update RT matrix cache for non-SRT matrix.");
            }

            if (_isCachedRTLocalMatrixUpToDate)
            {
                throw new InvalidOperationException("Attempted to update RT matrix cache when it's already up-to-date.  This would incur a performance penalty if the operation continued.");
            }

            _cachedRTLocalMatrix =
                Matrix.CreateFromQuaternion(_srtLocalRotation) *
                Matrix.CreateTranslation(_srtLocalPosition);
            _isCachedRTLocalMatrixUpToDate = true;
        }

        #endregion
    }
}
