//   Monogame ECS Engine, By Rik Cross
//   -- github.com/rik-cross/monogame-ecs
//   -- Shared under the MIT licence

using System;
using System.Collections;

namespace milk;

/// <summary>
/// An Entity is...
/// </summary>
public class Entity
{
    private EntityManager _entityManager = EngineGlobals.game.entityManager;

    private ComponentManager _componentManager = EngineGlobals.game.componentManager;

    public readonly int ID;

    // Entity names are [null] by default,
    // but must otherwise be unique
    private string _name = null;
    public string Name
    {
        get { return _name; }
        set
        {
            if (value == null)
                _name = value;
            else
            {
                bool uniqueName = true;
                foreach (Entity entity in _entityManager._allEntities)
                {
                    if (entity.Name.ToLower() == value.ToLower())
                    {
                        uniqueName = false;
                        break;
                    }
                }
                if (uniqueName == true)
                    _name = value;
                else
                    _name = value + ID.ToString(); // should be more unique
            }
        }
    }

    // The state of an entity can be any string
    // and allows entity behaviour to be determined
    // by the state. Setting the state stores the
    // previous state, to allow for comparison
    private string _state = "default";
    public string _previousState = "default";
    public string State
    {
        get { return _state; }
        set
        {
            _previousState = _state;
            _state = value;
        }
    }

    // The entity owner
    private Entity _owner = null;
    public Entity Owner
    {
        get { return _owner; }
        set { _owner = (value == null) ? this : value; }
    }

    // Setting Delete to true on an entity marks the entity
    // for deletion, so that it can be deleted after
    // processing a scene
    public bool Delete { get; set; }

    // A bit array indicating which components
    // the entity contains. The position of each
    // bit in the array is the ID of the associated
    // component 
    public BitArray bitMask;

    // A list of non-unique identifiers, used to
    // group and classify entities
    // Tags are stored as lower case
    private List<string> _tags = new List<string>();
    public List<string> Tags
    {
        get { return _tags; }
        private set
        {
            if (value == default)
                return;
            else
            {
                foreach (string tag in value)
                    AddTag(tag);
            }
        }
    }

    public Entity(
        string name = null,
        List<string> tags = default,
        string state = "default",
        Entity owner = null,
        Component[] components = default)
    {

        ID = _entityManager.CheckOutID();
        Name = name;
        State = state;
        Owner = owner;
        Tags = tags;
        bitMask = new BitArray(_componentManager.maxComponents, false);

        // Add components
        if (components != default)
            foreach (Component component in components)
                AddComponent(component);

        // Add the entity to the list of all entities        
        _entityManager.AddEntity(this);

    }

    //
    // Component methods
    //

    public bool HasComponent<T>() where T : Component
    {
        return _componentManager.EntityHasComponentOfType<T>(this);
    }

    public void AddComponent(Component c)
    {
        _componentManager.AddComponentToEntity(this, c);
    }

    public T GetComponent<T>() where T : Component
    {
        return _componentManager.GetComponentForEntity<T>(this);
    }

    public void RemoveComponent<T>() where T : Component
    {
        _componentManager.RemoveComponentFromEntity<T>(this);
    }

    //
    // Tag methods 
    //

    public void AddTag(params string[] tags)
    {
        // Adds one or more tags
        foreach (string tag in tags)
        {
            if (_tags.Contains(tag.ToLower()) == false)
                _tags.Add(tag.ToLower());
        }
    }

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
        output += Theme.PrintConsoleVar("Prev. State", _previousState);
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