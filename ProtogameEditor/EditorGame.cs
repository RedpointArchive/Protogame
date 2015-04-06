using System;
using Protogame;
using Ninject;
using Microsoft.Xna.Framework;

namespace ProtogameEditor
{
    public class EditorGame : CoreGame<EditorWorld, Default3DWorldManager>
    {
        public EditorGame(IKernel kernel) : base(kernel)
        {
        }
    }
}

