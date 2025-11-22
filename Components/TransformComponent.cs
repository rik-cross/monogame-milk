//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: github.com/rik-cross/milk-docs
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using milk.Core;

namespace milk.Components;

/// <summary>
/// Allows an entity to take up space in a scene.
/// </summary>
public class TransformComponent : Component
{

    /// <summary>
    /// The (x,y) top-left entity position. 
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// The (x,y) size of the entity in pixels.
    /// </summary>
    public Vector2 Size;

    /// <summary>
    /// Create a new transform component.
    /// </summary>
    /// <param name="position">The (x,y) top-left entity position.</param>
    /// <param name="size">The (x,y) size of the entity in pixels.</param>
    public TransformComponent(Vector2 position, Vector2? size = null)
    {
        Position = position;
        Size = size ?? Vector2.Zero;
    }

    /// <summary>
    /// The width.
    /// </summary>
    public float Width
    {
        get { return Size.X; }
        set { Size.X = value; }
    }

    /// <summary>
    /// The height.
    /// </summary>
    public float Height
    {
        get { return Size.Y; }
        set { Size.Y = value; }
    }

    /// <summary>
    /// The x position.
    /// </summary>
    public float X
    {
        get { return Position.X; }
        set { Position.X = value; }
    }

    /// <summary>
    /// The y position.
    /// </summary>
    public float Y
    {
        get { return Position.Y; }
        set { Position.Y = value; }
    }

    /// <summary>
    /// The top (y) position.
    /// </summary>
    public float Top
    {
        get { return Position.Y; }
        set { Position.Y = value; }
    }

    /// <summary>
    /// The middle (y + 1/2 height).
    /// </summary>
    public float Middle
    {
        get { return Position.Y + (Size.Y / 2); }
        set { Position.Y = value - (Size.Y / 2); }
    }

    /// <summary>
    /// The bottom (y + height).
    /// </summary>
    public float Bottom
    {
        get { return Position.Y + Size.Y; }
        set { Position.Y = value - Size.Y; }
    }

    /// <summary>
    /// The left (x) position.
    /// </summary>
    public float Left
    {
        get { return Position.X; }
        set { Position.X = value; }
    }

    /// <summary>
    /// The center (x + 1/2 width).
    /// </summary>
    public float Center
    {
        get { return Position.X + (Size.X / 2); }
        set { Position.X = value - (Size.X / 2); }
    }

    /// <summary>
    /// The right (x + width).
    /// </summary>
    public float Right
    {
        get { return Position.X + Size.X; }
        set { Position.X = value - Size.X; }
    }

}