using System.Threading.Tasks;

namespace ProtogameAssetTool
{
    public interface IOperation
    {
        Task Run(OperationArguments args);
    }
}
