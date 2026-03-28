//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;

namespace milk.Core;

public abstract partial class Scene
{
    
    // Generic method to add an element to a list,
    // ensuring no duplicate names (where names are specified).
    protected void AddElementToCollection<T>(List<T> list, T element)
        where T : class, INameable, ISceneParent
    {
        // Don't add the same object more than once
        if (list.Contains(element))
            return;

        // Don't add if a marker with the same name already exists
        if (element.Name != null && GetCollectionElementByName(list, element.Name) != null) 
            return;

        // Add the marker and set the parent scene
        element.ParentScene = this;
        list.Add(element);
    }

    protected T? GetCollectionElementByName<T>(List<T> list, string name)
        where T : class, INameable
    {
        // Only search with a valid name
        if (string.IsNullOrWhiteSpace(name)) 
            return null;
        
        // Compare trimmed, lowercase names only
        string? searchString = name.Trim().ToLower();

        // Check each existing marker, and return the
        // marker found with the specified name
        foreach (T element in list)
        {
            if (element.Name == searchString)
                return element;
        }

        // No marker with the specified name found
        return null;
    }

    protected void SetCollectionVisibility<T>(List<T> list, bool visibility, string? name = null)
        where T : class, INameable, IVisible
    {
        // Compare trimmed, lowercase names only
        string? searchString = name?.Trim().ToLower();

        // Iterate over all markers
        foreach (T element in list)
        {
            // Hide the marker if the names match,
            // or if no name is specified
            if (searchString == null || element.Name == searchString)
            {
                element.Visible = visibility;

                // As names are unique, if one marker is hidden
                // there is no more work to do
                if (searchString != null)
                    return;
            }    
        }
    }

    protected void RemoveElementFromCollection<T>(List<T> list, string? name = null)
        where T : class, INameable
    {
        // If no name is specified, then remove all markers
        if (name == null)
        {
            list.Clear();
            return;
        }

        // Compare trimmed, lowercase names only
        string searchString = name.Trim().ToLower();

        // Loop backwards over the marker list, so that removing
        // a marker doesn't impact later iterations of the loop
        for (int i = list.Count - 1; i >= 0; i--)
        {
            // Remove the marker is the names match,
            // or if no name is specified
            if (searchString == null || list[i].Name == searchString)
            {
                // Remove the marker
                list.RemoveAt(i);

                // As names are unique, if one marker is removed
                // there is no more work to do
                return;
            }    
        }
    }

    protected void UpdateCollection<T>(List<T> list, GameTime gameTime)
        where T : IUpdateable
    {
        foreach(T element in list)
            element.Update(gameTime);
    }

    protected void DrawCollection<T>(List<T> list)
        where T : class, IDrawable, IVisible
    {
        foreach(T element in list)
        {
            if (element.Visible == true)
                element.Draw();
        }
    }

}