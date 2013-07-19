using System;

namespace Protogame
{
    public class SelectedItemChangedEventArgs : EventArgs
    {
        public TreeItem Item { get; set; }

        public SelectedItemChangedEventArgs(TreeItem item)
        {
            this.Item = item;
        }
    }
}

