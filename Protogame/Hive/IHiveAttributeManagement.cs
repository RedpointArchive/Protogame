using System.Collections.Generic;
using System.Threading.Tasks;
using HiveMP.Attribute.Model;
using HiveMP.TemporarySession.Model;

namespace Protogame
{
    /// <summary>
    /// Allows you to set and retrieve custom attributes on Hive objects.
    /// </summary>
    /// <module>Hive</module>
    public interface IHiveAttributeManagement
    {
        /// <summary>
        /// Gets the value of an attribute stored against the given object by the specified owner.
        /// </summary>
        /// <typeparam name="T">The type of the value to return.  Must be JSON deserializable.</typeparam>
        /// <param name="userSession">The user session to perform the request as.</param>
        /// <param name="objectId">The object to retrieve the attribute from.</param>
        /// <param name="ownerId">The owner of the attribute.</param>
        /// <param name="key">The attribute key.</param>
        /// <returns>The attribute value.</returns>
        Task<T> GetAttribute<T>(TempSessionWithSecrets userSession, string objectId, string ownerId, string key);

        /// <summary>
        /// Gets a list of all attribute keys stored against an object.  You can use this method to find out
        /// the owner of a key for use with <see cref="GetAttribute{T}"/>.  Hive allows attributes to share
        /// key names if they are by different owners, to ensure that users can not overwrite the attributes
        /// set against an object by another user.
        /// </summary>
        /// <param name="userSession">The user session to perform the request as.</param>
        /// <param name="objectId">The object to retrieve the attribute keys from.</param>
        /// <returns>An enumeration of attribute keys against this object.</returns>
        Task<IEnumerable<AttributeKey>> GetAttributeKeys(TempSessionWithSecrets userSession, string objectId);

        /// <summary>
        /// Sets an attribute against a given object.  The specified user session will be the owner of
        /// the attribute.  The value must be JSON serializable.
        /// </summary>
        /// <remarks>
        /// The Protogame API for Hive attributes always stores the underlying attribute value in a
        /// JSON format.  If you are using the Hive API in another service or language, you'll need to
        /// deserialize the value as JSON when reading it, or serialize values as JSON if you want
        /// Protogame to be able to read them again later.
        /// </remarks>
        /// <typeparam name="T">The type of value to store.  Must be JSON serializable.</typeparam>
        /// <param name="userSession">The user session to perform the request as.  This user will own the attribute.</param>
        /// <param name="objectId">The object to set the attribute against.</param>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>An asynchronous task indicating the operation has completed.</returns>
        Task SetAttribute<T>(TempSessionWithSecrets userSession, string objectId, string key, T value);
    }
}
