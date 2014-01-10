namespace Protogame
{
    /// <summary>
    /// Represents that the user has held a button on a game pad.  This event is fired for every frame that the
    /// button is held down for, allowing it to be used for continuous input (such as "sprint").
    /// </summary>
    public class GamePadButtonHeldEvent : GamePadButtonEvent
    {
    }
}