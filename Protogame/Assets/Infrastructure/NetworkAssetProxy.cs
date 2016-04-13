
#if FALSE

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
        private bool m_Dirty;

        public NetworkAssetProxy(NetworkAssetManager manager, NetworkAsset networkAsset, string name, T instance)
             : base(instance.GetType())
        {
            this.m_Instance = instance;
            this.m_Manager = manager;
            this.m_NetworkAsset = networkAsset;
            this.m_AssetName = name;
            this.m_Dirty = false;
            
            this.m_NetworkAsset.Dirtied += MarkDirty;
        }

        private void MarkDirty(object sender, EventArgs e)
        {
            this.m_Dirty = true;
        }

        public override IMessage Invoke(IMessage msg)
        {
            if (this.m_Dirty)
            {
                this.m_NetworkAsset.Dirtied -= MarkDirty;
                this.m_NetworkAsset = this.m_Manager.GetUnresolved(this.m_AssetName) as NetworkAsset;
                var proxy = this.m_NetworkAsset.Resolve<T>();
                if (!RemotingServices.IsTransparentProxy(proxy))
                    throw new InvalidOperationException("Object retrieved was not transparent proxy.");
                var realProxy = RemotingServices.GetRealProxy(proxy);
                var newNetworkAssetProxy = realProxy as NetworkAssetProxy<T>;
                if (newNetworkAssetProxy == null)
                    throw new InvalidOperationException("Unable to cast real proxy back to NetworkAssetProxy<>.");
                this.m_Instance = newNetworkAssetProxy.m_Instance;
                this.m_Dirty = false;
                this.m_NetworkAsset.Dirtied += MarkDirty;
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

#endif