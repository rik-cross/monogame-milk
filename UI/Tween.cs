//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using milk.Core;

namespace milk.UI;

/// <summary>
/// A tween is added to a scene's animator, and links an action to a duration.
/// </summary>
internal class Tween
{
    
    // TODO: OnComplete

    /// <summary>
    /// The animation duration, in seconds.
    /// </summary>
    internal readonly float Duration;

    /// <summary>
    /// An animation action.
    /// </summary>
    internal Action<float> Action;

    /// <summary>
    /// An easing function for the animation.
    /// </summary>
    internal readonly EasingFunctions.EasingDelegate EasingFunction;
    
    /// <summary>
    /// Is set to true once the animation has finished.
    /// </summary>
    internal bool Finished;

    private float elapsedTime = 0;

    /// <summary>
    /// Creates a new tween object.
    /// </summary>
    /// <param name="action">The animation action.</param>
    /// <param name="duration">The length of the animation, in seconds.</param>
    /// <param name="easingFunction">The animation easing (default = null - linear).</param>
    internal Tween(
        Action<float> action,
        float duration,
        EasingFunctions.EasingDelegate? easingFunction = null)
    {
        Action = action;
        Duration = duration;
        EasingFunction = easingFunction ?? EasingFunctions.Linear;
    }

    internal void Update(GameTime gameTime)
    {
        
        elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        float progress = MathHelper.Clamp(elapsedTime / Duration, 0f, 1f);
        float easedProgress = EasingFunction(progress);

        if (Action != null)
            Action(easedProgress);

        if (elapsedTime >= Duration)
            Finished = true;

    }

}