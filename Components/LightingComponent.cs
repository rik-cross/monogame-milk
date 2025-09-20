using Microsoft.Xna.Framework;

namespace MonoGameECS;

public class LightingComponent : Component
{
    public Vector2 offset;
    public float worldRadius;
    public Color color;
    public float brightness;
    public LightingComponent(Vector2 offset = default, float worldRadius = 50, float brightness = 1.0f, Color color = default)
    {
        if (offset == default)
            this.offset = Vector2.Zero;
        else
            this.offset = offset;

        this.worldRadius = worldRadius;
        this.brightness = brightness;

        if (color == default)
            this.color = Color.White;
        else
            this.color = color;
    }
}