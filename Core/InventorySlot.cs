//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

namespace milk.Core;

/// <summary>
/// Describes the type of recent action.
/// </summary>
public enum SlotChangeType { None, Added, Removed }

/// <summary>
/// An InventorySlot links an Entity ID to a list
/// of entities of that type. Inventory slots can be
/// accessed, but not created or modified directly.
/// Use the parent InventoryComponent To modify.
/// </summary>
public class InventorySlot
{

    /// <summary>
    /// The type of entity stored, which matches to an
    /// Entity.Type property.
    /// </summary>
    public string? Type { get; private set; }

    /// <summary>
    /// A list of entities stored in the inventory slot.
    /// The 'Type' of entity stored will match the
    /// 'Type' property of the slot.
    /// </summary>
    public List<Entity> Items { get; private set; }

    /// <summary>
    /// The game time at which the last slot update was made.
    /// </summary>
    public double LastUpdated { get; internal set; }

    public SlotChangeType LastUpdateType { get; internal set; }

    // Creates an empty slot
    internal InventorySlot()
    {
        Type = null;
        Items = new List<Entity>();
        LastUpdated = -1;
        LastUpdateType = SlotChangeType.None;
    }

    // Adds an entity
    internal bool AddEntity(Entity entity)
    {
        // Only allow a slot to hold one type of entity
        if (Items.Count > 0 && entity.Type != Type)
            return false;

        // Set the type if using an empty slot
        if (Items.Count == 0)
            Type = entity.Type; 
        
        // Add the entity
        Items.Add(entity);

        // Set the last updated time and action
        LastUpdated = Milk.GameTime.TotalGameTime.Seconds;
        LastUpdateType = SlotChangeType.Added;

        return true;
    }

    // Removes an entity
    internal Entity? RemoveEntity()
    {
        // No items to remove
        if (Items.Count == 0) return null;

        // Get the entity to return, from the end of the list
        Entity entityToReturn = Items[Items.Count - 1];
        
        // Remove the entity from the list
        Items.RemoveAt(Items.Count - 1);
        
        // Remove the entity type if there are no entities left
        if (Items.Count == 0) Type = null;
        
        // Set the last updated time and action
        LastUpdated = Milk.GameTime.TotalGameTime.Seconds;
        LastUpdateType = SlotChangeType.Removed;
        
        // Return the entity
        return entityToReturn;
    }

}