//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;

namespace milk.Core;

/// <summary>
/// The part of a SceneRenderable to set the position against.
/// </summary>
public enum Anchor
{
    TopLeft, TopCenter, TopRight,
    MiddleLeft, MiddleCenter, MiddleRight,
    BottomLeft, BottomCenter, BottomRight
}

/// <summary>
/// Used as a superclass for images, text, etc.
/// </summary>
public abstract class SceneRenderable
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set;}
    public float Alpha;
    public Anchor Anchor { get; set; }
    protected readonly SceneRenderable? Parent;

    internal SceneRenderable(
        Vector2 size,
        Vector2? position = null,
        float alpha = 1.0f,
        Anchor anchor = Anchor.TopLeft,
        SceneRenderable? parent = null)
    {

        Size = size;
        Position = position ?? Vector2.Zero;
        Alpha = alpha;
        Anchor = anchor;
        Parent = parent;  

    }

    protected Vector2 CalculateTopLeftPositionFromAnchor(Vector2 position)
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

    protected Vector2 GetPositionIncludingParent(Vector2 position)
    {
        if (Parent == null)
            return position;
        else
            return position + Parent.GetPositionIncludingParent(Parent.Position);
    }

    public virtual void Draw() { }

}