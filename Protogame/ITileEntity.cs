namespace Protogame
{
    public interface ITileEntity : IEntity
    {
        int TX { get; set; }
        int TY { get; set; }
    }
}
