using Protoinject;

namespace Protogame
{
    public interface IUserInterfaceFactory : IGenerateFactory
    {
        IUserInterfaceController CreateUserInterfaceController(UserInterfaceAsset userInterfaceAsset);
    }
}