using Protogame;

namespace ProtogameAssetManager
{
    public class LanguageAssetEditor : AssetEditor<LanguageAsset>
    {
        private TextBox m_TextBox;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_TextBox = new TextBox { Text = this.m_Asset.Value };
            this.m_TextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.Value = this.m_TextBox.Text;
                assetManager.Save(this.m_Asset);
            };
            
            var form = new Form();
            form.AddControl("English:", this.m_TextBox);
            editorContainer.SetChild(form);
        }

        public override void FinishLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
        }

        public override void Bake()
        {
        }
    }
}

