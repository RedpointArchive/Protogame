namespace Protogame
{
    public interface IHasDesiredSize
    {
        int? GetDesiredWidth(ISkinLayout skin);

        int? GetDesiredHeight(ISkinLayout skin);
    }
}

