//   [ ] Finished
//   
//   Monogame ECS Engine
//   By Rik Cross
//   -- github.com/rik-cross/monogame-ecs
//   Shared under the MIT licence
//
//   ------------------------------------
//
//   MonogameECS.Text
//   ================
//  
//   Text can be rendered onto a Scene.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk;

public class Text : SceneRenderable
{
    private SpriteBatch _spriteBatch = EngineGlobals.game.spriteBatch;
    private SpriteFont _font;
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
    public string Caption
    {
        get { return _caption; }
        set
        {
            _caption = value;
            GenerateTextWidth(_caption, _font, _outlineWidth);
        }
    }

    // Colour
    public Color Color { get; set; }

    // Outline
    private int _outlineWidth = 0;
    public int OutlineWidth
    {
        get { return _outlineWidth; }
        set
        {
            _outlineWidth = value;
            GenerateTextWidth(_caption, _font, _outlineWidth);
        }
    }
    public Color OutlineColor { get; set; }

    public Text(
        string caption,
        SpriteFont font,
        Vector2 position,
        float alpha = 1.0f,
        Color? color = null,
        int outlineWidth = 0,
        Color? outlineColor = null,
        Anchor anchor = Anchor.TopLeft,
        SceneRenderable parent = null
        ) : base(GenerateTextWidth(caption, font, outlineWidth), position, alpha, anchor, parent)
    {

        Font = font;
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
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X - i, dp.Y - i), OutlineColor * 1);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X + i, dp.Y - i), OutlineColor * 1);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X - i, dp.Y + i), OutlineColor * 1);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X + i, dp.Y + i), OutlineColor * 1);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X - i, dp.Y), OutlineColor * 1);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X + i, dp.Y), OutlineColor * 1);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X, dp.Y - i), OutlineColor * 1);
                _spriteBatch.DrawString(Font, Caption, new Vector2(dp.X, dp.Y + i), OutlineColor * 1);
            }
        }

        _spriteBatch.DrawString(Font, Caption, dp, Color.White); // Color);

    }

}