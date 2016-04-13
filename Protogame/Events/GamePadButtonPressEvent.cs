namespace Protogame
{
    /// <summary>
    /// Represents that the user has pressed a button on a game pad.  This event is only fired once for each
    /// button press, allowing it to be used for single trigger actions (such as "jump").
    /// </summary>
    /// <module>Events</module>
    public class GamePadButtonPressEvent : GamePadButtonEvent
    {
    }
}