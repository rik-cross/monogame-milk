//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;

namespace milk.Core;

public abstract partial class Scene
{
    
    /// <summary>
    /// Scene marker list.
    /// </summary>
    public List<Marker> Markers = new List<Marker>();

    /// <summary>
    /// Adds a scene marker, if its name is unique (or null).
    /// </summary>
    /// <param name="marker">The marker object to add.</param>
    public void AddMarker(Marker marker) => AddElementToCollection<Marker>(
        list: Markers, element: marker
    );

    /// <summary>
    /// Gets a marker with the specified name.
    /// </summary>
    /// <param name="name">The name of the marker to get.</param>
    /// <returns>A marker with the specified name, or null if no marker exists.</returns>
    public Marker? GetMarkerByName(string name) => GetCollectionElementByName<Marker>(
        list: Markers, name: name
    );

    /// <summary>
    /// Shows the marker with the specified name,
    /// or all markers if no name is specified.
    /// </summary>
    /// <param name="name">Optional: The marker name to show.</param>
    public void ShowMarker(string? name = null) => SetCollectionVisibility<Marker>(
        list: Markers, visibility: true, name: name
    );

    /// <summary>
    /// Hides the marker with the specified name,
    /// or all markers if no name is specified.
    /// </summary>
    /// <param name="name">Optional: The marker name to hide.</param>
    public void HideMarker(string? name = null) => SetCollectionVisibility<Marker>(
        list: Markers, visibility: false, name: name
    );

    /// <summary>
    /// Removes the marker with the specified name,
    /// or all markers if no name is specified.
    /// </summary>
    /// <param name="name">Optional: The marker name to remove.</param>
    public void RemoveMarker(string? name = null) => RemoveElementFromCollection<Marker>(
        list: Markers, name: name
    );

    /// <summary>
    /// Iterates over all elements and runs their `Update()` method.
    /// </summary>
    /// <param name="gameTime">The MonoGame GameTime object.</param>
    public void UpdateMarkers(GameTime gameTime) => UpdateCollection<Marker>(Markers, gameTime);
    
    /// <summary>
    /// Iterates over all elements and runs their `Draw()` method.
    /// </summary>
    public void DrawMarkers() => DrawCollection<Marker>(Markers);
}