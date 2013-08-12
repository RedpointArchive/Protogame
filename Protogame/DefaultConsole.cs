using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class DefaultConsole : IConsole
    {
        private I2DRenderUtilities m_2DRenderUtilities;
        private IKeyboardStringReader m_KeyboardStringReader;
        private ICommand[] m_Commands;
        private FontAsset m_DefaultFontAsset;
    
        private List<string> m_Log = new List<string>();
        private StringBuilder m_Input = new StringBuilder();
        
        public bool Open { get; private set; }
    
        public DefaultConsole(
            I2DRenderUtilities _2DRenderUtilities,
            IKeyboardStringReader keyboardStringReader,
            IAssetManagerProvider assetManagerProvider,
            ICommand[] commands)
        {
            this.m_2DRenderUtilities = _2DRenderUtilities;
            this.m_KeyboardStringReader = keyboardStringReader;
            this.m_Commands = commands;
            this.m_DefaultFontAsset = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
        }
    
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            var state = Keyboard.GetState();
#if DEBUG
            if (state.IsKeyPressed(Keys.OemTilde))
                this.Open = !this.Open;
#endif
            
            if (!this.Open)
                return;
                
            this.m_KeyboardStringReader.Process(state, gameContext.GameTime, this.m_Input);
            if (this.m_Input.ToString().LastIndexOf('`') != -1)
                this.m_Input.Remove(this.m_Input.ToString().LastIndexOf('`'), 1);
            
            if (state.IsKeyPressed(Keys.Enter))
            {
                this.m_Log.Add("> " + this.m_Input);
                this.Execute(gameContext, this.Parse(this.m_Input.ToString()));
                this.m_Input = new StringBuilder();
            }
        }
        
        private struct ParsedCommand
        {
            public string Name;
            public List<string> Parameters;
        }
        
        private ParsedCommand Parse(string input)
        {
            var parsed = new ParsedCommand();
            var split = input.Split(new[] { ' ' }, 2);
            parsed.Name = split[0];
            parsed.Parameters = new List<string>();
            if (split.Length > 1)
            {
                var inQuote = false;
                var buffer = "";
                for (var i = 0; i < split[1].Length; i++)
                {
                    if (split[1][i] == '"')
                    {
                        if (i == 0 || split[1][i - 1] != '\\')
                            inQuote = !inQuote;
                        if (!inQuote)
                            parsed.Parameters.Add(buffer);
                    }
                    else if (split[1][i] == ' ' && !inQuote)
                    {
                        parsed.Parameters.Add(buffer);
                        buffer = "";
                    }
                    else
                        buffer += split[1][i];
                }
                if (buffer.Length > 0)
                {
                    parsed.Parameters.Add(buffer);
                    buffer = "";
                }
            }
            return parsed;
        }
        
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
                        return;
                    for (var i = lines.Length - 1; i >= start; i--)
                    {
                        if (string.IsNullOrEmpty(lines[i]))
                        {
                            end = i;
                            break;
                        }
                    }
                    for (var i = start; i < end; i++)
                        this.m_Log.Add(lines[i]);
                    return;
                }
            }
            this.m_Log.Add("No such command found.");
        }
        
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!this.Open || renderContext.Is3DContext)
                return;
            
            this.m_2DRenderUtilities.RenderRectangle(
                renderContext,
                new Rectangle(
                    0,
                    0,
                    gameContext.Window.ClientBounds.Width,
                    300),
                new Color(0, 0, 0, 210),
                filled: true);
            this.m_2DRenderUtilities.RenderRectangle(
                renderContext,
                new Rectangle(
                    0,
                    0,
                    gameContext.Window.ClientBounds.Width - 1,
                    300),
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
            for (var i = 0; i < this.m_Log.Count; i++)
            {
                this.m_2DRenderUtilities.RenderText(
                    renderContext,
                    new Vector2(2, 300 - 32 - i * 16),
                    this.m_Log[this.m_Log.Count - i - 1],
                    this.m_DefaultFontAsset);
            }
        }
    }
}

