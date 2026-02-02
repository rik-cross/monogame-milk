//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Core;

internal class SpriteTextureList
{

    internal List<Texture2D> textureList;

    internal Color hue;

    internal Vector2 offset;

    internal bool flipH;

    internal bool flipV;

    internal Vector2 scale;
    
    internal SpriteTextureList(
        List<Texture2D> textureList,
        Vector2? offset = null,
        Vector2? scale = null,
        Color? hue = null,
        bool flipH = false,
        bool flipV = false
    )
    {
        this.textureList = textureList;
        this.offset = offset ?? Vector2.Zero;
        this.scale = scale ?? Vector2.One;
        this.hue = hue ?? Color.White;
        this.flipH = flipH;
        this.flipV = flipV;
    }

}