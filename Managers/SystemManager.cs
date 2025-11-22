//   [ ] Finished
//   
//   Monogame ECS Engine
//   By Rik Cross
//   -- github.com/rik-cross/monogame-ecs
//   Shared under the MIT licence
//
//   ------------------------------------
//
//   MonogameECS.SystemManager
//   ============================
//  
//   A system manager stores all systems.

using System.Collections.Generic;

namespace milk.Core;

internal class SystemManager
{

    private SceneManager _sceneManager = EngineGlobals.game.sceneManager;

    // There can only be one instance of each system registered
    public List<System> registeredSystems = new List<System>();

    //
    // Registered systems
    //

    public string PrintSystems()
    {
        string returnString = "";

        foreach (System system in registeredSystems)
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
            if (scene.IncludeAlRegisteredSystems == true && scene.systems.Contains(system))
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
    private void AddSystemToScene(System system, Scene scene)
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
    public void AddAllRegisteredSystemsToScene(Scene scene)
    {
        foreach (System system in registeredSystems)
        {
            AddSystemToScene(system, scene);
            // Callback
            system.OnAddedToScene(scene);
        }
    }

    //
    // Generic system / scene list methods
    //

    // Returns true if a system of the specified type
    // exists in the list of systems 
    public bool HasSystemOfTypeInList<T>(List<System> systems)
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
    public T GetSystemOfTypeFromList<T>(List<System> systems) where T : System
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
    public void AddSystemOfTypeToList<T>(List<System> systemsList)
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
    public void RemoveSystemOfTypeFromList<T>(List<System> systemsList)
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

}