#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using MonoGamePlatform = Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform;

namespace Protogame
{
    /// <summary>
    /// The dummy content processor context.
    /// </summary>
    public class DummyContentProcessorContext : ContentProcessorContext
    {
        /// <summary>
        /// The m_ logger.
        /// </summary>
        private readonly PipelineBuildLogger m_Logger;

        /// <summary>
        /// The m_ target platform.
        /// </summary>
        private readonly MonoGamePlatform m_TargetPlatform;

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyContentProcessorContext"/> class.
        /// </summary>
        /// <param name="targetPlatform">
        /// The target platform.
        /// </param>
        public DummyContentProcessorContext(MonoGamePlatform targetPlatform)
        {
            this.m_TargetPlatform = targetPlatform;
            this.m_Logger = new PipelineBuildLogger();
        }

        /// <summary>
        /// Gets or sets the actual output filename.
        /// </summary>
        /// <value>
        /// The actual output filename.
        /// </value>
        public string ActualOutputFilename { get; set; }

        /// <summary>
        /// Gets the build configuration.
        /// </summary>
        /// <value>
        /// The build configuration.
        /// </value>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override string BuildConfiguration
        {
            get
            {
                throw new NotImplementedException();
            }
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
        /// Gets the output filename.
        /// </summary>
        /// <value>
        /// The output filename.
        /// </value>
        public override string OutputFilename
        {
            get
            {
                return this.ActualOutputFilename;
            }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override OpaqueDataDictionary Parameters
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the target platform.
        /// </summary>
        /// <value>
        /// The target platform.
        /// </value>
        public override MonoGamePlatform TargetPlatform
        {
            get
            {
                return this.m_TargetPlatform;
            }
        }

        /// <summary>
        /// Gets the target profile.
        /// </summary>
        /// <value>
        /// The target profile.
        /// </value>
        public override GraphicsProfile TargetProfile
        {
            get
            {
                return GraphicsProfile.HiDef;
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

        /// <summary>
        /// The add output file.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override void AddOutputFile(string filename)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The build and load asset.
        /// </summary>
        /// <param name="sourceAsset">
        /// The source asset.
        /// </param>
        /// <param name="processorName">
        /// The processor name.
        /// </param>
        /// <param name="processorParameters">
        /// The processor parameters.
        /// </param>
        /// <param name="importerName">
        /// The importer name.
        /// </param>
        /// <typeparam name="TInput">
        /// </typeparam>
        /// <typeparam name="TOutput">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TOutput"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override TOutput BuildAndLoadAsset<TInput, TOutput>(
            ExternalReference<TInput> sourceAsset, 
            string processorName, 
            OpaqueDataDictionary processorParameters, 
            string importerName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The build asset.
        /// </summary>
        /// <param name="sourceAsset">
        /// The source asset.
        /// </param>
        /// <param name="processorName">
        /// The processor name.
        /// </param>
        /// <param name="processorParameters">
        /// The processor parameters.
        /// </param>
        /// <param name="importerName">
        /// The importer name.
        /// </param>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        /// <typeparam name="TInput">
        /// </typeparam>
        /// <typeparam name="TOutput">
        /// </typeparam>
        /// <returns>
        /// The <see cref="ExternalReference"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override ExternalReference<TOutput> BuildAsset<TInput, TOutput>(
            ExternalReference<TInput> sourceAsset, 
            string processorName, 
            OpaqueDataDictionary processorParameters, 
            string importerName, 
            string assetName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="processorName">
        /// The processor name.
        /// </param>
        /// <param name="processorParameters">
        /// The processor parameters.
        /// </param>
        /// <typeparam name="TInput">
        /// </typeparam>
        /// <typeparam name="TOutput">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TOutput"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override TOutput Convert<TInput, TOutput>(
            TInput input, 
            string processorName, 
            OpaqueDataDictionary processorParameters)
        {
            throw new NotImplementedException();
        }
    }
}

#endif