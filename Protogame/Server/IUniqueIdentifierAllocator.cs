namespace Protogame
{
    public interface IUniqueIdentifierAllocator
    {
        int Current { get; set; }

        int Allocate();
    }
}

