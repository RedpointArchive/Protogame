using System;
using Protogame;
using Protoinject;
using Microsoft.Xna.Framework;

namespace ProtogameEditor
{
    public class EditorGame : CoreGame<EditorWorld, Default3DWorldManager>
    {
        public EditorGame(IKernel kernel) : base(kernel)
        {
        }

        protected override void LoadContent ()
        {
            this.GraphicsDeviceManager.GraphicsDevice.Clear(new Color(63, 63, 63));

            base.LoadContent ();
        }
    }
}

