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
    /// Scene camera list.
    /// </summary>
    public List<Camera> Cameras = new List<Camera>();

    /// <summary>
    /// Adds a camera object to a scene.
    /// </summary>
    /// <param name="camera">The camera to add to the scene.</param>
    public void AddCamera(Camera camera) => AddElementToCollection<Camera>(
        list: Cameras, element: camera
    );

    /// <summary>
    /// Returns a camera with the specified (unique) name.
    /// The name is checked in lower-case.
    /// </summary>
    /// <param name="name">The name of the camera to find.</param>
    /// <returns>A camera, or null if no camera exists with the specified name.</returns>
    public Camera? GetCameraByName(string name) => GetCollectionElementByName<Camera>(
        list: Cameras, name: name
    );

    /// <summary>
    /// Shows the camera with the specified name,
    /// or all camera if no name is specified.
    /// </summary>
    /// <param name="name">Optional: The camera name to show.</param>
    public void ShowCamera(string? name = null) => SetCollectionVisibility<Camera>(
        list: Cameras, visibility: true, name: name
    );

    /// <summary>
    /// Hides the camera with the specified name,
    /// or all camera if no name is specified.
    /// </summary>
    /// <param name="name">Optional: The camera name to hide.</param>
    public void HideCamera(string? name = null) => SetCollectionVisibility<Camera>(
        list: Cameras, visibility: false, name: name
    );

    /// <summary>
    /// Removes the camera with the specified name,
    /// or all cameras if no name is specified.
    /// </summary>
    /// <param name="name">Optional: The camera name to remove.</param>
    public void RemoveCamera(string? name = null) => RemoveElementFromCollection<Camera>(
        list: Cameras, name: name
    );

    /// <summary>
    /// Iterates over all elements and runs their `Update()` method.
    /// </summary>
    /// <param name="gameTime">The MonoGame GameTime object.</param>
    public void UpdateCameras(GameTime gameTime) => UpdateCollection<Camera>(Cameras, gameTime);

    /// <summary>
    /// Iterates over all elements and runs their `Draw()` method.
    /// </summary>
    public void DrawCameras() => DrawCollection<Camera>(Cameras);
    
}