using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protogame
{
    public interface IBuiltinComponentFactory
    {
        Render2DTextComponent CreateRender2DTextComponent();

        Render2DTextureComponent CreateRender2DTextureComponent();
    }
}
