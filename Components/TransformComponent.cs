using Microsoft.Xna.Framework;

namespace MonoGameECS;

public class TransformComponent : Component
{

    private Vector2 Position;
    private Vector2 Size;

    public TransformComponent(Vector2 position, Vector2 size)
    {
        this.Position = position;
        this.Size = size;
    }

    public float Width
    {
        get { return Size.X; }
        set { Size.X = value; }
    }
    public float Height
    {
        get { return Size.Y; }
        set { Size.Y = value; }
    }
    public float X
    {
        get { return Position.X; }
        set { Position.X = value; }
    }
    public float Y
    {
        get { return Position.Y; }
        set { Position.Y = value; }
    }
    public float Top
    {
        get { return Position.Y; }
        set { Position.Y = value; }
    }
    public float Middle
    {
        get { return Position.Y + (Size.Y / 2); }
        set { Position.Y = value - (Size.Y / 2); }
    }
    public float Bottom
    {
        get { return Position.Y + Size.Y; }
        set { Position.Y = value - Size.Y; }
    }
    public float Left
    {
        get { return Position.X; }
        set { Position.X = value; }
    }
    public float Center
    {
        get { return Position.X + (Size.X / 2); }
        set { Position.X = value - (Size.X / 2); }
    }
    public float Right
    {
        get { return Position.X + Size.X; }
        set { Position.X = value - Size.X; }
    }


}