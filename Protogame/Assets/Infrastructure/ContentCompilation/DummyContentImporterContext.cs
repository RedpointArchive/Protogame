#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Framework.Content.Pipeline.Builder;
using MonoGamePlatform = Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform;

namespace Protogame
{
    /// <summary>
    /// The dummy content importer context.
    /// </summary>
    public class DummyContentImporterContext : ContentImporterContext
    {
        /// <summary>
        /// The m_ logger.
        /// </summary>
        private readonly PipelineBuildLogger m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyContentImporterContext"/> class.
        /// </summary>
        public DummyContentImporterContext()
        {
            this.m_Logger = new PipelineBuildLogger();
        }

        /// <summary>
        /// Gets the intermediate directory.
        /// </summary>
        /// <value>
        /// The intermediate directory.
        /// </value>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override string IntermediateDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public override ContentBuildLogger Logger
        {
            get
            {
                return this.m_Logger;
            }
        }

        /// <summary>
        /// Gets the output directory.
        /// </summary>
        /// <value>
        /// The output directory.
        /// </value>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override string OutputDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The add dependency.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override void AddDependency(string filename)
        {
            throw new NotImplementedException();
        }
    }
}

#endif