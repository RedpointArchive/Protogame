namespace Protogame
{
    public delegate void UserInterfaceBehaviourHandler<in TContainerType>(
        TContainerType container, IUserInterfaceController controller, IGameContext gameContext,
        IUpdateContext updateContext);
}