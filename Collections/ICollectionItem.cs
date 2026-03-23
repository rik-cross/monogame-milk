//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

namespace milk.Core;

/// <summary>
/// Specifies an interface for a collection of items,
/// which is used for the Collection class. This allows
/// the interface to Get/Remove by name, and to set
/// item visibility.
/// </summary>
public interface ICollectionItem
{

    /// <summary>
    /// The name of the item.
    /// </summary>
    string? Name { get; set; }

    /// <summary>
    /// The visibility of the item.
    /// </summary>
    bool Visible { get; set; }

}