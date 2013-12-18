using System;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace ProtogameAssetManager
{
    public class FontAssetEditor : AssetEditor<FontAsset>
    {
        private TextBox m_FontNameTextBox;
        private TextBox m_FontSizeTextBox;
        private TextBox m_UseKerningTextBox;
        private TextBox m_SpacingTextBox;
        private Thread m_CompilationThread;
        private Label m_StatusLabel;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_FontNameTextBox = new TextBox { Text = this.m_Asset.FontName };
            this.m_FontNameTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.FontName = this.m_FontNameTextBox.Text;

                this.StartCompilation(assetManager);
            };
            this.m_FontSizeTextBox = new TextBox { Text = this.m_Asset.FontSize.ToString() };
            this.m_FontSizeTextBox.TextChanged += (sender, e) =>
            {
                try
                {
                    this.m_Asset.FontSize = Convert.ToInt32(this.m_FontSizeTextBox.Text);

                    this.StartCompilation(assetManager);
                } catch (FormatException) { }
            };
            this.m_UseKerningTextBox = new TextBox { Text = this.m_Asset.UseKerning.ToString() };
            this.m_UseKerningTextBox.TextChanged += (sender, e) =>
            {
                try
                {
                    this.m_Asset.UseKerning = Convert.ToBoolean(this.m_UseKerningTextBox.Text);

                    this.StartCompilation(assetManager);
                } catch (FormatException) { }
            };
            this.m_SpacingTextBox = new TextBox { Text = this.m_Asset.Spacing.ToString() };
            this.m_SpacingTextBox.TextChanged += (sender, e) =>
            {
                try
                {
                    this.m_Asset.Spacing = Convert.ToInt32(this.m_SpacingTextBox.Text);

                    this.StartCompilation(assetManager);
                } catch (FormatException) { }
            };
            
            var form = new Form();
            form.AddControl("Font Name:", this.m_FontNameTextBox);
            form.AddControl("Font Size:", this.m_FontSizeTextBox);
            form.AddControl("Use Kerning (true/false):", this.m_UseKerningTextBox);
            form.AddControl("Spacing:", this.m_SpacingTextBox);

            this.m_StatusLabel = new Label();
            this.m_StatusLabel.Text = "";

            var fontViewer = new FontViewer();
            fontViewer.Font = this.m_Asset;

            var vertContainer = new VerticalContainer();
            vertContainer.AddChild(form, "*");
            vertContainer.AddChild(this.m_StatusLabel, "20");
            vertContainer.AddChild(fontViewer, "200");
            
            editorContainer.SetChild(vertContainer);
        }

        private void StartCompilation(IAssetManager assetManager)
        {
            if (this.m_CompilationThread != null && this.m_CompilationThread.ThreadState != ThreadState.Stopped)
            {
                this.m_CompilationThread.Abort();
                this.m_CompilationThread = null;
            }

            this.m_StatusLabel.Text = "Compiling font...";
            this.m_CompilationThread = new Thread(this.Compile);
            this.m_CompilationThread.Start(assetManager);
        }

        public void Compile(object p)
        {
            var assetManager = (IAssetManager)p;
            assetManager.Recompile(this.m_Asset);
            assetManager.Save(this.m_Asset);

            this.m_StatusLabel.Text = "";
        }

        private Texture2D RetrieveTextureFromFont(SpriteFont font)
        {
            var tex = font.GetType().GetField("_texture", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(font);
            return (Texture2D)tex;
        }

        public override void FinishLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
        }

        public override void Bake(IAssetManager assetManager)
        {
            assetManager.Bake(this.m_Asset);
        }
    }
}

