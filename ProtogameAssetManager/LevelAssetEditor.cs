using Protogame;
using System.IO;
using System.Text;

namespace ProtogameAssetManager
{
    public class LevelAssetEditor : AssetEditor<LevelAsset>
    {
        private FileSelect m_FileSelect;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_FileSelect = new FileSelect { Path = this.m_Asset.SourcePath };
            this.m_FileSelect.Changed += (sender, e) =>
            {
                this.m_Asset.SourcePath = this.m_FileSelect.Path;
                using (var stream = new StreamReader(this.m_Asset.SourcePath, Encoding.UTF8))
                {
                    this.m_Asset.Value = stream.ReadToEnd();
                }
                assetManager.Save(this.m_Asset);
            };
            
            var form = new Form();
            form.AddControl("Source File:", this.m_FileSelect);
            
            var vertContainer = new VerticalContainer();
            vertContainer.AddChild(form, "*");
            
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

