namespace Protogame
{
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The KeyboardStringReader interface.
    /// </summary>
    public interface IKeyboardStringReader
    {
        /// <summary>
        /// Gets or sets the first repeat key interval.
        /// </summary>
        /// <value>
        /// The first repeat key interval.
        /// </value>
        int FirstRepeatKeyInterval { get; set; }

        /// <summary>
        /// Gets or sets the repeat key interval.
        /// </summary>
        /// <value>
        /// The repeat key interval.
        /// </value>
        int RepeatKeyInterval { get; set; }

        /// <summary>
        /// The process.
        /// </summary>
        /// <param name="keyboard">
        /// The keyboard.
        /// </param>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        void Process(KeyboardState keyboard, GameTime time, StringBuilder text);
    }
}