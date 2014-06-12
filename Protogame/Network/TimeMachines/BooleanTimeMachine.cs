namespace Protogame
{
    /// <summary>
    /// A time machine for a boolean.
    /// </summary>
    public class BooleanTimeMachine : TimeMachine<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanTimeMachine"/> class.
        /// </summary>
        /// <param name="history">
        /// The amount of history to store in this time machine.
        /// </param>
        public BooleanTimeMachine(int history)
            : base(history)
        {
        }
    }
}