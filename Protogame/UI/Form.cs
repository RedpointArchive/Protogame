namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The form.
    /// </summary>
    public class Form : IContainer
    {
        /// <summary>
        /// The m_ controls.
        /// </summary>
        private readonly Dictionary<IContainer, IContainer> m_Controls = new Dictionary<IContainer, IContainer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class.
        /// </summary>
        public Form()
        {
            this.LabelMaxWidth = 200;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public IContainer[] Children
        {
            get
            {
                return this.m_Controls.Select(x => x.Value).ToArray();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether focused.
        /// </summary>
        /// <value>
        /// The focused.
        /// </value>
        public bool Focused { get; set; }

        /// <summary>
        /// Gets or sets the label max width.
        /// </summary>
        /// <value>
        /// The label max width.
        /// </value>
        public int LabelMaxWidth { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public IContainer Parent { get; set; }

        /// <summary>
        /// The add control.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="control">
        /// The control.
        /// </param>
        public void AddControl(string name, IContainer control)
        {
            this.m_Controls.Add(new Label { Text = name, Parent = this }, control);
            control.Parent = this;
        }

        /// <summary>
        /// The add control.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="control">
        /// The control.
        /// </param>
        public void AddControl(IContainer name, IContainer control)
        {
            this.m_Controls.Add(name, control);
            control.Parent = this;
        }

        /// <summary>
        /// The children with layouts.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<KeyValuePair<IContainer, Rectangle>> ChildrenWithLayouts(Rectangle layout)
        {
            var i = 0;
            foreach (var kv in this.m_Controls)
            {
                yield return
                    new KeyValuePair<IContainer, Rectangle>(
                        kv.Value, 
                        new Rectangle(
                            layout.X + Math.Min(layout.Width / 2, this.LabelMaxWidth), 
                            layout.Y + i * 20, 
                            layout.Width - Math.Min(layout.Width / 2, this.LabelMaxWidth), 
                            20));
                i++;
            }
        }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawForm(context, layout, this);
            foreach (var kv in this.LabelsWithLayouts(layout))
            {
                kv.Key.Draw(context, skin, kv.Value);
            }

            foreach (var kv in this.ChildrenWithLayouts(layout))
            {
                kv.Key.Draw(context, skin, kv.Value);
            }
        }

        /// <summary>
        /// The labels with layouts.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<KeyValuePair<IContainer, Rectangle>> LabelsWithLayouts(Rectangle layout)
        {
            var i = 0;
            foreach (var kv in this.m_Controls)
            {
                yield return
                    new KeyValuePair<IContainer, Rectangle>(
                        kv.Key, 
                        new Rectangle(layout.X, layout.Y + i * 20, Math.Min(layout.Width / 2, this.LabelMaxWidth), 20));
                i++;
            }
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        /// <param name="stealFocus">
        /// The steal focus.
        /// </param>
        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }

        /// <summary>
        /// Requests that the UI container handle the specified event or return false.
        /// </summary>
        /// <param name="skin">
        /// The UI skin.
        /// </param>
        /// <param name="layout">
        /// The layout for the element.
        /// </param>
        /// <param name="context">
        /// The current game context.
        /// </param>
        /// <param name="event">
        /// The event that was raised.
        /// </param>
        /// <returns>
        /// Whether or not this UI element handled the event.
        /// </returns>
        public bool HandleEvent(ISkin skin, Rectangle layout, IGameContext context, Event @event)
        {
            var mousePressEvent = @event as MousePressEvent;

            if (mousePressEvent != null && mousePressEvent.Button == MouseButton.Left)
            {
                this.Focus();
            }

            foreach (var kv in this.LabelsWithLayouts(layout))
            {
                if (kv.Key.HandleEvent(skin, kv.Value, context, @event))
                {
                    return true;
                }
            }

            foreach (var kv in this.ChildrenWithLayouts(layout))
            {
                if (kv.Key.HandleEvent(skin, kv.Value, context, @event))
                {
                    return true;
                }
            }

            return false;
        }
    }
}