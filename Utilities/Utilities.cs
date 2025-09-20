using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameECS;

public static class Utilities
{
    public static List<List<Texture2D>> SplitTexture(
        Texture2D originalTexture,
        Vector2 subTextureSize,
        Rectangle subTextureClippingRect = default
    )
    {
        List<List<Texture2D>> subTextureList = new List<List<Texture2D>>();

        for (int row = 0; row < originalTexture.Height; row += (int)subTextureSize.Y)
        {
            List<Texture2D> textureRow = new List<Texture2D>();
            for (int col = 0; col < originalTexture.Width; col += (int)subTextureSize.X)
            {
                int x = col;
                int y = row;
                int w = (int)subTextureSize.X;
                int h = (int)subTextureSize.Y;

                if (subTextureClippingRect != default)
                {
                    x += subTextureClippingRect.X;
                    y += subTextureClippingRect.Y;
                    w = subTextureClippingRect.Width;
                    h = subTextureClippingRect.Height;
                }
                //S.WriteLine(x + " " + y + " " + w + " " + h);
                Texture2D t = GetSubTexture(originalTexture, x, y, w, h);
                textureRow.Add(t);
            }
            subTextureList.Add(textureRow);
        }
        return subTextureList;
    }

    public static Texture2D GetSubTexture(Texture2D texture, int x, int y, int width, int height)
    {

        // Create the new sub texture
        Rectangle rect = new Rectangle(x, y, width, height);
        Texture2D subTexture = new Texture2D(EngineGlobals.game.graphicsDevice, rect.Width, rect.Height);

        // Set the texture data
        Color[] data = new Color[rect.Width * rect.Height];
        texture.GetData(0, rect, data, 0, data.Length);
        subTexture.SetData(data);

        return subTexture;

    }
    

}