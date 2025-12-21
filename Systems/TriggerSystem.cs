//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using MonoGame.Extended;
using milk.Core;
using milk.Components;

namespace milk.Systems;

/// <summary>
/// Processes entities with trigger components, and calls the relevant
/// callbacks when 2 entities have overlapping colliders.
/// </summary>
internal class TriggerSystem : milk.Core.System
{

    /// <summary>
    /// Initialises the trigger system, which only processes entities
    /// with a transform component and a trigger component.
    /// </summary>
    public override void Init()
    {
        AddRequiredComponentType<TransformComponent>();
        AddRequiredComponentType<TriggerComponent>();
    }

    /// <summary>
    /// Updates each of the entities with the required components.
    /// </summary>
    /// <param name="gameTime">The MonoGame gameTime object for measuring elapsed time.</param>
    /// <param name="scene">The scene containing the system.</param>
    /// <param name="entity">The entity to be processed.</param>
    public override void UpdateEntity(GameTime gameTime, Scene scene, Entity entity)
    {
        TransformComponent transformComponent = entity.GetComponent<TransformComponent>();
        TriggerComponent triggerComponent = entity.GetComponent<TriggerComponent>();

        // this rect
        Rectangle triggerRect = new Rectangle(
            (int)(transformComponent.X + (int)triggerComponent.offset.X),
            (int)(transformComponent.Y + (int)triggerComponent.offset.Y),
            (int)(triggerComponent.size.X),
            (int)(triggerComponent.size.Y)
        );

        // check against others
        foreach (Entity otherEntity in scene.entities)
        {
            if (
                otherEntity != entity &&
                otherEntity.HasComponent<TransformComponent>() &&
                otherEntity.HasComponent<TriggerComponent>()
            )
            {

                TransformComponent otherTransformComponent = otherEntity.GetComponent<TransformComponent>();
                TriggerComponent otherTriggerComponent = otherEntity.GetComponent<TriggerComponent>();

                // other entity rect
                Rectangle otherTriggerRect = new Rectangle(
                    (int)(otherTransformComponent.X + (int)otherTriggerComponent.offset.X),
                    (int)(otherTransformComponent.Y + (int)otherTriggerComponent.offset.Y),
                    (int)(otherTriggerComponent.size.X),
                    (int)(otherTriggerComponent.size.Y)
                );
                
                // calculate distance
                float thisXMiddle = transformComponent.X + triggerComponent.offset.X + (triggerComponent.size.X / 2);
                float otherXMiddle = otherTransformComponent.X + otherTriggerComponent.offset.X + (otherTriggerComponent.size.X / 2);
                float thisYMiddle = transformComponent.Y + triggerComponent.offset.Y + (triggerComponent.size.Y / 2);
                float otherYMiddle = otherTransformComponent.Y + otherTriggerComponent.offset.Y + (otherTriggerComponent.size.Y / 2);
                float xDiff = Math.Abs(thisXMiddle - otherXMiddle);
                float yDiff = Math.Abs(thisYMiddle - otherYMiddle);
                float distance = (float)Math.Sqrt((xDiff * xDiff) + (yDiff * yDiff));

                // process triggers
                if (triggerRect.Intersects(otherTriggerRect))
                {

                    // onCollisionEnter
                    if (!triggerComponent.collidedEntities.Contains(otherEntity))
                    {
                        if (triggerComponent.onCollisionEnter != null)
                        {
                            triggerComponent.onCollisionEnter(entity, otherEntity, distance);
                        }
                        triggerComponent.collidedEntities.Add(otherEntity);
                    } else

                    // onCollide
                    {
                        if (triggerComponent.onCollide != null)
                        {
                            triggerComponent.onCollide(entity, otherEntity, distance);
                        }
                    }

                }
                else

                // onCollisionExit
                {

                    if (triggerComponent.collidedEntities.Contains(otherEntity))
                    {
                        if (triggerComponent.onCollisionExit != null)
                        {
                            triggerComponent.onCollisionExit(entity, otherEntity, distance);
                        }
                        triggerComponent.collidedEntities.Remove(otherEntity);
                    }

                }

            }
        }

    }

    /// <summary>
    /// Draws each of the entity triggers if in DEBUG mode.
    /// </summary>
    /// <param name="scene">The scene containing the system.</param>
    /// <param name="entity">The entity to be processed.</param>
    public override void DrawEntity(Scene scene, Entity entity)
    {
        TransformComponent transformComponent = entity.GetComponent<TransformComponent>();
        TriggerComponent triggerComponent = entity.GetComponent<TriggerComponent>();

        if (scene.game.Debug == true)
        {
            spriteBatch.DrawRectangle(
                new Rectangle(
                    (int)transformComponent.X + (int)triggerComponent.offset.X,
                    (int)transformComponent.Y + (int)triggerComponent.offset.Y,
                    (int)triggerComponent.size.X,
                    (int)triggerComponent.size.Y
                ),
                Color.Red,
                1.0f
            );
        }

    }

    /// <summary>
    /// Removes the entity to be removed from the scene from the list
    /// of collided entities.
    /// </summary>
    /// <param name="scene">The scene containing the system.</param>
    /// <param name="entity">The entity to be processed.</param>
    public override void OnEntityRemovedFromScene(Scene scene, Entity entity)
    {
        // Clear entity trigger's collided components
        TriggerComponent triggerComponent = entity.GetComponent<TriggerComponent>();
        triggerComponent.collidedEntities.Clear();

        // Remove the entity from other scene entities trigger's collided components
        foreach (Entity e in scene.entities)
        {
            if (e.HasComponent<TriggerComponent>() && e != entity)
            {
                TriggerComponent otherTriggerComponent = e.GetComponent<TriggerComponent>();
                otherTriggerComponent.collidedEntities.Remove(entity);
            }
        }

    }

}
