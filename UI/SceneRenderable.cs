//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using milk.Core;

namespace milk.UI;

public abstract class SceneRenderable
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set;}
    public float Alpha;
    public Anchor Anchor { get; set; }
    protected readonly SceneRenderable? Parent;
    public bool Visible { get; set; }

    internal SceneRenderable(
        Vector2 size,
        Vector2? position = null,
        float alpha = 1.0f,
        Anchor anchor = Anchor.TopLeft,
        SceneRenderable? parent = null,
        bool visible = true
    )
    {

        Size = size;
        Position = position ?? Vector2.Zero;
        Alpha = alpha;
        Anchor = anchor;
        Parent = parent;
        Visible = visible;

    }

    public Vector2 CalculateTopLeftPositionFromAnchor(Vector2 position)
    {
        float newX = position.X;
        float newY = position.Y;

        if (Anchor == Anchor.TopCenter || Anchor == Anchor.MiddleCenter || Anchor == Anchor.BottomCenter)
            newX -= Size.X / 2;
        if (Anchor == Anchor.TopRight || Anchor == Anchor.MiddleRight || Anchor == Anchor.BottomRight)
            newX -= Size.X;
        if (Anchor == Anchor.MiddleLeft || Anchor == Anchor.MiddleCenter || Anchor == Anchor.MiddleRight)
            newY -= Size.Y / 2;
        if (Anchor == Anchor.BottomLeft || Anchor == Anchor.BottomCenter || Anchor == Anchor.BottomRight)
            newY -= Size.Y;

        return new Vector2(newX, newY);
    }

    public Vector2 GetPositionIncludingParent(Vector2 position)
    {
        if (Parent == null)
            return position;
        else
            return position + Parent.GetPositionIncludingParent(Parent.Position);
    }

    public virtual void Draw() { }

    //
    // Properties
    //

    /// <summary>
    /// The width.
    /// </summary>
    public float Width
    {
        get { return Size.X; }
        set { Size = new Vector2(value, Size.Y); }
    }

    /// <summary>
    /// The height.
    /// </summary>
    public float Height
    {
        get { return Size.Y; }
        set { Size = new Vector2(Size.Y, value); }
    }

    /// <summary>
    /// The x position.
    /// </summary>
    public float X
    {
        get { return Position.X; }
        set { Position = new Vector2(value, Position.Y); }
    }

    /// <summary>
    /// The y position.
    /// </summary>
    public float Y
    {
        get { return Position.Y; }
        set { Position = new Vector2(Position.X, value); }
    }

    /// <summary>
    /// The top (y) position.
    /// </summary>
    public float Top
    {
        get { return Position.Y; }
        set { Position = new Vector2(Position.X, value); }
    }

    /// <summary>
    /// The middle (y + 1/2 height).
    /// </summary>
    public float Middle
    {
        get { return Position.Y + (Size.Y / 2); }
        set { Position = new Vector2(Position.X, value - (Size.Y / 2)); }
    }

    /// <summary>
    /// The bottom (y + height).
    /// </summary>
    public float Bottom
    {
        get { return Position.Y + Size.Y; }
        set { Position = new Vector2(Position.X, value - Size.Y); }
    }

    /// <summary>
    /// The left (x) position.
    /// </summary>
    public float Left
    {
        get { return Position.X; }
        set { Position = new Vector2(value, Position.Y); }
    }

    /// <summary>
    /// The center (x + 1/2 width).
    /// </summary>
    public float Center
    {
        get { return Position.X + (Size.X / 2); }
        set { Position = new Vector2(value - (Size.X / 2), Position.Y); }
    }

    /// <summary>
    /// The right (x + width).
    /// </summary>
    public float Right
    {
        get { return Position.X + Size.X; }
        set { Position = new Vector2(value - Size.X, Position.Y); }
    }

}