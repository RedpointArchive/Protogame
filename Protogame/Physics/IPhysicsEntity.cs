using System;
using Jitter.Dynamics;
using Jitter;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IPhysicsEntity : IHasPosition
	{
        Matrix Rotation { get; set; }
	}
}

