//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

namespace milk.Core;

/// <summary>
/// A component stores data for systems to use,
/// and has some useful additional callback methods.
/// </summary>
public abstract class Component
{

    /// <summary>
    /// The parent entity owner of the component
    /// </summary>
    public Entity? ParentEntity { get; set; }

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

}