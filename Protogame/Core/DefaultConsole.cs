namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The default console.
    /// </summary>
    public class DefaultConsole : IConsole
    {
        /// <summary>
        /// The m_2 d render utilities.
        /// </summary>
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        /// <summary>
        /// The m_ commands.
        /// </summary>
        private readonly ICommand[] m_Commands;

        /// <summary>
        /// The m_ default font asset.
        /// </summary>
        private FontAsset m_DefaultFontAsset;

        /// <summary>
        /// The m_ keyboard string reader.
        /// </summary>
        private readonly IKeyboardStringReader m_KeyboardStringReader;

        private readonly IAssetManagerProvider m_AssetManagerProvider;

        /// <summary>
        /// The m_ log.
        /// </summary>
        private readonly List<string> m_Log = new List<string>();

        /// <summary>
        /// The m_ input.
        /// </summary>
        private StringBuilder m_Input = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConsole"/> class.
        /// </summary>
        /// <param name="_2DRenderUtilities">
        /// The _2 d render utilities.
        /// </param>
        /// <param name="keyboardStringReader">
        /// The keyboard string reader.
        /// </param>
        /// <param name="assetManagerProvider">
        /// The asset manager provider.
        /// </param>
        /// <param name="commands">
        /// The commands.
        /// </param>
        public DefaultConsole(
            I2DRenderUtilities _2DRenderUtilities, 
            IKeyboardStringReader keyboardStringReader, 
            IAssetManagerProvider assetManagerProvider, 
            ICommand[] commands)
        {
            this.m_2DRenderUtilities = _2DRenderUtilities;
            this.m_KeyboardStringReader = keyboardStringReader;
            this.m_AssetManagerProvider = assetManagerProvider;
            this.m_Commands = commands;
        }

        /// <summary>
        /// Gets a value indicating whether open.
        /// </summary>
        /// <value>
        /// The open.
        /// </value>
        public bool Open { get; private set; }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (this.m_DefaultFontAsset == null)
            {
                this.m_DefaultFontAsset = this.m_AssetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
            }

            if (!this.Open || renderContext.IsCurrentRenderPass<I3DRenderPass>())
            {
                return;
            }

            this.m_2DRenderUtilities.RenderRectangle(
                renderContext, 
                new Rectangle(0, 0, gameContext.Window.ClientBounds.Width, 300), 
                new Color(0, 0, 0, 210), 
                true);
            this.m_2DRenderUtilities.RenderRectangle(
                renderContext, 
                new Rectangle(0, 0, gameContext.Window.ClientBounds.Width - 1, 300), 
                Color.White);
            this.m_2DRenderUtilities.RenderLine(
                renderContext, 
                new Vector2(0, 300 - 16), 
                new Vector2(gameContext.Window.ClientBounds.Width, 300 - 16), 
                Color.White);
            this.m_2DRenderUtilities.RenderText(
                renderContext, 
                new Vector2(2, 300 - 16), 
                this.m_Input.ToString(), 
                this.m_DefaultFontAsset);
            var a = 0;
            for (var i = Math.Max(0, this.m_Log.Count - 30); i < this.m_Log.Count; i++)
            {
                this.m_2DRenderUtilities.RenderText(
                    renderContext, 
                    new Vector2(2, 300 - 32 - a * 16), 
                    this.m_Log[this.m_Log.Count - i - 1], 
                    this.m_DefaultFontAsset);
                a++;
            }

            if (this.m_Log.Count > 31)
            {
                this.m_Log.RemoveRange(0, this.m_Log.Count - 31);
            }
        }

        /// <summary>
        /// The toggle.
        /// </summary>
        public void Toggle()
        {
            this.Open = !this.Open;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="updateContext">
        /// The update context.
        /// </param>
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            var state = Keyboard.GetState();

            if (!this.Open)
            {
                return;
            }

            this.m_KeyboardStringReader.Process(state, gameContext.GameTime, this.m_Input);
            if (this.m_Input.ToString().LastIndexOf('`') != -1)
            {
                this.m_Input.Remove(this.m_Input.ToString().LastIndexOf('`'), 1);
            }

            if (state.IsKeyChanged(this, Keys.Enter) == KeyState.Down)
            {
                this.m_Log.Add("> " + this.m_Input);
                this.Execute(gameContext, this.Parse(this.m_Input.ToString()));
                this.m_Input = new StringBuilder();
            }
        }

        public void Log(string message)
        {
            this.m_Log.Add(message);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="parsedCommand">
        /// The parsed command.
        /// </param>
        private void Execute(IGameContext gameContext, ParsedCommand parsedCommand)
        {
            foreach (var command in this.m_Commands)
            {
                if (command.Names.Contains(parsedCommand.Name))
                {
                    var output = command.Execute(gameContext, parsedCommand.Name, parsedCommand.Parameters.ToArray());
                    var lines = output.Split('\n');
                    int start = -1, end = lines.Length;
                    for (var i = 0; i < lines.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(lines[i]))
                        {
                            start = i;
                            break;
                        }
                    }

                    if (start == -1)
                    {
                        return;
                    }

                    for (var i = lines.Length - 1; i >= start; i--)
                    {
                        if (string.IsNullOrEmpty(lines[i]))
                        {
                            end = i;
                            break;
                        }
                    }

                    for (var i = start; i < end; i++)
                    {
                        this.m_Log.Add(lines[i]);
                    }

                    return;
                }
            }

            this.m_Log.Add("No such command found.");
        }

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="ParsedCommand"/>.
        /// </returns>
        private ParsedCommand Parse(string input)
        {
            var parsed = new ParsedCommand();
            var split = input.Split(new[] { ' ' }, 2);
            parsed.Name = split[0];
            parsed.Parameters = new List<string>();
            if (split.Length > 1)
            {
                var inQuote = false;
                var buffer = string.Empty;
                for (var i = 0; i < split[1].Length; i++)
                {
                    if (split[1][i] == '"')
                    {
                        if (i == 0 || split[1][i - 1] != '\\')
                        {
                            inQuote = !inQuote;
                        }

                        if (!inQuote)
                        {
                            parsed.Parameters.Add(buffer);
                        }
                    }
                    else if (split[1][i] == ' ' && !inQuote)
                    {
                        parsed.Parameters.Add(buffer);
                        buffer = string.Empty;
                    }
                    else
                    {
                        buffer += split[1][i];
                    }
                }

                if (buffer.Length > 0)
                {
                    parsed.Parameters.Add(buffer);
                    buffer = string.Empty;
                }
            }

            return parsed;
        }

        /// <summary>
        /// The parsed command.
        /// </summary>
        private struct ParsedCommand
        {
            /// <summary>
            /// The name.
            /// </summary>
            public string Name;

            /// <summary>
            /// The parameters.
            /// </summary>
            public List<string> Parameters;
        }
    }
}