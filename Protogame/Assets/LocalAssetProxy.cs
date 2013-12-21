namespace Protogame
{
    using System;
    using System.Reflection;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;

    /// <summary>
    /// An asset proxy for local debugging.  This allows you to reload assets transparently
    /// on the fly without explicit checks in your game logic.
    /// </summary>
    /// <typeparam name="T">The type of asset that this is a proxy for.</typeparam>
    public class LocalAssetProxy<T> : RealProxy
        where T : class, IAsset
    {
        /// <summary>
        /// The m_ asset name.
        /// </summary>
        private readonly string m_AssetName;

        /// <summary>
        /// The m_ manager.
        /// </summary>
        private readonly LocalAssetManager m_Manager;

        /// <summary>
        /// The m_ dirty.
        /// </summary>
        private bool m_Dirty;

        /// <summary>
        /// The m_ instance.
        /// </summary>
        private T m_Instance;

        private LocalAsset m_LocalAsset;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalAssetProxy{T}"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <param name="networkAsset">
        /// The network asset.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public LocalAssetProxy(LocalAssetManager manager, LocalAsset networkAsset, string name, T instance)
            : base(instance.GetType())
        {
            this.m_Instance = instance;
            this.m_Manager = manager;
            this.m_LocalAsset = networkAsset;
            this.m_AssetName = name;
            this.m_Dirty = false;

            this.m_LocalAsset.Dirtied += this.MarkDirty;
        }

        /// <summary>
        /// The mark dirty.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MarkDirty(object sender, EventArgs e)
        {
            this.m_Dirty = true;
        }

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <returns>
        /// The <see cref="IMessage"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public override IMessage Invoke(IMessage msg)
        {
            if (this.m_Dirty)
            {
                this.m_LocalAsset.Dirtied -= this.MarkDirty;
                this.m_LocalAsset = this.m_Manager.GetUnresolved(this.m_AssetName) as LocalAsset;
                var proxy = this.m_LocalAsset.Resolve<T>();
                if (!RemotingServices.IsTransparentProxy(proxy))
                {
                    throw new InvalidOperationException("Object retrieved was not transparent proxy.");
                }

                var realProxy = RemotingServices.GetRealProxy(proxy);
                var newNetworkAssetProxy = realProxy as LocalAssetProxy<T>;
                if (newNetworkAssetProxy == null)
                {
                    throw new InvalidOperationException("Unable to cast real proxy back to NetworkAssetProxy<>.");
                }

                this.m_Instance = newNetworkAssetProxy.m_Instance;
                this.m_Dirty = false;
                this.m_LocalAsset.Dirtied += this.MarkDirty;
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