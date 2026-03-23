//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;

namespace milk.Core;

/// <summary>
/// A collection of items, with methods for
/// adding, getting and removing items.
/// </summary>
/// <typeparam name="T">The type of item in the collection.</typeparam>
public class Collection<T> where T : ICollectionItem
{

    protected List<T> _items = new List<T>();
    
    //public IReadOnlyList<T> Items => _items;

    /// <summary>
    /// Adds a new item to the collection.
    /// </summary>
    /// <param name="itemToAdd">The item to add.</param>
    public void Add(T itemToAdd)
    {
        
        // Ensure no 2 items with the same name are added
        if (itemToAdd.Name != null)
        {
            foreach (T item in _items)
            {
                if (item.Name != null && item.Name.ToLower() == itemToAdd.Name.ToLower())
                    return;
            }            
        }

        // Add the item
        _items.Add(itemToAdd);

    }

    /// <summary>
    /// Returns the item with the specified name.
    /// </summary>
    /// <param name="name">The name of the item to return.</param>
    /// <returns>An item with the specified name if one exists, or null.</returns>
    public T? GetByName(string name)
    {

        // Find an item with a matching name
        foreach (T item in _items)
        {
            // Item found
            if (item.Name != null && item.Name?.ToLower() == name.ToLower())
                return item;
        }

        // No item found
        return default;

    }

    /// <summary>
    /// Removes an item with the specified name,
    /// or all items if no item name is specified.
    /// </summary>
    /// <param name="name">Optional: the name of the item to remove (default = null).</param>
    public void Remove(string? name = null)
    {

        string? searchName = name?.ToLower();

        // Loop backwards to safely remove while iterating
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            // If no name is provided OR the name matches, remove it
            if (name == null || (_items[i].Name != null && _items[i].Name!.ToLower() == searchName))
            {
                // Remove the item
                _items.RemoveAt(i);

                // If an item is found, then we are done
                // as names are unique
                if (name != null)
                    return;
            }
        }
    }

    /// <summary>
    /// Shows an item with the specified name,
    /// or all items if no item name is specified.
    /// </summary>
    /// <param name="name">Optional: the name of the item to show (default = null).</param>
    public void Show(string? name = null)
    {

        string? searchName = name?.ToLower();

        foreach (T item in _items)
        {
            // If no name is provided OR the name matches, set the visibility
            if (searchName == null || (item.Name != null && item.Name.ToLower() == searchName))
            {
                // Set item visibility
                item.Visible = true;

                // If an item is found, then we are done
                // as names are unique
                if (searchName != null)
                    return;
            }
        }
    }

    /// <summary>
    /// Hides an item with the specified name,
    /// or all items if no item name is specified.
    /// </summary>
    /// <param name="name">Optional: the name of the item to hide (default = null).</param>
    public void Hide(string? name = null)
    {

        string? searchName = name?.ToLower();

        foreach (T item in _items)
        {
            // If no name is provided OR the name matches, set the visibility
            if (searchName == null || (item.Name != null && item.Name.ToLower() == searchName))
            {
                // Set item visibility
                item.Visible = false;

                // If an item is found, then we are done
                // as names are unique
                if (searchName != null)
                    return;
            }
        }
    }

    //
    // Virtual methods for implementation by collecitons
    //

    internal virtual void Update(GameTime gameTime, Scene scene) { }
    internal virtual void Draw(Scene scene) { }

}