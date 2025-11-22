//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: github.com/rik-cross/milk-docs
//   -- Shared under the MIT licence

namespace milk.Core;

/// <summary>
/// A component stores data for systems to use,
/// and has some useful additional callback methods.
/// </summary>
public abstract class Component
{

    //
    // Optional callback methods
    //

    /// <summary>
    /// This method is called automatically whenever a component is added to an entity.
    /// </summary>
    /// <param name="entity">The entity that the component has been added to.</param>
    public virtual void OnAddedToEntity(Entity entity) { }

    /// <summary>
    /// This method is called automatically whenever a component is removed from an entity.
    /// </summary>
    /// <param name="entity">The entity that the component has been added to.</param>
    public virtual void OnRemovedFromEntity(Entity entity) { }

    /// <summary>
    /// Debug entity string print method.
    /// </summary>
    /// <returns>A string output representing the entity.</returns>
    public override string ToString()
    {
        string output = "";
        output += Theme.CreateConsoleTitle("Component");
        output += Theme.PrintConsoleVar("ID", EngineGlobals.game.componentManager.GetComponentTypeID(this.GetType()).ToString());
        output = output.Remove(output.Length - 1);
        return output;
    }

}