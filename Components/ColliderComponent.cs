//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using milk.Core;

namespace milk.Components;

/// <summary>
/// Sets a collision bounding box for an entity.
/// </summary>
public class ColliderComponent : Component
{

    /// <summary>
    /// The (x,y) size of the collider.
    /// </summary>
    public Vector2 Size;

    /// <summary>
    /// The (x,y) distance from the entity position.
    /// </summary>
    public Vector2 Offset;

    /// <summary>
    /// Creates a new collider component.
    /// </summary>
    /// <param name="size">The (x,y) size of the collider.</param>
    /// <param name="offset">The (x,y) distance from the entity position.</param>
    public ColliderComponent(Vector2 size, Vector2 offset)
    {
        this.Size = size;
        this.Offset = offset;
    }

}