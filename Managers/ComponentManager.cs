//   [ ] Finished
//   
//   Monogame ECS Engine
//   By Rik Cross
//   -- github.com/rik-cross/monogame-ecs
//   Shared under the MIT licence
//
//   ------------------------------------
//
//   MonogameECS.ComponentManager
//   ============================
//  
//   A component manager stores components against entities
//   in a 2D array.

using System;
using System.Collections.Generic;

namespace milk;

public class ComponentManager {

    // The maximum number of components is used to create
    // a 2D entity-component list of the correct size
    public readonly int maxComponents;

    // The component types list stores the type of all
    // registered component types. The ID of a component
    // is its position in the component types list
    private List<Type> _componentTypesList = new List<Type>();

    // The 2D list of components stored for each entity
    // stored as _entityComponents[componentID][entityID]
    private Component[,] _entityComponents;

    public ComponentManager(int maxComponents, int maxEntities) {
        this.maxComponents = maxComponents;
        _entityComponents = new Component[maxComponents, maxEntities];
    }

    //
    // Component types
    //

    // Registers a component type by adding it to the list of
    // component types. A component type can only be registered once
    public void RegisterComponentType(Type componentType)
    {
        if (_componentTypesList.Contains(componentType) == false)
            _componentTypesList.Add(componentType);
    }

    // Returns true if the component type exists in the
    // component types list
    public bool IsComponentTypeRegistered(Type componentType)
    {
        if (_componentTypesList.Contains(componentType))
            return true;
        else
            return false;
    }

    // Returns the ID of a provided component type, which is just
    // the index of the component type in the registered types list.
    // (Only call this if the component type is already registered)
    public int GetComponentTypeID(Type componentType)
    {
        return _componentTypesList.IndexOf(componentType);
    }

    //
    // Entities and components
    //

    public void AddComponentToEntity(Entity entity, Component component)
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
    public T GetComponentForEntity<T>(Entity entity) where T : Component
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
    public bool EntityHasComponentOfType<T>(Entity entity) where T : Component
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
    public void RemoveComponentFromEntity<T>(Entity entity) where T : Component
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
    public void RemoveAllComponentsFromEntity(Entity entity)
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
    public void UpdateEntityComponentFlagsForEntity(Entity entity)
    {
        // Iterate over each component type
        for (int componentID = 0; componentID < _entityComponents.GetLength(0); componentID++)
        {

            // If the entity has a component of this type, then set the bitmask
            // at the position of the component in the entity-component list
            entity.bitMask.Set(componentID, _entityComponents[componentID, entity.ID] != null);
        }
    }

    public override string ToString()
    {
        string output = "";
        output += Theme.CreateConsoleTitle("ComponentManager");
        string components = _componentTypesList.Count + "/" + _entityComponents.GetLength(0).ToString() + " created ";
        int percentage = (int)((float)_componentTypesList.Count / (float)maxComponents * 100 / 10);
        components += new string('◼', percentage) + new string('◻', 10-percentage);
        output += Theme.PrintConsoleVar("Components", components);
        output += " Entities→ / components↓\n";
        for (int i = 0; i < _entityComponents.GetLength(0); i++)
        {
            output += " ";
            for (int j = 0; j < _entityComponents.GetLength(1); j++)
            {
                if (_entityComponents[i, j] == null)
                    output += "◻";
                else
                    output += "◼";
            }
            output += "\n";
        }
        return output;
    }

}