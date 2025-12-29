//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using milk.Core;

namespace milk.UI;

/// <summary>
/// A button ties an area of the screen with callbacks.
/// </summary>
public class Button : UIElement
{

    /// <summary>
    /// Creates a new button object.
    /// </summary>
    /// <param name="caption">The text to display (default = null - no caption).</param>
    /// <param name="font">The text font (default = null - FontSmall).</param>
    /// <param name="position">The (x, y) button position.</param>
    /// <param name="size">The (w, h) size of the button.</param>
    /// <param name="anchor">The part of the button to link to the position.</param>
    /// <param name="parent">The UIElement parent, for relative positioning.</param>
    /// <param name="foregroundColor">The foreground color.</param>
    /// <param name="backgroundColor">The background color.</param>
    /// <param name="borderColor">The border color.</param>
    /// <param name="selectedForegroundColor">The foreground color when the element is selected (default = null - no change).</param>
    /// <param name="selectedBackgroundColor">The background color when the element is selected (default = null - no change).</param>
    /// <param name="selectedBorderColor">The border color when the element is selected (default = null - no change).</param>
    /// <param name="borderWidth">The width of the border (default = 1).</param>
    /// <param name="onSelected">The button callback when selected.</param>
    /// <param name="customDrawMethod">Optionally specified a custom draw method (default = null).</param>
    /// <param name="alpha">Button's opacity (between 0 and 1, default = 1 - no opacity).</param>
    /// <param name="visible">Button visibility (default = true).</param>
    /// <param name="active">Button in use (default = true).</param>
    public Button(
        string? caption = null,
        SpriteFont? font = null,
        Vector2? position = null,
        Vector2? size = null,
        Anchor anchor = Anchor.TopLeft,
        UIElement? parent = null,
        Color? foregroundColor = null,
        Color? backgroundColor = null,
        Color? borderColor = null,
        Color? selectedForegroundColor = null,
        Color? selectedBackgroundColor = null,
        Color? selectedBorderColor = null,
        int borderWidth = 1,
        Action<UIElement, Scene>? onSelected = null,
        Action<UIElement, bool>? customDrawMethod = null,
        float alpha = 1.0f,
        bool visible = true,
        bool active = true
    ) : base(
            caption: caption ?? "",
            font: font ?? EngineGlobals.game._engineResources.FontSmall,
            position: position ?? Vector2.Zero,
            size: size ?? new Vector2(100, 50),
            anchor: anchor,
            parent: parent,
            foregroundColor: foregroundColor,
            backgroundColor: backgroundColor,
            borderColor: borderColor,
            selectedForegroundColor: selectedForegroundColor,
            selectedBackgroundColor: selectedBackgroundColor,
            selectedBorderColor: selectedBorderColor,
            borderWidth: borderWidth,
            onSelected: onSelected,
            customDrawMethod: customDrawMethod,
            active: active,
            alpha: alpha,
            visible: visible
        )
    {
        
    }

    internal override void Draw(bool selected)
    {

        if (Visible == false)
            return;

        // 
        if (CustomDrawMethod != null)
        {
            CustomDrawMethod.Invoke(this, selected);
            return;
        }

        //
        
        Vector2 pp = this.GetPositionIncludingParent(this.Position);
        Vector2 pos = this.CalculateTopLeftPositionFromAnchor(pp);

        Color bg = selected == true ? SelectedBackgroundColor : BackgroundColor;
        Color fg = selected == true ? SelectedForegroundColor : ForegroundColor;
        Color br = selected == true ? SelectedBorderColor : BorderColor;

        // Background
        EngineGlobals.game.spriteBatch.FillRectangle(
            new Rectangle(
                (int)pos.X,
                (int)pos.Y,
                (int)Size.X,
                (int)Size.Y
            ),
            Active == true ? bg * Alpha : bg * Alpha * 0.5f
        );

        // Text
        Vector2 stringLength = Font.MeasureString(Caption);
        EngineGlobals.game.spriteBatch.DrawString(
            Font,
            Caption, 
            new Vector2(
                (int)(pos.X + Size.X / 2 - stringLength.X / 2),
                (int)(pos.Y + Size.Y / 2 - stringLength.Y / 2)
            )
            ,
            Active == true ? fg * Alpha : fg * Alpha * 0.5f
        );

        // Border
        EngineGlobals.game.spriteBatch.DrawRectangle(
            new Rectangle(
                (int)pos.X - BorderWidth,
                (int)pos.Y - BorderWidth,
                (int)Size.X + (2 * BorderWidth),
                (int)Size.Y + (2 * BorderWidth)
            ),
            Active == true ? br * Alpha : br * Alpha * 0.5f,
            BorderWidth
        );

    }

    // 
    public void DrawBackground() {}
    public void DrawText() {}
    public void DrawBorder() {}

    
}