namespace Protogame
{
    /// <summary>
    /// Fired when the mouse wheel is scrolled.
    /// </summary>
    public class MouseScrollEvent : MouseEvent
    {
        /// <summary>
        /// The amount the mouse wheel has scrolled by.
        /// </summary>
        public float ScrollDelta { get; set; }
    }
}