using System;
using System.Text;
using Microsoft.Xna.Framework;

#if PLATFORM_ANDROID
using Android.Content;
using Android.Views.InputMethods;
#endif

namespace Protogame
{
    public class TextBox : IContainer
    {
        private readonly DefaultKeyboardStringReader _keyboardReader = new DefaultKeyboardStringReader();
        
        private string _previousValue = string.Empty;
        
        private StringBuilder _textBuilder = new StringBuilder();
        
        public event EventHandler TextChanged;

        public IContainer[] Children => IContainerConstant.EmptyContainers;
        
        public bool Focused { get; set; }
        
        public string Hint { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public string Text
        {
            get
            {
                return _textBuilder.ToString();
            }
            set
            {
                var oldValue = _textBuilder.ToString();
                _textBuilder = new StringBuilder(value);
                _previousValue = value;
                if (oldValue != value)
                {
                    TextChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        
        public int UpdateCounter { get; set; }
        
        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
        }
        
        public void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            UpdateCounter++;
        }
        
        public bool HandleEvent(ISkinLayout skin, Rectangle layout, IGameContext context, Event @event)
        {
            var mousePressEvent = @event as MousePressEvent;
            var touchPressEvent = @event as TouchPressEvent;

            if (mousePressEvent != null && mousePressEvent.Button == MouseButton.Left)
            {
                if (layout.Contains(mousePressEvent.X, mousePressEvent.Y))
                {
                    this.Focus();

                    return true;
                }
            }

            if (touchPressEvent != null)
            {
                if (layout.Contains((int)touchPressEvent.X, (int)touchPressEvent.Y))
                {
                    this.Focus();

#if PLATFORM_ANDROID
                    var manager = (InputMethodManager)Game.Activity.GetSystemService(Context.InputMethodService);
                    manager.ShowSoftInput(((Microsoft.Xna.Framework.AndroidGameWindow)context.Game.Window).GameViewAsView, ShowFlags.Forced);
#endif

                    return true;
                }
                else
                {
#if PLATFORM_ANDROID
                    var manager = (InputMethodManager)Game.Activity.GetSystemService(Context.InputMethodService);
                    manager.HideSoftInputFromWindow(((Microsoft.Xna.Framework.AndroidGameWindow)context.Game.Window).GameViewAsView.WindowToken, HideSoftInputFlags.None);
#endif
                }
            }

            var keyEvent = @event as KeyboardEvent;

            if (keyEvent != null)
            {
                if (Focused)
                {
                    _keyboardReader.Process(keyEvent.PressedKeys, context.GameTime, _textBuilder);
                    if (_textBuilder.ToString() != _previousValue)
                    {
                        TextChanged?.Invoke(this, new EventArgs());
                    }

                    _previousValue = _textBuilder.ToString();

                    return true;
                }
            }

            return false;
        }
    }
}