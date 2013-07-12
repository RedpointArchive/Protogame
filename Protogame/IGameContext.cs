using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IGameContext
    {
        Camera Camera { get; }
        Game Game { get; }
        GameTime GameTime { get; set; }
        GameWindow Window { get; }
        GraphicsDeviceManager Graphics { get; }
        int FPS { get; set; }
        int FrameCount { get; set; }
        IWorld World { get; }
        IWorldManager WorldManager { get; }
        
        IWorld CreateWorld<T>() where T : IWorld;
        void SwitchWorld<T>() where T : IWorld;
        void SwitchWorld<T>(T world) where T : IWorld;
        void ResizeWindow(int width, int height);
    }
}

