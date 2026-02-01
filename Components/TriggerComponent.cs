//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using milk.Core;

namespace milk.Components;

/// <summary>
/// Executes the specified callbacks when 2 triggers collide.
/// </summary>
public class TriggerComponent : Component
{

    /// <summary>
    /// The (x,y) size of the trigger.
    /// </summary>
    public Vector2 Size;

    /// <summary>
    /// The (x,y) offset from the entity top-left position.
    /// </summary>
    public Vector2 Offset;

    /// <summary>
    /// Called once when 2 colliders first intersect.
    /// </summary>
    public Action<Entity, Entity, float>? OnCollisionEnter;

    /// <summary>
    /// Called for every frame that 2 colliders intersect.
    /// </summary>
    public Action<Entity, Entity, float>? OnCollide;

    /// <summary>
    /// Called once when 2 colliders no longer intersect.
    /// </summary>
    public Action<Entity, Entity, float>? OnCollisionExit;

    internal List<Entity> collidedEntities;

    /// <summary>
    /// Creates a new trigger component.
    /// </summary>
    /// <param name="size">The (x,y) size of the trigger.</param>
    /// <param name="offset">The (x,y) offset from the entity top-left position.</param>
    /// <param name="onCollisionEnter">Called once when 2 colliders first intersect.</param>
    /// <param name="onCollide">Called for every frame that 2 colliders intersect.</param>
    /// <param name="onCollisionExit">Called once when 2 colliders no longer intersect.</param>
    public TriggerComponent(Vector2 size, Vector2? offset = null,
        Action<Entity, Entity, float>? onCollisionEnter = null,
        Action<Entity, Entity, float>? onCollide = null,
        Action<Entity, Entity, float>? onCollisionExit = null)
    {
        Size = size;
        Offset = offset ?? Vector2.Zero;
        
        OnCollisionEnter = onCollisionEnter;
        OnCollide = onCollide;
        OnCollisionExit = onCollisionExit;

        collidedEntities = new List<Entity>();
    }

}
