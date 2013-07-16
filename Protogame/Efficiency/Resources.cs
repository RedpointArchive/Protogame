//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    static class Resources
    {
        private static ContentManager GetContentManager(GraphicsDevice device)
        {
            return new ContentManager(new GraphicsDeviceWrapper(device));
        }

        private class GraphicsDeviceWrapper : IServiceProvider, IGraphicsDeviceService
        {
            private GraphicsDevice m_Device;

            public GraphicsDeviceWrapper(GraphicsDevice device)
            {
                this.m_Device = device;
            }

            #region IServiceProvider Members

            public object GetService(Type serviceType)
            {
                return this;
            }

            #endregion

            #region IGraphicsDeviceService Members

            public event EventHandler<EventArgs> DeviceCreated;
            public event EventHandler<EventArgs> DeviceDisposing;
            public event EventHandler<EventArgs> DeviceReset;
            public event EventHandler<EventArgs> DeviceResetting;

            public GraphicsDevice GraphicsDevice
            {
                get { return this.m_Device; }
            }

            #endregion
        }

        #region Loading Methods

        public static Effect LoadOccludableSpriteEffect(GraphicsDevice device)
        {
            return GetContentManager(device).Load<Effect>("Protogame.Efficiency.Content/OccludableSpriteEffect");
        }

        public static Effect LoadOccludingEffect(GraphicsDevice device)
        {
            return GetContentManager(device).Load<Effect>("Protogame.Efficiency.Content/OccludingEffect");
        }

        #endregion
    }
}
