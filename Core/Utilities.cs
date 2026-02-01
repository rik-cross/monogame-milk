//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using System.Collections;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Core;

/// <summary>
/// A suite of useful additional utilities.
/// </summary>
public static class Utilities
{

    /// <summary>
    /// Splits an original source texture into a 2D list of sub-textures,
    /// using the size and clipping rectangle provided.
    /// </summary>
    /// <param name="originalTexture">The texture to split.</param>
    /// <param name="subTextureSize">The (w, h) size to split the original texture into.</param>
    /// <param name="subTextureClippingRect">Specifies a sub-region of the sub-textures to split.</param>
    /// <returns>A 2D list of textures, mapped to the position of the sub-textures in the original texture.</returns>
    public static List<List<Texture2D>> SplitTexture(
        Texture2D originalTexture,
        Vector2 subTextureSize,
        Rectangle subTextureClippingRect = default
    )
    {
        // Creates a 2D list of sub-textures to return
        List<List<Texture2D>> subTextureList = new List<List<Texture2D>>();

        // Iterate over each row
        for (int row = 0; row < originalTexture.Height; row += (int)subTextureSize.Y)
        {
            // Create a new list per column
            List<Texture2D> textureRow = new List<Texture2D>();
            for (int col = 0; col < originalTexture.Width; col += (int)subTextureSize.X)
            {
                int x = col;
                int y = row;
                int w = (int)subTextureSize.X;
                int h = (int)subTextureSize.Y;

                // If a sub-texture clipping rect is specified, 
                // then further crop each sub-texture
                if (subTextureClippingRect != default)
                {
                    x += subTextureClippingRect.X;
                    y += subTextureClippingRect.Y;
                    w = subTextureClippingRect.Width;
                    h = subTextureClippingRect.Height;
                }

                // Get the smaller sub-texture, and add to the current row in the 2D list
                Texture2D t = GetSubTexture(originalTexture, x, y, w, h);
                textureRow.Add(t);
            }
            // Add the completed row to the 2D list
            subTextureList.Add(textureRow);
        }
        return subTextureList;
    }

    /// <summary>
    /// Returns a sub-region of the specified texture.
    /// </summary>
    /// <param name="texture">The texture to crop.</param>
    /// <param name="x">The starting x position.</param>
    /// <param name="y">The starting y position.</param>
    /// <param name="width">The width to crop to.</param>
    /// <param name="height">The height to crop to.</param>
    /// <returns>A cropped texture.</returns>
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

    // Returns the current working directory
    internal static string GetCurrentSourceDirectory([CallerFilePath] string callerFilePath = "")
    {
        return Path.GetDirectoryName(callerFilePath);
    }

    // Returns true if 2 BitArrays are identical
    internal static bool CompareBitArrays(BitArray ba1, BitArray ba2)
    {
        if (ba1.Count != ba2.Count)
            return false;
        
        for (int i = 0; i < ba1.Count; i++)
        {
            if (ba1.Get(i) != ba2.Get(i))
                return false;
        }
        return true;
    }
    
}