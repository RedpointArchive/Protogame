using System;

namespace Protogame
{
    public abstract class BaseContainer
    {
        protected IContainer Child { get; private set; }

        public IContainer[] Children
        {
            get
            {
                return new[] { this.Child };
            }
        }

        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public bool Focused { get; set; }

        public void SetChild(IContainer child)
        {
            if (child == null)
                throw new ArgumentNullException("child");
            if (child.Parent != null)
                throw new InvalidOperationException();
            this.Child = child;
            if (this is IContainer)
                this.Child.Parent = this as IContainer;
        }
    }
}

