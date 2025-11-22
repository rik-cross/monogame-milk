using System;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

using milk.Core;
using milk.Components;

namespace milk.Systems;

public class TriggerSystem : milk.Core.System
{

    public override void Init()
    {
        AddRequiredComponentType<TransformComponent>();
        AddRequiredComponentType<TriggerComponent>();
    }

    public override void OnEnterScene(Scene scene)
    {
        //Console.WriteLine("onEnterScene");
    }

    public override void OnExitScene(Scene scene)
    {

        // TODO
        // when to clear, as this causes issues when adding a new scene.

        //Console.WriteLine("onExitScene");
        /*foreach (Entity entity in scene.entities)
        {
            if (entity.HasComponent<TriggerComponent>())
            {
                entity.GetComponent<TriggerComponent>().collidedEntities.Clear();
            }
        }*/
    }

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
