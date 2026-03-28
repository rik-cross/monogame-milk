//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using milk.UI;

namespace milk.Core;

public abstract partial class Scene
{
    
    /// <summary>
    /// Scene tween list.
    /// </summary>
    public List<Tween> Tweens { get; set; } = new List<Tween>();

    /// <summary>
    /// Adds a scene tween, if its name is unique (or null).
    /// </summary>
    /// <param name="tween">The tween object to add.</param>
    public void AddTween(Tween tween) => AddElementToCollection<Tween>(
        list: Tweens, element: tween
    );

    /// <summary>
    /// Gets a tween with the specified name.
    /// </summary>
    /// <param name="name">The name of the tween to get.</param>
    /// <returns>A tween with the specified name, or null if no tween exists.</returns>
    public Tween? GetTweenByName(string name) => GetCollectionElementByName<Tween>(
        list: Tweens, name: name
    );

    /// <summary>
    /// Removes the tween with the specified name,
    /// or all tweens if no name is specified.
    /// </summary>
    /// <param name="name">Optional: The tween name to remove.</param>
    public void RemoveTween(string? name = null) => RemoveElementFromCollection<Tween>(
        list: Tweens, name: name
    );

    /// <summary>
    /// Iterates over all elements and runs their `Update()` method.
    /// </summary>
    /// <param name="gameTime">The MonoGame GameTime object.</param>
    public void UpdateTweens(GameTime gameTime) => UpdateCollection<Tween>(Tweens, gameTime);

}