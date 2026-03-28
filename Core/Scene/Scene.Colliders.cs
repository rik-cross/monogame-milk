//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

namespace milk.Core;

public abstract partial class Scene
{
    
    /// <summary>
    /// Scene collider list.
    /// </summary>
    public List<SceneCollider> SceneColliders = new List<SceneCollider>();

    /// <summary>
    /// Adds a collider object to a scene.
    /// </summary>
    /// <param name="collider">The collider to add to the scene.</param>
    public void AddSceneCollider(SceneCollider collider) => AddElementToCollection<SceneCollider>(
        list: SceneColliders, element: collider
    );

    /// <summary>
    /// Returns a collider with the specified (unique) name.
    /// The name is checked in lower-case.
    /// </summary>
    /// <param name="name">The name of the collider to find.</param>
    /// <returns>A collider, or null if no collider exists with the specified name.</returns>
    public SceneCollider? GetSceneColliderByName(string name) => GetCollectionElementByName<SceneCollider>(
        list: SceneColliders, name: name
    );

    /// <summary>
    /// Removes the collider with the specified name,
    /// or all collider if no name is specified.
    /// </summary>
    /// <param name="name">Optional: The collider name to remove.</param>
    public void RemoveSceneCollider(string? name = null) => RemoveElementFromCollection<SceneCollider>(
        list: SceneColliders, name: name
    );

}