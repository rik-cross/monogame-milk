//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using System.Collections;
using Microsoft.Xna.Framework;

namespace milk.Core;

public class EntityManager
{
    // Store local references to manager objects
    private ComponentManager _componentManager = EngineGlobals.game.componentManager;
    private SceneManager _sceneManager = EngineGlobals.game.sceneManager;

    public readonly int MaxExtities;
    internal List<Entity> _allEntities = new List<Entity>();
    private Queue<int> IDPool = new Queue<int>();
    private Dictionary<string, Func<Vector2?, Entity>> _prototypes = new();

    internal EntityManager(int maxExtities)
    {

        MaxExtities = maxExtities;

        for (int i = 0; i < MaxExtities; i++)
            IDPool.Enqueue(i);

    }

    internal void AddEntity(Entity entity)
    {
        _allEntities.Add(entity);
    }

    internal void DeleteEntities()
    {

        // delete entity from all scenes
        foreach (Scene scene in _sceneManager.allScenes)
        {
            for (int i = scene.Entities.Count - 1; i >= 0; i--)
            {
                if (scene.Entities[i].Delete == true)
                {
                    Entity? e = scene.Entities[i];
                    _componentManager.RemoveAllComponentsFromEntity(scene.Entities[i]);
                    scene.OnEntityRemoved(e);
                    scene.Entities.RemoveAt(i);

                    // System.OnEntityRemovedFromScene

                    foreach (System system in scene.systems)
                    {

                        // only if the entity has the required components for the system
                        BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                        temp.And(e.bitMask);
                        if (Utilities.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
                            system.OnEntityRemovedFromScene(scene, e);

                    }

                }
            }
        }

        // delete entity from list of all entities
        for (int i = _allEntities.Count - 1; i >= 0; i--)
        {
            if (_allEntities[i].Delete == true)
            {
                CheckInID(_allEntities[i].ID);
                _allEntities.RemoveAt(i);
            }
        }
        
    }

    internal void CheckInID(int ID)
    {
        IDPool.Enqueue(ID);
        IDPool.Order();
    }

    internal int CheckOutID()
    {
        return IDPool.Dequeue();
    }

    // Get the (only) entity in the list of all entities
    internal Entity? GetEntityByName(string name)
    {
        return GetEntityByNameInList(_allEntities, name);
    }

    // Get the (only) entity in a provided list by name
    internal Entity? GetEntityByNameInList(List<Entity> entityList, string name)
    {
        foreach (Entity entity in entityList)
        {
            if (entity.Name != null && entity.Name.ToLower() == name.ToLower())
                return entity;
        }
        return null;
    }

    internal List<Entity> GetEntitiesByTag(params string[] tags)
    {
        return GetEntitiesByTagInList(_allEntities, tags);
    }

    internal List<Entity> GetEntitiesByTagInList(List<Entity> entityList, params string[] tags)
    {
        List<Entity> returnList = new List<Entity>();
        foreach (Entity entity in entityList)
        {
            if (entity.HasTag(tags) == true)
                returnList.Add(entity);
        }
        return returnList;
    }

    internal List<Entity> GetEntitiesByType(string type)
    {
        return GetEntitiesByTypeInList(_allEntities, type);
    }

    internal List<Entity> GetEntitiesByTypeInList(List<Entity> entityList, string type)
    {
        List<Entity> returnList = new List<Entity>();
        foreach (Entity entity in entityList)
        {
            if (entity.Type != null && entity.Type.ToLower() == type.ToLower())
                returnList.Add(entity);
        }
        return returnList;
    }

    internal void RemoveEntityByNameInList(List<Entity> entityList, string name)
    {
        foreach (Entity entity in entityList)
        {
            if (entity.Name != null && entity.Name.ToLower() == name.ToLower())
            {
                // Remove the entity
                entityList.Remove(entity);

                // Exit, as there should only be one entity
                // with the specified name
                return;
            }
        }
    }

    internal void RemoveEntitiesByTagInList(List<Entity> entityList, params string[] tags)
    {
        for (int i = entityList.Count - 1; i > 0; i--)
        {
            if (entityList[i].HasTag(tags))
                entityList.RemoveAt(i);
        }
    }

    internal void RemoveEntitiesByTypeInList(List<Entity> entityList, string type)
    {
        for (int i = entityList.Count - 1; i > 0; i--)
        {
            if (entityList[i].Type != null && entityList[i].Type!.ToLower() == type.ToLower())
                entityList.RemoveAt(i);
        }
    }  

    //
    // Entity prototyping
    //

    /// <summary>
    /// Add a method for creating a new instance of an entity of a particular type.
    /// </summary>
    /// <param name="entityType">The type of the entity to create, which should match Entity.Type</param>
    /// <param name="prototypeMethod">The method used to create a new entity (optionally specifying a Vector2 position and returning an Entity)</param>
    public void AddPrototype(string entityType, Func<Vector2?, Entity> prototypeMethod)
    {
        if (_prototypes.ContainsKey(entityType) == false)
            RemovePrototype(entityType);
        _prototypes.Add(entityType, prototypeMethod);
    }

    /// <summary>
    /// Call the method associated with an entity type to create a new entity.
    /// </summary>
    /// <param name="entityType">The 'type' of entity to create.</param>
    /// <param name="position">The optional (x, y) world position.</param>
    /// <returns>An entity if a ethod exists for the type, or null.</returns>
    public Entity? CreateFromPrototype(string entityType, Vector2? position)
    {
        return _prototypes.ContainsKey(entityType) ? _prototypes[entityType].Invoke(position) : null;
    }

    /// <summary>
    /// Removes the method for creating an entity of the specified type if one exists.
    /// </summary>
    /// <param name="entityType">The type of entity creation method to remove.</param>
    public void RemovePrototype(string entityType)
    {
        if (_prototypes.ContainsKey(entityType))
            _prototypes.Remove(entityType);
    }

    /// <summary>
    /// Returns whether an entity creation prototype exists for a particular entity type.
    /// </summary>
    /// <param name="entityType">The entity type to check.</param>
    /// <returns>True if a prototype method exists, false otherwise.</returns>
    public bool HasPrototype(string entityType)
    {
        return _prototypes.ContainsKey(entityType);
    }

}