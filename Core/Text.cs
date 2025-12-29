//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Core;

/// <summary>
/// Displays text in a scene, in the font specified.
/// </summary>
public class Text : SceneRenderable
{
    private SpriteBatch _spriteBatch = EngineGlobals.game.spriteBatch;
    private SpriteFont _font;

    /// <summary>
    /// The font to use for the text.
    /// </summary>
    public SpriteFont Font
    {
        get { return _font; }
        set
        {
            _font = value;
            GenerateTextWidth(_caption, _font, _outlineWidth);
        }
    }

    // Caption
    private string _caption = "";

    /// <summary>
    /// The text to display.
    /// </summary>
    public string Caption
    {
        get { return _caption; }
        set
        {
            _caption = value;
            GenerateTextWidth(_caption, _font, _outlineWidth);
        }
    }

    /// <summary>
    /// Text color.
    /// </summary>
    public Color Color { get; set; }

    private int _outlineWidth = 0;
    /// <summary>
    /// Text outline width.
    /// </summary>
    public int OutlineWidth
    {
        get { return _outlineWidth; }
        set
        {
            _outlineWidth = value;
            GenerateTextWidth(_caption, _font, _outlineWidth);
        }
    }

    /// <summary>
    /// Outline color.
    /// </summary>
    public Color OutlineColor { get; set; }

    /// <summary>
    /// Create a new next object.
    /// </summary>
    /// <param name="caption">The text to display.</param>
    /// <param name="font">The font to use to render the text (default = null - FontSmall).</param>
    /// <param name="position">The (x, y) text position (default = (0,0)).</param>
    /// <param name="alpha">The amount of transparency, between 0 and 1 (default = 1 - fully visible).</param>
    /// <param name="color">The text color.</param>
    /// <param name="outlineWidth">The width of the text outline.</param>
    /// <param name="outlineColor">The color of the outline.</param>
    /// <param name="anchor">The anchor point to set the position against (default = top-left).</param>
    /// <param name="parent">The parent SceneRenderable, for setting relative position (default = null).</param>
    public Text(
        string caption,
        SpriteFont? font = null,
        Vector2? position = null,
        float alpha = 1.0f,
        Color? color = null,
        int outlineWidth = 0,
        Color? outlineColor = null,
        Anchor anchor = Anchor.TopLeft,
        SceneRenderable? parent = null
        ) : base(GenerateTextWidth(caption, font == null ? EngineGlobals.game._engineResources.FontSmall : font, outlineWidth), position ?? Vector2.Zero, alpha, anchor, parent)
    {

        Font = font ?? EngineGlobals.game._engineResources.FontSmall;
        Color = color ?? Color.White;
        OutlineWidth = outlineWidth;
        OutlineColor = outlineColor ?? Color.Black;
        Caption = caption;

    }

    public static Vector2 GenerateTextWidth(string s, SpriteFont sf, int ow)
    {
        return new Vector2(
                sf.MeasureString(s).X + 2 * ow,
                sf.MeasureString(s).Y + 2 * ow
            );
    }

    public override void Draw()
    {

        Vector2 pip = GetPositionIncludingParent(Position);
        Vector2 dp = CalculateTopLeftPositionFromAnchor(pip);

        // Draw the outline
        if (OutlineWidth > 0)
        {
            for (int i = 1; i < OutlineWidth - 1; i++)
            {
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X - i, dp.Y - i), OutlineColor * Alpha);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X + i, dp.Y - i), OutlineColor * Alpha);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X - i, dp.Y + i), OutlineColor * Alpha);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X + i, dp.Y + i), OutlineColor * Alpha);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X - i, dp.Y), OutlineColor * Alpha);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X + i, dp.Y), OutlineColor * Alpha);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X, dp.Y - i), OutlineColor * Alpha);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X, dp.Y + i), OutlineColor * Alpha);
            }
        }

        _spriteBatch.DrawString(Font, Caption, dp, Color * Alpha);

    }

}