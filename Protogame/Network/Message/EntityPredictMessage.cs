// ReSharper disable CheckNamespace

namespace Protogame
{
    public interface IEntityPredictMessage : INetworkMessage
    { 
        int EntityID { get; set; }

        int PredictionID { get; set; }
    }
}
