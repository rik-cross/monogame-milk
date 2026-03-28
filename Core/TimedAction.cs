//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;

namespace milk.Core;

/// <summary>
/// Runs the specified action after an amount of elapsed scene time.
/// </summary>
public class TimedAction : ISceneParent, INameable, IUpdateable
{

    public Scene ParentScene { get; set; }

    /// <summary>
    /// The time the timed action was created.
    /// </summary>
    internal readonly float StartTime;

    /// <summary>
    /// Amount of elapsed scene time before executing action.
    /// </summary>
    internal readonly float ElapsedTime;

    /// <summary>
    /// The action to execute after the elapsed time.
    /// </summary>
    internal readonly Action? Action;

        private string? _name;
    /// <summary>
    /// The name of the timed action, unique to a scene.
    /// Names are stored trimmed and in lowercase.
    /// </summary>
    public string? Name
    {
        get
        { return _name; }
        set
        {
            if (value == null)
                _name = value;
            else
                _name = value.Trim().ToLower();
        }
    }

    /// <summary>
    /// Create a new timed action.
    /// </summary>
    /// <param name="elapsedTime">The amount of elapsed time in seconds (default = 0 - instant).</param>
    /// <param name="action">The action callback to execute (default = null).</param>
    /// <param name="name">The name of the timed action, to allow finding and deleting later (default = null).</param>
    /// <param name="startTimerFromNow">Decide whether elapsed time starts from the start of the game or the creation of the timed action (default = true - start from creation).</param>
   internal TimedAction(
        float elapsedTime = 0f,
        Action? action = null,
        string? name = null,
        bool startTimerFromNow = true
    )
    {
        ElapsedTime = elapsedTime;
        Action = action;
        Name = name;
        StartTime = startTimerFromNow == true ? (float)Milk.TotalGameTime : 0; 
    }

    internal void Execute()
    {
        if (Action != null)
            Action();
    }

    public void Update(GameTime gameTime)
    {
        
    }

}