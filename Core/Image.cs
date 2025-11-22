using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace milk.Core;

public class Image : SceneRenderable
{
    private SpriteBatch _spriteBatch = EngineGlobals.game.spriteBatch;
    public readonly Texture2D Texture;
    public Color BackgroundColor;
    public Color BorderColor;
    public int BorderWidth;
    public Color Hue;

    public Image(
        Texture2D texture,
        Vector2 size = default,
        Vector2 position = default,
        float alpha = 1,
        Anchor anchor = Anchor.TopLeft,
        SceneRenderable? parent = null,
        Color backgroundColor = default,
        int borderWidth = 0,
        Color borderColor = default,
        Color hue = default
    ) : base(CalculateSize(texture, size), position, alpha, anchor, parent)
    {
        Texture = texture;

        BackgroundColor = backgroundColor;

        // utils.setcolor?
        if (borderColor == default)
            BorderColor = Color.Black;
        else
            BorderColor = borderColor;

        BorderWidth = borderWidth;

        if (hue == default)
            Hue = Color.White;
        else
            Hue = hue;

    }
    public override void Draw()
    {

        Vector2 drawSize = CalculateSize(Texture, Size);

        Vector2 pip = GetPositionIncludingParent(Position);
        Vector2 dp = CalculateTopLeftPositionFromAnchor(pip);

        // Draw background
        _spriteBatch.DrawRectangle(new RectangleF(dp.X, dp.Y, drawSize.X, drawSize.Y), BackgroundColor, Math.Max(Size.X / 2, Size.Y / 2));

        // Draw the texture, at the correct size
        // TODO: alpha
        if (Texture != null)
            _spriteBatch.Draw(Texture, new Rectangle((int)dp.X, (int)dp.Y, (int)drawSize.X, (int)drawSize.Y), Hue);

        // Draw the border
        _spriteBatch.DrawRectangle(new RectangleF(dp.X, dp.Y, drawSize.X, drawSize.Y), BorderColor, BorderWidth);

    }

    public static Vector2 CalculateSize(Texture2D texture, Vector2 size)
    {
        if (size == default || size == Vector2.Zero)
            return new Vector2(texture.Width, texture.Height);
        else
            return size;
    }
}