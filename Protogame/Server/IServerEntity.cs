// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;

namespace Protogame
{
    /// <summary>
    /// The ServerEntity interface.
    /// </summary>
    public interface IServerEntity : IHasPosition
    {
        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="serverContext">
        /// The server context.
        /// </param>
        /// <param name="updateContext">
        /// The update context.
        /// </param>
        void Update(IServerContext serverContext, IUpdateContext updateContext);
    }
}

