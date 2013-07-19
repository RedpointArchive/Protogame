//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Net;
using Data4;
using Process4;
using Process4.Interfaces;

namespace Protogame
{
    public class ProtogameAssetManagerNetwork : INetworkProvider
    {
        private static readonly ID ClientID =
            new ID(
                Guid.Parse("8b335407-ef34-4ce3-b3a3-d5f21b487df2"),
                Guid.Parse("84748d6a-14b7-4ecf-8d2c-ec218f361fef"),
                Guid.Parse("12a92ece-c6b6-43f5-80c2-501b629a7caf"),
                Guid.Parse("c423dbc0-f151-4081-ae7b-9a93a22a3662"));
        private static readonly ID AssetManagerID =
            new ID(
                Guid.Parse("ab019f90-ca50-461c-a93e-2c2f27787235"),
                Guid.Parse("11b08119-17cb-486b-b309-b5e13275c0b3"),
                Guid.Parse("1cdb068a-3b00-4442-b7d4-cf051df5db3b"),
                Guid.Parse("f9db9e39-eb6f-4e1a-8f4b-0ead4c109c2d"));

        public ProtogameAssetManagerNetwork(LocalNode node, bool isAssetManager)
        {
            this.Node = node;
            this.IsAssetManager = isAssetManager;
        }

        public bool IsAssetManager
        {
            get;
            private set;
        }

        #region INetworkProvider Members

        public LocalNode Node { get; private set; }

        public ID ID
        {
            get
            {
                if (this.IsAssetManager)
                    return AssetManagerID;
                return ClientID;
            }
        }

        public int DiscoveryPort { get { return 9836; } }

        public int MessagingPort
        {
            get
            {
                if (this.IsAssetManager)
                    return 9837;
                return 9838;
            }
        }

        public IPAddress IPAddress
        {
            get
            {
                return IPAddress.Loopback;
            }
        }

        public bool IsFirst { get { return this.IsAssetManager; } }

        #endregion

        public void Join(ID id)
        {
            // We don't care about the ID.
            Contact contact;
            if (this.IsAssetManager)
                contact = new Contact(ClientID, new IPEndPoint(IPAddress.Loopback, 9838));
            else
                contact = new Contact(AssetManagerID, new IPEndPoint(IPAddress.Loopback, 9837));
            this.Node.Contacts.Add(contact);
        }

        public void Leave()
        {
            // Don't care about leaving either.
        }
    }
}
