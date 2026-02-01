//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

namespace milk.Core;

public class SystemManager
{

    private SceneManager _sceneManager = EngineGlobals.game.sceneManager;

    // There can only be one instance of each system registered
    internal List<System> registeredSystems = new List<System>();

    //
    // Registered systems
    //

    public string PrintSystems()
    {
        return PrintSystemsInList(registeredSystems);
    }

    internal string PrintSystemsInList(List<System>? systemsList)
    {
        string returnString = "";

        foreach (System system in systemsList)
        {
            returnString += system.ToString() + " ";
        }

        return returnString;        
    }

    public void RegisterSystem(System system)
    {

        // Only allows the addition of a type of system once
        foreach (System s in registeredSystems)
        {
            if (s.GetType() == system.GetType())
                return;
        }

        // Add the system
        registeredSystems.Add(system);

        // Add to scenes that should contain all systems
        foreach (Scene scene in _sceneManager.allScenes)
        {
            if (scene.IncludeAlRegisteredSystems == true && scene.systems.Contains(system) == false)
                AddSystemToScene(system, scene);
        }

    }

    // Returns true if the system manager has a registered
    // system of the specified type
    public bool IsRegistered<T>()
    {
        return HasSystemOfTypeInList<T>(registeredSystems);
    }

    // Gets the (only) system of the specified type
    // within the scene manager
    public T GetSystem<T>() where T : System
    {
        // Iterate over all systems in list
        foreach (System s in registeredSystems)
        {
            // Return the system if the types match
            if (s.GetType() == typeof(T))
                return (T)s;
        }
        return null;
    }

    //
    // System scene methods
    //

    // Adds a system instance to the scene provided
    internal void AddSystemToScene(System system, Scene scene)
    {
        // Only add registered systems
        if (registeredSystems.Contains(system) == false)
            return;

        if (scene.systems.Contains(system) == false)
        {
            scene.systems.Add(system);
            // Callback
            system.OnAddedToScene(scene);
        }
    }

    // Adds all registered systems to a specified scene
    internal void AddAllRegisteredSystemsToScene(Scene scene)
    {
        foreach (System system in registeredSystems)
            AddSystemToScene(system, scene);
    }

    //
    // Generic system / scene list methods
    //

    // Returns true if a system of the specified type
    // exists in the list of systems 
    internal bool HasSystemOfTypeInList<T>(List<System> systems)
    {
        foreach (System system in systems)
        {
            if (system.GetType() == typeof(T))
                return true;
        }
        return false;
    }

    // Returns the (only) instance of a system of the specified type
    // from the specified list.
    internal T GetSystemOfTypeFromList<T>(List<System> systems) where T : System
    {
        // Iterate over all systems in list
        foreach (System s in systems)
        {
            // Return the system if the types match
            if (s.GetType() == typeof(T))
                return (T)s;
        }
        return null;
    }

    // Adds a system of the specified type to the
    // provided system list
    // TODO: add to scene instead, and use this method in Scene()
    // to add a system
    internal void AddSystemOfTypeToList<T>(List<System> systemsList)
    {

        // Add registered systems only
        if (IsRegistered<T>() == false)
            return;

        foreach (System system in systemsList)
            {
                if (system.GetType() == typeof(T))
                {
                    systemsList.Add(system);
                    // Exit, as there should only be one
                    // system of the specified type
                    return;
                }
            }
    }

    // Removes the (only) specified system type from the
    // provided list of systems
    internal void RemoveSystemOfTypeFromList<T>(List<System> systemsList)
    {
        // Iterate backwards through specified systems list
        // to ensure all systems are iterated over
        for (int i = systemsList.Count - 1; i >= 0; i--)
        {
            // Check if types match
            if (systemsList[i].GetType() == typeof(T))
            {
                systemsList.RemoveAt(i);
                // return as there should only be one
                return;
            }
        }
    }

    /// <summary>
    /// Positions the system of the specified type at the requested position.
    /// This is useful for setting the order in which systems execute.
    /// (Use `PrintSystems()` to see the order of systems.)
    /// </summary>
    /// <typeparam name="T">The type of system to move.</typeparam>
    /// <param name="position">The new position of the system.</param>
    public void PositionSystem<T>(int position)
    {
        PositionSystemTypeInList<T>(position, registeredSystems);
    }

    internal void PositionSystemTypeInList<T>(int position, List<System> systemList)
    {
        if (IsRegistered<T>() && position >= 0 && position <= systemList.Count)
        {
            Type targetType = typeof(T);
            object systemToMove = null;
            for (int i = 0; i < systemList.Count; i++)
            {
                // Check the type of the current system in the list.
                if (systemList[i].GetType() == targetType)
                {
                    systemToMove = systemList[i];
                    break;
                }
            }
            if (systemToMove != null)
            {
                // 4. Remove the system from its current position.
                // This prevents duplication and ensures it's only in one place.
                systemList.Remove((System)systemToMove);
                // 6. Insert the system at the new position.
                systemList.Insert(position, (System)systemToMove);
            }
        }
    }


}