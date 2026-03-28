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
    /// Scene timed actions.
    /// </summary>
    public List<TimedAction> TimedActions { get; set; } = new List<TimedAction>();

    /// <summary>
    /// Adds a scene timed action, if its name is unique (or null).
    /// </summary>
    /// <param name="timedAction">The timed action object to add.</param>
    public void AddTimedAction(TimedAction timedAction) => AddElementToCollection<TimedAction>(
        list: TimedActions, element: timedAction
    );

    /// <summary>
    /// Gets a timed action with the specified name.
    /// </summary>
    /// <param name="name">The name of the timed action to get.</param>
    /// <returns>A timed action with the specified name, or null if no tween exists.</returns>
    public TimedAction? GetTimedActionByName(string name) => GetCollectionElementByName<TimedAction>(
        list: TimedActions, name: name
    );

    /// <summary>
    /// Removes the timed action with the specified name,
    /// or all timed actions if no name is specified.
    /// </summary>
    /// <param name="name">Optional: The timed action name to remove.</param>
    public void RemoveTimedAction(string? name = null) => RemoveElementFromCollection<TimedAction>(
        list: TimedActions, name: name
    );

    /// <summary>
    /// Iterates over all elements and runs their `Update()` method.
    /// </summary>
    /// <param name="gameTime">The MonoGame GameTime object.</param>
    public void UpdateTimedActions(GameTime gameTime) => UpdateCollection<TimedAction>(TimedActions, gameTime);


}