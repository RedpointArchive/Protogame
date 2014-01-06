using Protogame;
using System.IO;

namespace ProtogameAssetManager
{
    internal class ModelAssetEditor : AssetEditor<ModelAsset>
    {
        private FileSelect m_FileSelect;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_FileSelect = new FileSelect();
            this.m_FileSelect.Changed += (sender, e) =>
            {
                using (var stream = new FileStream(this.m_FileSelect.Path, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        this.m_Asset.SourceData = reader.ReadBytes((int)stream.Length);
                    }
                }
                assetManager.Recompile(this.m_Asset);
                assetManager.Save(this.m_Asset);
            };
            
            var form = new Form();
            form.AddControl("Source File:", this.m_FileSelect);
            
            //var textureViewer = new TextureViewer();
            //textureViewer.Texture = this.m_Asset;
            
            var vertContainer = new VerticalContainer();
            vertContainer.AddChild(form, "*");
            //vertContainer.AddChild(textureViewer, "400");
            
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

