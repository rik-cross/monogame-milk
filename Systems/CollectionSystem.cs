//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using milk.Core;
using milk.Components;

namespace milk.Systems;

/// <summary>
/// Provides methods for dropping and picking up entities.
/// Note that only entities with a CollectableComponent and
/// a TriggerComponent can be collected.
/// </summary>
public class CollectionSystem : milk.Core.System
{

    /// <summary>
    /// An optional method called whenever an entity is collected.
    /// </summary>
    public Action? OnCollectEntity = null;

    /// <summary>
    /// An optional method called whenever entities are dropped.
    /// Note that this method is only called once if
    /// multiple items are dropped together.
    /// </summary>
    public Action? OnDropEntity = null;

    /// <summary>
    /// Option to show or hide log info.
    /// </summary>
    public bool ShowLogInfo { get; set; } = true;

    /// <summary>
    /// Defines a delegate method for dropping entities.
    /// </summary>
    public Func<Scene, Entity, Entity, Vector2>? PositionCalculationMethod { get; set; } = null;

    /// <summary>
    /// Finds an entities closest overlapping entity, and adds it
    /// to their inventory.
    /// Note that both entities must have a TriggerComponents, and
    /// these components must be intersecting.
    /// </summary>
    /// <param name="scene">The scene to check.</param>
    /// <param name="entity">The entity requesting to pick up another entity.</param>
    public void CollectNearestEntity(Scene scene, Entity entity)
    {

        // Only allow entities with inventories and triggers to pick up
        if (
            entity.HasComponent<InventoryComponent>() == false ||
            entity.HasComponent<TriggerComponent>() == false
        )
            return;

        List<Entity> collectionCandidateEntities = new List<Entity>();
        List<Rectangle> collectionCandidateRectangles = new List<Rectangle>();

        TransformComponent entityTransform = entity.GetComponent<TransformComponent>()!;
        TriggerComponent entityTrigger = entity.GetComponent<TriggerComponent>()!;

        Rectangle entityTriggerRect = new Rectangle(
            (int)(entityTransform.X + entityTrigger.Offset.X),
            (int)(entityTransform.Y + entityTrigger.Offset.Y),
            (int)entityTrigger.Size.X,
            (int)entityTrigger.Size.Y
        );

        foreach(Entity entityToCollect in scene.Entities)
        {
            if (
                entityToCollect != entity &&
                entityToCollect.HasComponent<CollectableComponent>() &&
                entityToCollect.HasComponent<TriggerComponent>()
            )
            {

                TransformComponent collectTransform = entityToCollect.GetComponent<TransformComponent>()!;
                TriggerComponent collectTrigger = entityToCollect.GetComponent<TriggerComponent>()!;

                Rectangle entityCollectRect = new Rectangle(
                    (int)(collectTransform.X + collectTrigger.Offset.X),
                    (int)(collectTransform.Y + collectTrigger.Offset.Y),
                    (int)collectTrigger.Size.X,
                    (int)collectTrigger.Size.Y
                );

                if (entityTriggerRect.Intersects(entityCollectRect))
                {
                    collectionCandidateEntities.Add(entityToCollect);
                    collectionCandidateRectangles.Add(entityCollectRect);
                }
            }
        }

        // If there's only a single entity then pick it up
        if (collectionCandidateEntities.Count == 1)
        {
            // Remove the entity from the scene
            scene.RemoveEntity(collectionCandidateEntities[0]);
            // Add the entity to the inventory
            entity.GetComponent<InventoryComponent>()!.AddEntity(collectionCandidateEntities[0]);
            // Show log info
            if (ShowLogInfo == true)
                Log.Add("Collected " + collectionCandidateEntities[0].Type);
            // Execute the callback if one exists
            OnCollectEntity?.Invoke();
            return;
        }

        // If there are multiple intersecting entities
        // then find the nearest.
        else if (collectionCandidateEntities.Count > 1)
        {

            //int entityIndex = -1;
            float maxValue = float.MaxValue;
            Entity? closestEntity = null;

            for(int i=0; i<collectionCandidateRectangles.Count - 1; i++)
            {
                float d = Vector2.Distance(
                    collectionCandidateRectangles[i].Center.ToVector2(),
                    entityTriggerRect.Center.ToVector2()
                );

                if (d < maxValue)
                {
                    maxValue = d;
                    closestEntity = collectionCandidateEntities[i];
                }

            }

            // If a closest entity has been found
            if (closestEntity != null && maxValue < float.MaxValue)
            {
                // Remove the entity from the scene
                scene.RemoveEntity(closestEntity);
                // Add the entity to the inventory
                entity.GetComponent<InventoryComponent>()!.AddEntity(closestEntity);
                // Show log info
                if (ShowLogInfo == true)
                    Log.Add("Collected " + closestEntity.Type);
                // Execute the callback if one exists
                OnCollectEntity?.Invoke();
                return;
            }

        }

    }

    /// <summary>
    /// Drops the specified entity into a scene at the specified location.
    /// </summary>
    /// <param name="scene">The scene to drop the entity in.</param>
    /// <param name="entityDropping">The entity wanting to drop another entity.</param>
    /// <param name="droppedEntity">The entity being dropped.</param>
    public void DropEntity(Scene scene, Entity entityDropping, Entity droppedEntity)
    {
        DropEntities(scene, entityDropping, new List<Entity>(){droppedEntity});
    }

    /// <summary>
    /// Drops the specified entity list into a scene at the specified location.
    /// </summary>
    /// <param name="scene">The scene to drop the entity in.</param>
    /// <param name="entityDropping">The entity wanting to drop another entity.</param>
    /// <param name="droppedEntities">The entities being dropped.</param>
    public void DropEntities(Scene scene, Entity entityDropping, List<Entity> droppedEntities)
    {

        // Only drop if there's a method specified for calculating the position
        if (PositionCalculationMethod == null)
            return;
            
        // This flag checks that at least one entity has been dropped
        // in order to call the OnDropEntity callback
        bool dropped = false;

        // Drop each entity in turn
        foreach (Entity droppedEntity in droppedEntities)
        {
            // Use the delegated method to calculate the drop position
            Vector2 p = PositionCalculationMethod(scene, entityDropping, droppedEntity);
            // Set the entity position
            droppedEntity.GetComponent<TransformComponent>()!.Position = p;
            // Add the entity to the scene
            scene.AddEntity(droppedEntity);
            
            // At least one entity has been dropped
            dropped = true;
        }

        // Show log info
        if (ShowLogInfo == true)
        {
            // Log message for single item
            if (droppedEntities.Count == 1)
                Log.Add("Dropped " + droppedEntities[0].Type);
            // Log message for multiple items
            else
            {
                Dictionary<string, int> itemInfo = new Dictionary<string, int>();
                foreach (Entity e in droppedEntities)
                {
                    if (itemInfo.ContainsKey(e.Type) == false)
                        itemInfo.Add(e.Type, 1);
                    else
                        itemInfo[e.Type] += 1;
                }
                foreach (var ie in itemInfo)
                    Log.Add("Dropped " + ie.Key + " x " + ie.Value);
            }
        }

        if (dropped == true)
            // Execute the callback if one exists
            OnDropEntity?.Invoke();
    }


}
