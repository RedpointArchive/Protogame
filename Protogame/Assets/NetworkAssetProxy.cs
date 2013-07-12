//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using System.Runtime.Remoting;

namespace Protogame
{
    public class NetworkAssetProxy<T> : RealProxy where T : class, IAsset
    {
        private T m_Instance;
        private NetworkAsset m_NetworkAsset;
        private readonly NetworkAssetManager m_Manager;
        private readonly string m_AssetName;

        public NetworkAssetProxy(NetworkAssetManager manager, NetworkAsset networkAsset, string name, T instance)
             : base(instance.GetType())
        {
            this.m_Instance = instance;
            this.m_Manager = manager;
            this.m_NetworkAsset = networkAsset;
            this.m_AssetName = name;
        }

        public override IMessage Invoke(IMessage msg)
        {
            if (this.m_NetworkAsset.Dirty)
            {
                this.m_NetworkAsset = this.m_Manager.Get(this.m_AssetName) as NetworkAsset;
                var proxy = this.m_NetworkAsset.Resolve<T>();
                if (!RemotingServices.IsTransparentProxy(proxy))
                    throw new InvalidOperationException("Object retrieved was not transparent proxy.");
                var realProxy = RemotingServices.GetRealProxy(proxy);
                var newNetworkAssetProxy = realProxy as NetworkAssetProxy<T>;
                if (newNetworkAssetProxy == null)
                    throw new InvalidOperationException("Unable to cast real proxy back to NetworkAssetProxy<>.");
                this.m_Instance = newNetworkAssetProxy.m_Instance;
            }

            var methodCall = (IMethodCallMessage)msg;
            var method = (MethodInfo)methodCall.MethodBase;

            try
            {
                var result = method.Invoke(this.m_Instance, methodCall.InArgs);
                return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException && e.InnerException != null)
                {
                    return new ReturnMessage(e.InnerException, msg as IMethodCallMessage);
                }

                return new ReturnMessage(e, msg as IMethodCallMessage);
            }
        }
    }
}

