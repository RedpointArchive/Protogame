namespace Protogame
{
    public interface ITickRegulator
    {
        bool EmitSlowTicks { get; set; }

        void WaitUntilReady();
    }
}

