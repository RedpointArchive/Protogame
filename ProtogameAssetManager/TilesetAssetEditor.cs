using Protogame;
using System.IO;
using System;

namespace ProtogameAssetManager
{
    public class TilesetAssetEditor : AssetEditor<TilesetAsset>
    {
        private TextBox m_TextureAssetName;
        private TextBox m_CellWidth;
        private TextBox m_CellHeight;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_TextureAssetName = new TextBox { Text = this.m_TextureAssetName == null ? null : this.m_TextureAssetName.Text };
            this.m_TextureAssetName.TextChanged += (sender, e) =>
            {
                if (assetManager.TryGet<TextureAsset>(this.m_TextureAssetName.Text) != null)
                {
                    this.m_Asset.TextureName = this.m_TextureAssetName.Text;
                    assetManager.Save(this.m_Asset);
                }
            };
            this.m_CellWidth = new TextBox { Text = this.m_CellWidth == null ? null : this.m_CellWidth.Text };
            this.m_CellWidth.TextChanged += (sender, e) =>
            {
                try
                {
                    this.m_Asset.CellWidth = Convert.ToInt32(this.m_CellWidth);
                    assetManager.Save(this.m_Asset);
                }
                catch { }
            };
            this.m_CellHeight = new TextBox { Text = this.m_CellHeight == null ? null : this.m_CellHeight.Text };
            this.m_CellHeight.TextChanged += (sender, e) =>
            {
                try
                {
                    this.m_Asset.CellHeight = Convert.ToInt32(this.m_CellHeight);
                    assetManager.Save(this.m_Asset);
                }
                catch { }
            };
            
            var form = new Form();
            form.AddControl("Texture Name:", this.m_TextureAssetName);
            form.AddControl("Cell Width:", this.m_CellWidth);
            form.AddControl("Cell Height:", this.m_CellHeight);
            
            editorContainer.SetChild(form);
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

