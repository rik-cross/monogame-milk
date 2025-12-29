//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

namespace milk.Core;

// TODO: OnComplete

/// <summary>
/// Runs the specified action after an amount of elapsed scene time.
/// </summary>
internal class TimedAction
{

    /// <summary>
    /// Amount of elapsed scene time before executing action.
    /// </summary>
    internal readonly float ElapsedTime;

    /// <summary>
    /// The action to execute after the elapsed time.
    /// </summary>
    internal readonly Action? Action;

    /// <summary>
    /// The name of the action, for finding and deleting later.
    /// </summary>
    internal readonly string? Name;

    /// <summary>
    /// Create a new timed action.
    /// </summary>
    /// <param name="elapsedTime">The amount of elapsed time (default = 0 - instant).</param>
    /// <param name="action">The action callback to execute (default = null).</param>
    /// <param name="name">The name of the timed action, to allow finding and deleting later (default = null).</param>
    internal TimedAction(float elapsedTime = 0f, Action? action = null, string? name = null)
    {
        ElapsedTime = elapsedTime;
        Action = action;
        Name = name;
    }

    internal void Execute()
    {
        if (Action != null)
            Action();
    }

}