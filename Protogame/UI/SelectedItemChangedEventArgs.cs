using System;

namespace Protogame
{
    public class SelectedItemChangedEventArgs<T> : EventArgs
    {
        public T Item { get; set; }

        public SelectedItemChangedEventArgs(T item)
        {
            this.Item = item;
        }
    }
}

