//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace milk.Core;

/// <summary>
/// An image can be displayed within a scene.
/// </summary>
public class Image : SceneRenderable
{
    
    /// <summary>
    /// The texture to display.
    /// </summary>
    public readonly Texture2D Texture;

    /// <summary>
    /// Background colour.
    /// </summary>
    public Color BackgroundColor;

    /// <summary>
    /// Border color.
    /// </summary>
    public Color BorderColor;

    /// <summary>
    /// Border width.
    /// </summary>
    public int BorderWidth;

    /// <summary>
    /// Hue for recoloring image.
    /// </summary>
    public Color Hue;

    private SpriteBatch _spriteBatch = EngineGlobals.game.spriteBatch;

    /// <summary>
    /// Creates a new image.
    /// </summary>
    /// <param name="texture">The texture to display.</param>
    /// <param name="size">The (x, y) size of the image (default = null - the texture original size).</param>
    /// <param name="position">The (x, y) position (default = null - (0, 0)).</param>
    /// <param name="alpha">The image transparency (default = 1 - no transparency).</param>
    /// <param name="anchor">The position anchor (default = ).</param>
    /// <param name="parent">The parent SceneRenderable, for relative positioning (default = null).</param>
    /// <param name="backgroundColor">The background color (default = null - transparent).</param>
    /// <param name="borderWidth">The width of the border (default = 0).</param>
    /// <param name="borderColor">The border color (default = null - black).</param>
    /// <param name="hue">The hue, for image recoloring (default = null - white / no recoloring).</param>
    public Image(
        Texture2D texture,
        Vector2 size = default,
        Vector2 position = default,
        float alpha = 1,
        Anchor anchor = Anchor.TopLeft,
        SceneRenderable? parent = null,
        Color? backgroundColor = null,
        int borderWidth = 0,
        Color? borderColor = null,
        Color? hue = null
    ) : base(CalculateSize(texture, size), position, alpha, anchor, parent)
    {
        Texture = texture;
        BackgroundColor = backgroundColor ?? Color.Transparent;
        BorderColor = borderColor ?? Color.Black;
        BorderWidth = borderWidth;
        Hue = hue ?? Color.White;
    }

    /// <summary>
    /// Draw needs to be called on an image within a scene.
    /// </summary>
    public override void Draw()
    {

        Vector2 drawSize = CalculateSize(Texture, Size);

        Vector2 pip = GetPositionIncludingParent(Position);
        Vector2 dp = CalculateTopLeftPositionFromAnchor(pip);

        // Draw background
        _spriteBatch.DrawRectangle(new RectangleF(dp.X, dp.Y, drawSize.X, drawSize.Y), BackgroundColor, Math.Max(Size.X / 2, Size.Y / 2));

        // Draw the texture, at the correct size
        if (Texture != null)
            _spriteBatch.Draw(Texture, new Rectangle((int)dp.X, (int)dp.Y, (int)drawSize.X, (int)drawSize.Y), Hue * Alpha);

        // Draw the border
        _spriteBatch.DrawRectangle(new RectangleF(dp.X, dp.Y, drawSize.X, drawSize.Y), BorderColor, BorderWidth);

    }

    private static Vector2 CalculateSize(Texture2D texture, Vector2 size)
    {
        if (size == default || size == Vector2.Zero)
            return new Vector2(texture.Width, texture.Height);
        else
            return size;
    }
    
}