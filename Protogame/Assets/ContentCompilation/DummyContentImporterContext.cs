#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Framework.Content.Pipeline.Builder;
using MonoGamePlatform = Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform;

namespace Protogame
{
    public class DummyContentImporterContext : ContentImporterContext
    {
        private readonly PipelineBuildLogger m_Logger;

        public DummyContentImporterContext()
        {
            this.m_Logger = new PipelineBuildLogger();
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
            get
            {
                return this.m_Logger;
            }
        }

        public override string OutputDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void AddDependency(string filename)
        {
            throw new NotImplementedException();
        }
    }
}

#endif