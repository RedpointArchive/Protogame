using Protogame;
using System.IO;

namespace ProtogameAssetManager
{
    public class TextureAssetEditor : AssetEditor<TextureAsset>
    {
        private FileSelect m_FileSelect;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_FileSelect = new FileSelect { Path = this.m_Asset.SourcePath };
            this.m_FileSelect.Changed += (sender, e) =>
            {
                this.m_Asset.SourcePath = this.m_FileSelect.Path;
                using (var stream = new FileStream(this.m_Asset.SourcePath, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        this.m_Asset.Data = reader.ReadBytes((int)stream.Length);
                    }
                }
                this.m_Asset.RebuildTexture();
                assetManager.Save(this.m_Asset);
            };
            
            var form = new Form();
            form.AddControl("Source File:", this.m_FileSelect);
            
            var textureViewer = new TextureViewer();
            textureViewer.Texture = this.m_Asset;
            
            var vertContainer = new VerticalContainer();
            vertContainer.AddChild(form, "*");
            vertContainer.AddChild(textureViewer, "400");
            
            editorContainer.SetChild(vertContainer);
        }

        public override void FinishLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
        }

        public override void Bake(IAssetManager assetManager)
        {
            assetManager.Save(this.m_Asset);
            assetManager.Bake(this.m_Asset);
        }
    }
}

