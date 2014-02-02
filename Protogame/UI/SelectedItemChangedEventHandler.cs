namespace Protogame
{
    /// <summary>
    /// The selected item changed event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public delegate void SelectedItemChangedEventHandler<T>(object sender, SelectedItemChangedEventArgs<T> args);
}