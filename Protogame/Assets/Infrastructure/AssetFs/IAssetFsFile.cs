using System;
using System.IO;
using System.Threading.Tasks;

namespace Protogame
{
    public interface IAssetFsFile
    {
        string Name { get; }

        string Extension { get; }

        DateTimeOffset ModificationTimeUtcTimestamp { get; }

        Task<Stream> GetContentStream();

        /*
         * 
         * ********************************** TODO EACH FILE MUST HAVE A DEPENDENCIES LIST *****************************
         * **************** THIS IS NOT IMPLEMENTED, BUT IS REQUIRED SO THAT COMPILED ASSETS GET RECOMPILED WHEN *******************
         * ****************** ANY OF THEIR DEPENDENCIES CHANGE AS WELL *****************************
         */
        Task<string[]> GetDependentOnAssetFsFileNames();
    }
}
