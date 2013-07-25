using System;
using Protogame;

namespace ProtogameAssetManager
{
    public class FontAssetEditor : AssetEditor<FontAsset>
    {
        private TextBox m_FontNameTextBox;
        private TextBox m_FontSizeTextBox;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_FontNameTextBox = new TextBox { Text = this.m_Asset.FontName };
            this.m_FontNameTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.FontName = this.m_FontNameTextBox.Text;
                this.m_Asset.RebuildFont();
                assetManager.Save(this.m_Asset);
            };
            this.m_FontSizeTextBox = new TextBox { Text = this.m_Asset.FontSize.ToString() };
            this.m_FontSizeTextBox.TextChanged += (sender, e) =>
            {
                try
                {
                    this.m_Asset.FontSize = Convert.ToInt32(this.m_FontSizeTextBox.Text);
                    this.m_Asset.RebuildFont();
                    assetManager.Save(this.m_Asset);
                } catch (FormatException) { }
            };
            
            var form = new Form();
            form.AddControl("Font Name:", this.m_FontNameTextBox);
            form.AddControl("Font Size:", this.m_FontSizeTextBox);
            
            var fontViewer = new FontViewer();
            fontViewer.Font = this.m_Asset;
            
            var vertContainer = new VerticalContainer();
            vertContainer.AddChild(form, "*");
            vertContainer.AddChild(fontViewer, "200");
            
            editorContainer.SetChild(vertContainer);
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

