using System;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IHivePublicAuthentication"/>.
    /// </summary>
    /// <module>Hive</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IHivePublicAuthentication</interface_ref>
    public class DefaultHivePublicAuthentication : IHivePublicAuthentication
    {
        private string _publicApiKey;
        
        public string PublicApiKey
        {
            get
            {
                if (_publicApiKey == null)
                {
                    throw new InvalidOperationException(
                        "You must configure a public API key before using the Hive APIs.  " + 
                        "For example: _hivePublicAuthentication.PublicApiKey = \"...\";");
                }

                return _publicApiKey;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _publicApiKey = value;
            }
        }
    }
}