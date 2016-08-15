using Protoinject;

namespace Protogame
{
    public class ProtogameUserInterfaceModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<IUserInterfaceFactory>().ToFactory();
            kernel.Bind<IUserInterfaceController>().To<DefaultUserInterfaceController>();

            kernel.Bind<IUserInterfaceNodeProcessor>()
                .To<CanvasUserInterfaceNodeProcessor>()
                .Named("canvas")
                .InSingletonScope();
            kernel.Bind<IUserInterfaceNodeProcessor>()
                .To<ContainerUserInterfaceNodeProcessor>()
                .Named("container")
                .InSingletonScope();
            kernel.Bind<IUserInterfaceNodeProcessor>()
                .To<ButtonUserInterfaceNodeProcessor>()
                .Named("button")
                .InSingletonScope();
            kernel.Bind<IUserInterfaceNodeProcessor>()
                .To<TextBoxUserInterfaceNodeProcessor>()
                .Named("textbox")
                .InSingletonScope();
            kernel.Bind<IUserInterfaceNodeProcessor>()
                .To<LabelUserInterfaceNodeProcessor>()
                .Named("label")
                .InSingletonScope();
            kernel.Bind<IUserInterfaceNodeProcessor>()
                .To<ListViewUserInterfaceNodeProcessor>()
                .Named("listview")
                .InSingletonScope();
            kernel.Bind<IUserInterfaceNodeProcessor>()
                .To<ListItemUserInterfaceNodeProcessor>()
                .Named("listitem")
                .InSingletonScope();
        }
        
        public static ISkin GetDefaultSkin(IKernel kernel)
        {
            return new BasicSkin(
                new DefaultBasicSkin(),
                kernel.Get<I2DRenderUtilities>(),
                kernel.Get<IAssetManagerProvider>());
        }
    }
}
