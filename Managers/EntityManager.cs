//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using System.Collections;

namespace milk.Core;

internal class EntityManager
{
    // Store local references to manager objects
    private ComponentManager _componentManager = EngineGlobals.game.componentManager;
    private SceneManager _sceneManager = EngineGlobals.game.sceneManager;

    public readonly int MaxExtities;
    public List<Entity> _allEntities = new List<Entity>();
    private Queue<int> IDPool = new Queue<int>();

    public EntityManager(int maxExtities)
    {

        MaxExtities = maxExtities;

        for (int i = 0; i < MaxExtities; i++)
            IDPool.Enqueue(i);

    }

    public void AddEntity(Entity entity)
    {
        _allEntities.Add(entity);
    }

    public void DeleteEntities()
    {

        // delete entity from all scenes
        foreach (Scene scene in _sceneManager.allScenes)
        {
            for (int i = scene.entities.Count - 1; i >= 0; i--)
            {
                if (scene.entities[i].Delete == true)
                {
                    Entity? e = scene.entities[i];
                    _componentManager.RemoveAllComponentsFromEntity(scene.entities[i]);
                    scene.OnEntityRemoved(e);
                    scene.entities.RemoveAt(i);

                    // System.OnEntityRemovedFromScene

                    foreach (System system in scene.systems)
                    {

                        // only if the entity has the required components for the system
                        BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                        temp.And(e.bitMask);
                        if (Utils.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
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

    public void CheckInID(int ID)
    {
        IDPool.Enqueue(ID);
        IDPool.Order();
        //Console.WriteLine("check in " + ID);
        // should components be removed, etc.?
    }

    public int CheckOutID()
    {
        return IDPool.Dequeue();
    }

    // Get the (only) entity in the list of all entities
    public Entity? GetEntityByName(string name)
    {
        return GetEntityByNameInList(_allEntities, name);
    }

    // Get the (only) entity in a provided list by name
    public Entity? GetEntityByNameInList(List<Entity> entityList, string name)
    {
        foreach (Entity entity in entityList)
        {
            if (entity.Name != null && entity.Name.ToLower() == name.ToLower())
                return entity;
        }
        return null;
    }

    public List<Entity> GetEntitiesByTag(params string[] tags)
    {
        return GetEntitiesByTagInList(_allEntities, tags);
    }

    public List<Entity> GetEntitiesByTagInList(List<Entity> entityList, params string[] tags)
    {
        List<Entity> returnList = new List<Entity>();
        foreach (Entity entity in entityList)
        {
            if (entity.HasTag(tags) == true)
                returnList.Add(entity);
        }
        return returnList;
    }

    public void RemoveEntityByNameInList(List<Entity> entityList, string name)
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

    public void RemoveEntitiesByTagInList(List<Entity> entityList, params string[] tags)
    {
        for (int i = entityList.Count - 1; i > 0; i--)
        {
            if (entityList[i].HasTag(tags))
                entityList.RemoveAt(i);
        }
    }

    public override string ToString()
    {
        string output = "";
        output += Theme.CreateConsoleTitle("EntityManager");
        int percentage = (int)((float)_allEntities.Count / (float)MaxExtities * 100 / 10);
        string p = _allEntities.Count.ToString() + "/" + MaxExtities.ToString() + " created" + " ";
        p += new string('◼', percentage) + new string('◻', 10 - percentage);
        output += Theme.PrintConsoleVar("Entities", p);
        return output;
    }

}