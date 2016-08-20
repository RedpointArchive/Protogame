using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HiveMP.Attribute.Api;
using HiveMP.Attribute.Client;
using HiveMP.Attribute.Model;
using HiveMP.TemporarySession.Model;
using Newtonsoft.Json;

namespace Protogame
{
    /// <summary>
    /// Default implementation of <see cref="IHiveAttributeManagement"/>.
    /// </summary>
    /// <module>Hive</module>
    /// <internal>true</internal>
    /// <interface_ref>Protogame.IHiveAttributeManagement</interface_ref>
    public class DefaultHiveAttributeManagement : IHiveAttributeManagement
    {
        public async Task<T> GetAttribute<T>(TempSessionWithSecrets userSession, string objectId, string ownerId, string key)
        {
            var attributeApi = new AttributeApi();
            attributeApi.Configuration.ApiKey["api_key"] = userSession.ApiKey;
            try
            {
                var result = await attributeApi.AttributeGetAsync(objectId, key, ownerId);
                return JsonConvert.DeserializeObject<T>(result.Value);
            }
            catch (JsonException)
            {
                // Badly formatted data, to prevent other sessions
                // potentially causing issues by storing incorrect data,
                // we just return the default value for T here.
                return default(T);
            }
            catch (ApiException ex)
            {
                if (ex.ErrorCode == 404)
                {
                    return default(T);
                }

                throw;
            }
        }

        public async Task<IEnumerable<AttributeKey>> GetAttributeKeys(TempSessionWithSecrets userSession, string objectId)
        {
            var attributeApi = new AttributeApi();
            attributeApi.Configuration.ApiKey["api_key"] = userSession.ApiKey;
            return await attributeApi.AttributesGetAsync(objectId);
        }

        public async Task SetAttribute<T>(TempSessionWithSecrets userSession, string objectId, string key, T value)
        {
            var attributeApi = new AttributeApi();
            attributeApi.Configuration.ApiKey["api_key"] = userSession.ApiKey;
            await attributeApi.AttributePutAsync(objectId, key, JsonConvert.SerializeObject(value));
        }
    }
}
