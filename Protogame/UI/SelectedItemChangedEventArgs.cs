namespace Protogame
{
    using System;

    /// <summary>
    /// The selected item changed event args.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class SelectedItemChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedItemChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public SelectedItemChangedEventArgs(T item)
        {
            this.Item = item;
        }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public T Item { get; set; }
    }
}