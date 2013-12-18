#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using MonoGamePlatform = Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform;

namespace Protogame
{
    public class DummyContentProcessorContext : ContentProcessorContext
    {
        private MonoGamePlatform m_TargetPlatform;
        private PipelineBuildLogger m_Logger;

        public DummyContentProcessorContext(MonoGamePlatform targetPlatform)
        {
            this.m_TargetPlatform = targetPlatform;
            this.m_Logger = new PipelineBuildLogger();
        }
    
        public override void AddDependency(string filename)
        {
            throw new NotImplementedException();
        }
        
        public override void AddOutputFile(string filename)
        {
            throw new NotImplementedException();
        }
        
        public override TOutput BuildAndLoadAsset<TInput, TOutput>(ExternalReference<TInput> sourceAsset, string processorName, OpaqueDataDictionary processorParameters, string importerName)
        {
            throw new NotImplementedException();
        }
        
        public override ExternalReference<TOutput> BuildAsset<TInput, TOutput>(ExternalReference<TInput> sourceAsset, string processorName, OpaqueDataDictionary processorParameters, string importerName, string assetName)
        {
            throw new NotImplementedException();
        }
        
        public override TOutput Convert<TInput, TOutput>(TInput input, string processorName, OpaqueDataDictionary processorParameters)
        {
            throw new NotImplementedException();
        }
        
        public override string BuildConfiguration
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        
        public override string IntermediateDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        
        public override ContentBuildLogger Logger
        {
            get { return this.m_Logger; }
        }
        
        public override string OutputDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        
        public override string OutputFilename
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        
        public override OpaqueDataDictionary Parameters
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override MonoGamePlatform TargetPlatform
        {
            get { return this.m_TargetPlatform; }
        }
        
        public override GraphicsProfile TargetProfile
        {
            get { return GraphicsProfile.HiDef; }
        }
    }
}

#endif
