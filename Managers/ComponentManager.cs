//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

namespace milk.Core;

internal class ComponentManager {

    // The maximum number of components is used to create
    // a 2D entity-component list of the correct size
    internal readonly int maxComponents;

    // The component types list stores the type of all
    // registered component types. The ID of a component
    // is its position in the component types list
    private List<Type> _componentTypesList = new List<Type>();

    // The 2D list of components stored for each entity
    // stored as _entityComponents[componentID][entityID]
    private Component[,] _entityComponents;

    internal ComponentManager(int maxComponents, int maxEntities) {
        this.maxComponents = maxComponents;
        _entityComponents = new Component[maxComponents, maxEntities];
    }

    //
    // Component types
    //

    // Registers a component type by adding it to the list of
    // component types. A component type can only be registered once
    internal void RegisterComponentType(Type componentType)
    {
        if (_componentTypesList.Contains(componentType) == false)
            _componentTypesList.Add(componentType);
    }

    // Returns true if the component type exists in the
    // component types list
    internal bool IsComponentTypeRegistered(Type componentType)
    {
        if (_componentTypesList.Contains(componentType))
            return true;
        else
            return false;
    }

    // Returns the ID of a provided component type, which is just
    // the index of the component type in the registered types list.
    // (Only call this if the component type is already registered)
    internal int GetComponentTypeID(Type componentType)
    {
        return _componentTypesList.IndexOf(componentType);
    }

    //
    // Entities and components
    //

    internal void AddComponentToEntity(Entity entity, Component component)
    {

        // Register the type of the component if not already registered.
        // Auto-registering new component types means that the user does
        // not need to do this themselves
        if (IsComponentTypeRegistered(component.GetType()) == false)
            RegisterComponentType(component.GetType());

        // Add the component at the correct place in the 2D entity-comopnent array
        // and update the component flags for the entity, to denote that the entity
        // now has an instance of the new component
        _entityComponents[GetComponentTypeID(component.GetType()), entity.ID] = component;
        UpdateEntityComponentFlagsForEntity(entity);

        // Execute the component callback for the parent entity
        // of the newly-added component
        component.OnAddedToEntity(entity);

    }

    // Returns the component of the specified template type
    // for the specified entity. Returns null if the entity doesn't have
    // a component of the specified type
    internal T? GetComponentForEntity<T>(Entity entity) where T : Component
    {

        // Return null if the component type isn't registered
        if (_componentTypesList.Contains(typeof(T)) == false)
            return null;
        // Return null if the entity doesn't have a component
        // of the specified type
        if (EntityHasComponentOfType<T>(entity) == false)
            return null;

        // Get the component from the 2D entity-component list
        // using the component type and entity IDs
        return (T)_entityComponents[GetComponentTypeID(typeof(T)), entity.ID];

    }

    // Returns true if the entity has a component of the specified
    // component type
    internal bool EntityHasComponentOfType<T>(Entity entity) where T : Component
    {

        // Return false if the provided component type isn't registered
        if (_componentTypesList.Contains(typeof(T)) == false)
            return false;

        // A component exists for the entity in the 2D entity-component
        // array if the [componentID][entityID] slot is not null
        return _entityComponents[GetComponentTypeID(typeof(T)), entity.ID] != null;

    }

    // Removes the component of the specified type from the entity provided
    // (if one exists)
    internal void RemoveComponentFromEntity<T>(Entity entity) where T : Component
    {

        // Do nothing if the component type isn't registered
        if (_componentTypesList.Contains(typeof(T)) == false)
            return;

        int componentTypeID = GetComponentTypeID(typeof(T));
        int entityID = entity.ID;

        // Get the component to remove
        T component = (T)_entityComponents[componentTypeID, entityID];

        // Remove the component
        _entityComponents[componentTypeID, entityID] = null;

        // Update the flags for the entity, as the entity no
        // longer has a component of the specified type
        UpdateEntityComponentFlagsForEntity(entity);

        // Execute the component callback for the parent entity
        // of the removed component
        component.OnRemovedFromEntity(entity);

    }

    // Removes all components for the specified entity
    internal void RemoveAllComponentsFromEntity(Entity entity)
    {

        // Loop through all components
        for (int i = 0; i < _entityComponents.GetLength(0); i++)
        {
            // Delete the component if one exists
            if (_entityComponents[i, entity.ID] != null)
            {
                // Get the component
                Component component = _entityComponents[i, entity.ID];
                // Execute the OnRemoved callback for the removed component
                component.OnRemovedFromEntity(entity);
                // Delete the component
                _entityComponents[i, entity.ID] = null;
            }
            // Update the component flags, as the entity has no components
            UpdateEntityComponentFlagsForEntity(entity);
        }

    }

    // Keeps track of which components an entity has. This is useful
    // for running systems for entities with the correct set of components
    internal void UpdateEntityComponentFlagsForEntity(Entity entity)
    {
        // Iterate over each component type
        for (int componentID = 0; componentID < _entityComponents.GetLength(0); componentID++)
        {

            // If the entity has a component of this type, then set the bitmask
            // at the position of the component in the entity-component list
            entity.bitMask.Set(componentID, _entityComponents[componentID, entity.ID] != null);
        }
    }

}