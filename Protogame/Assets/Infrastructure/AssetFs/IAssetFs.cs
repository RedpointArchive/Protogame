using System.Threading.Tasks;

namespace Protogame
{
    public interface IAssetFs
    {
        void Update();

        Task<IAssetFsFile[]> List();

        Task<IAssetFsFile> Get(string name);
    }
}
