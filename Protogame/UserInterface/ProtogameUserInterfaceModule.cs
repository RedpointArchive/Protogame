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
            kernel.Bind<IUserInterfaceNodeProcessor>()
                .To<CanvasFragmentUserInterfaceNodeProcessor>()
                .Named("fragment")
                .InSingletonScope();

            kernel.Bind<ISkinRenderer<AudioPlayer>>().To<BasicAudioPlayerSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<Button>>().To<BasicButtonSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<Canvas>>().To<BasicCanvasSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<FileSelect>>().To<BasicFileSelectSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<FixedContainer>>().To<BasicFixedContainerSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<FontViewer>>().To<BasicFontViewerSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<Form>>().To<BasicFormSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<HorizontalContainer>>().To<BasicHorizontalContainerSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<Label>>().To<BasicLabelSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<Link>>().To<BasicLinkSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<ListItem>>().To<BasicListItemSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<ListView>>().To<BasicListViewSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<MainMenu>>().To<BasicMainMenuSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<MenuItem>>().To<BasicMenuItemSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<ScrollableContainer>>().To<BasicScrollableContainerSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<SingleContainer>>().To<BasicSingleContainerSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<TextBox>>().To<BasicTextBoxSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<TextureViewer>>().To<BasicTextureViewerSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<TreeItem>>().To<BasicTreeItemSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<TreeView>>().To<BasicTreeViewSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<VerticalContainer>>().To<BasicVerticalContainerSkinRenderer>().InSingletonScope();
            kernel.Bind<ISkinRenderer<Window>>().To<BasicWindowSkinRenderer>().InSingletonScope();

            kernel.Bind<IBasicSkinHelper>().To<BasicSkinHelper>().InSingletonScope();
            kernel.Bind<ISkinLayout>().To<BasicSkinLayout>().InSingletonScope();
            kernel.Bind<ISkinDelegator>().To<DefaultSkinDelegator>().InSingletonScope();
        }
    }
}
