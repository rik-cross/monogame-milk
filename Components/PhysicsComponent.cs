//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: github.com/rik-cross/milk-docs
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using milk.Core;

namespace milk.Components;

/// <summary>
/// Allows an entity to move within a scene.
/// </summary>
public class PhysicsComponent : Component
{

    /// <summary>
    /// The (x,y) velocity.
    /// </summary>
    public Vector2 Velocity;

    /// <summary>
    /// The (x,y) acceleration.
    /// </summary>
    public Vector2 Acceleration;

    /// <summary>
    /// Creates a new physics component.
    /// </summary>
    /// <param name="velocity">The (x,y) velocity.</param>
    /// <param name="acceleration">The (x,y) acceleration.</param>
    public PhysicsComponent(Vector2? velocity = null, Vector2? acceleration = null)
    {
        Velocity = velocity ?? Vector2.Zero;
        Acceleration = acceleration ?? Vector2.Zero;
    }

}