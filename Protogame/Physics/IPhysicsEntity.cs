using System;
using Jitter.Dynamics;
using Jitter;
using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// Represents a physics entity.
    /// </summary>
    /// <module>Physics</module>
    public interface IPhysicsEntity : IHasPosition
	{
        Matrix Rotation { get; set; }
	}
}

