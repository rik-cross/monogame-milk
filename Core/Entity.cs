//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using System.Collections;

namespace milk.Core;

/// <summary>
/// An Entity is a logical container for components.
/// Systems in a Scene act on entities with the required set of components.
/// </summary>
public class Entity
{

    // Manager references, for access to all other entities.
    private EntityManager _entityManager = EngineGlobals.game.entityManager;
    private ComponentManager _componentManager = EngineGlobals.game.componentManager;

    /// <summary>
    /// The unique ID of the entity.
    /// </summary>
    public int ID { get; internal set; }

    //
    // Entity name
    //

    private string? _name;
    
    /// <summary>
    /// The unique (not case-sensitive) name of the entity.
    /// Setting a non-unique, non-null name will raise an InvalidOperationException.
    /// </summary>
    public string? Name
    {
        get { return _name; }
        set
        {
            foreach (Entity entity in this._entityManager._allEntities)
            {
                if (entity.Name != null && entity.Name.ToLower() == value)
                    throw new InvalidOperationException($"Cannot set the name. The name '{value}' is already in use by another entity.");
            }
             _name = value;
        }
    }

    //
    // Entity state
    //

    private string _state;
    
    /// <summary>
    /// The state of the entity in the previous frame.
    /// </summary>
    public string PreviousState{ get; internal set; }
    
    /// <summary>
    /// The current state of the entity.
    /// </summary>
    public string State
    {
        get { return _state; }
        set
        {
            PreviousState = _state;
            _state = value;
        }
    }

    //
    // Entity owner
    //

    private Entity? _owner;

    /// <summary>
    /// The owner Entity.
    /// This is set to itself if null is passed.
    /// </summary>
    public Entity? Owner
    {
        get { return _owner; }
        set { _owner = (value == null) ? this : value; }
    }

    /// <summary>
    /// Setting Delete = true marks an entity for deletion.
    /// Deletion happens at the end of each game loop.
    /// </summary>
    public bool Delete { get; set; }

    // A bit array indicating which components
    // the entity contains. The position of each
    // bit in the array is the ID of the associated
    // component 

    /// <summary>
    /// The bitMask of an Entity uses each component ID
    /// to record which components are present for an entity.
    /// </summary>
    internal BitArray bitMask;

    //
    // Entity tags
    //

    private List<string> _tags = new List<string>();
    
    /// <summary>
    /// A list of string tags, used to categorise and/or identify an entity.
    /// Tags are stored as lower-case.
    /// They must be unique (non case-sensitive) per-entity,
    /// but the same tag can exist across multiple entities.
    /// (Repeated tags are ignored, and not added to an entities tag list.)
    /// </summary>
    public List<string> Tags
    {
        get { return _tags; }
        private set
        {
            if (value == null)
                return;
            else
            {
                foreach (string tag in value)
                    AddTag(tag);
            }
        }
    }

    /// <summary>
    /// Sets the visibility of an entity.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Create a new entity.
    /// </summary>
    /// <param name="name">The unique name of the entity.</param>
    /// <param name="tags">A list of tags to identify and/or categorise the entity.</param>
    /// <param name="state">The state of the entity.</param>
    /// <param name="visible">The entity visibility.</param>
    /// <param name="owner">The parent of the entity.</param>
    /// <param name="components">A list of components to add to the entity.</param>
    public Entity(
        string? name = null,
        List<string>? tags = null,
        string state = "default",
        bool visible = true,
        Entity? owner = null,
        Component[]? components = null)
    {

        ID = _entityManager.CheckOutID();
        bitMask = new BitArray(_componentManager.maxComponents, false);
        
        Name = name;
        Tags = tags ?? new List<string>();
        _state = state;
        PreviousState = state;
        Visible = visible;
        Owner = owner;

        // Add components
        if (components != null)
            foreach (Component component in components)
                AddComponent(component);

        // Add the entity to the list of all entities        
        _entityManager.AddEntity(this);

    }

    //
    // Component methods
    //

    /// <summary>
    /// Checks whether the entity has a component of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the component to check for.</typeparam>
    /// <returns>A boolean indicating the presence of a component.</returns>
    public bool HasComponent<T>() where T : Component
    {
        return _componentManager.EntityHasComponentOfType<T>(this);
    }

    /// <summary>
    /// Adds one or more components to the entity,
    /// overwriting an existing component if one exists.
    /// </summary>
    /// <param name="components">One or more components to add.</param>
    public void AddComponent(params Component[] components)
    {
        foreach (Component component in components)
            _componentManager.AddComponentToEntity(this, component);
    }

    /// <summary>
    /// Returns the component of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the componet to retrieve.</typeparam>
    /// <returns>The requested component, or null.</returns>
    public T? GetComponent<T>() where T : Component
    {
        return _componentManager.GetComponentForEntity<T>(this);
    }

    /// <summary>
    /// Removes the component of the specified type (if one exists).
    /// </summary>
    /// <typeparam name="T">The type of the component to remove.</typeparam>
    public void RemoveComponent<T>() where T : Component
    {
        _componentManager.RemoveComponentFromEntity<T>(this);
    }

    //
    // Tag methods 
    //

    /// <summary>
    /// Adds one or more tags, stored in lower-case with duplicates removed.
    /// </summary>
    /// <param name="tags">One or more tags to add.</param>
    public void AddTag(params string[] tags)
    {
        // Adds one or more tags
        foreach (string tag in tags)
        {
            if (_tags.Contains(tag.ToLower()) == false)
                _tags.Add(tag.ToLower());
        }
    }

    /// <summary>
    /// Checks whether the entity has all of the tags specified.
    /// Tags are checked as lower-case.
    /// </summary>
    /// <param name="tags">One or more tags to check.</param>
    /// <returns>A boolean, indicating the preence of all specified tags.</returns>
    public bool HasTag(params string[] tags)
    {
        // Returns true if all tags are present
        foreach (string tag in tags)
        {
            if (_tags.Contains(tag.ToLower()) == false)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Removes the specified tags (if they exist).
    /// </summary>
    /// <param name="tags">One or more tags to remove.</param>
    public void RemoveTag(params string[] tags)
    {
        // Adds one or more tags 
        foreach (string tag in tags)
        {
            if (_tags.Contains(tag.ToLower()) == true)
                _tags.Remove(tag.ToLower());
        }
    }

    //
    // ToString override 
    //

    /// <summary>
    /// Debug entity string print method.
    /// </summary>
    /// <returns>A string output representing the entity.</returns>
    public override string ToString()
    {
        string output = Theme.CreateConsoleTitle("Entity");
        output += Theme.PrintConsoleVar("ID", ID.ToString());
        string n = "[none]";
        if (Name != null)
            n = Name;
        output += Theme.PrintConsoleVar("Name", n);
        string tagListOutput = Utils.PrintList(Tags, separator: ", ");
        output += Theme.PrintConsoleVar("Tags", tagListOutput);
        output += Theme.PrintConsoleVar("State", State);
        output += Theme.PrintConsoleVar("Prev. State", PreviousState);
        string ownerString = Owner.ID.ToString();
        if (ID == Owner.ID)
        {
            ownerString += " (self)";
        }
        output += Theme.PrintConsoleVar("Owner ID", ownerString);
        string bitMaskOutput = Utils.PrintBitArray(bitMask);
        output += Theme.PrintConsoleVar("BitMask", bitMaskOutput);
        output += Theme.PrintConsoleVar("To Delete", Delete.ToString());
        output += "\n";
        return output;
    }

}