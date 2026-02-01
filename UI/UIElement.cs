//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using milk.Core;

namespace milk.UI;

public abstract class UIElement : SceneRenderable
{

    public Scene parentScene = null;
    public string Caption;
    public SpriteFont Font;
    public Color ForegroundColor;
    public Color BackgroundColor;
    public Color BorderColor;
    public Color SelectedForegroundColor;
    public Color SelectedBackgroundColor;
    public Color SelectedBorderColor;
    public int BorderWidth;

    
    private UIElement? _elementAbove = null;
    private UIElement? _elementBelow = null;
    private UIElement? _elementLeft = null;
    private UIElement? _elementRight = null;

    public UIElement? ElementAbove {
        get
        {
            return _elementAbove;
        }
        set
        {
            _elementAbove = value;
            if (value != null) value._elementBelow = this;
        }
    }
    public UIElement? ElementBelow {
        get
        {
            return _elementBelow;
        }
        set
        {
            _elementBelow = value;
            if (value != null) value._elementAbove = this;
        }
    }
    public UIElement? ElementLeft {
        get
        {
            return _elementLeft;
        }
        set
        {
            _elementLeft = value;
            if (value != null) value._elementRight = this;
        }
    }
    public UIElement? ElementRight {
        get
        {
            return _elementRight;
        }
        set
        {
            _elementRight = value;
            if (value != null) value._elementLeft = this;
        }
    }

    public Action<UIElement, Scene>? OnSelected;

    protected Action<UIElement, bool>? CustomDrawMethod;
    public bool Active;
    public bool Enabled;

    protected UIElement(
        string caption,
        SpriteFont font,
        Vector2 position,
        Vector2 size,
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
        bool active = true,
        float alpha = 1.0f,
        bool visible = true
    ) : base (
        size,
        position,
        alpha,
        anchor,
        parent,
        visible
    )
    {
        Caption = caption;
        Font = font;

        ForegroundColor = foregroundColor ?? Color.Black;
        BackgroundColor = backgroundColor ?? Color.White;
        BorderColor = borderColor ?? Color.Black;

        SelectedForegroundColor = selectedForegroundColor ?? ForegroundColor;
        SelectedBackgroundColor = selectedBackgroundColor ?? BackgroundColor;
        SelectedBorderColor = selectedBorderColor ?? BorderColor;

        ElementAbove = null;
        ElementBelow = null;
        ElementLeft = null;
        ElementRight = null;

        BorderWidth = borderWidth;
        OnSelected = onSelected;
        CustomDrawMethod = customDrawMethod;
        Active = active;
    }

    public virtual void Update(GameTime gameTime, bool selected) {}
    public virtual void Input(bool selected) {}
    internal virtual void Draw(bool selected) {}

}