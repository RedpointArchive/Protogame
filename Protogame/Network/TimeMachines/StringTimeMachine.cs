namespace Protogame
{
    /// <summary>
    /// A time machine for a string.
    /// </summary>
    /// <module>Network</module>
    public class StringTimeMachine : TimeMachine<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringTimeMachine"/> class.
        /// </summary>
        /// <param name="history">
        /// The amount of history to store in this time machine.
        /// </param>
        public StringTimeMachine(int history)
            : base(history)
        {
        }
    }
}