using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protoinject;

namespace Protogame
{
    public interface IBuiltinComponentFactory : IGenerateFactory
    {
        Render2DTextComponent CreateRender2DTextComponent();

        Render2DTextureComponent CreateRender2DTextureComponent();
    }
}
