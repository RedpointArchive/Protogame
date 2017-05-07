using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protogame
{
    public class UserInterfaceComponent : IUpdatableComponent
    {
        private readonly IAssetManager _assetManager;
        private string _userInterfaceControllerAsset;
        private UserInterfaceAsset _userInterfaceAsset;
        private IAssetReference<UserInterfaceAsset> _userInterfaceAssetRef;
        private IUserInterfaceController _userInterfaceController;
        private readonly IUserInterfaceFactory _userInterfaceFactory;
        private List<Action<IUserInterfaceController>> _behaviourRegistration;

        public UserInterfaceComponent(
            IAssetManager assetManager,
            IUserInterfaceFactory userInterfaceFactory)
        {
            _assetManager = assetManager;
            _userInterfaceFactory = userInterfaceFactory;
            _behaviourRegistration = new List<Action<IUserInterfaceController>>();
        }

        public string UserInterfaceAssetName { get; set; }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            if (!string.IsNullOrWhiteSpace(UserInterfaceAssetName))
            {
                if (_userInterfaceAssetRef == null ||
                    _userInterfaceControllerAsset != UserInterfaceAssetName)
                {
                    _userInterfaceAssetRef = _assetManager.Get<UserInterfaceAsset>(UserInterfaceAssetName);
                    _userInterfaceControllerAsset = UserInterfaceAssetName;
                }

                if (_userInterfaceAssetRef.IsReady && _userInterfaceAssetRef.Asset != _userInterfaceAsset)
                {
                    if (_userInterfaceController != null)
                    {
                        _userInterfaceController.Enabled = false;
                    }

                    _userInterfaceAsset = _userInterfaceAssetRef.Asset;
                    _userInterfaceController = _userInterfaceFactory.CreateUserInterfaceController(_userInterfaceAsset);
                    _userInterfaceController.Enabled = true;

                    // Register behaviours.
                    foreach (var behaviour in _behaviourRegistration)
                    {
                        behaviour(_userInterfaceController);
                    }
                }
            }

            if (_userInterfaceController != null)
            {
                _userInterfaceController.Update(gameContext, updateContext);
            }
        }

        public void RegisterBehaviour<TContainerType>(string name, UserInterfaceBehaviourEvent @event, UserInterfaceBehaviourHandler<TContainerType> callback)
        {
            _behaviourRegistration.Add(controller => controller.RegisterBehaviour<TContainerType>(name, @event, callback));
        }
    }
}
