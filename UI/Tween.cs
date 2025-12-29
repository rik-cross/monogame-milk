using Microsoft.Xna.Framework;
using milk.Core;

namespace milk.UI;

/// <summary>
/// A tween is added to a scene's animator, and links an action to a duration.
/// </summary>
public class Tween
{
    
    // TODO: OnComplete

    /// <summary>
    /// The animation duration, in seconds.
    /// </summary>
    public readonly float Duration;

    /// <summary>
    /// An animation action.
    /// </summary>
    public Action<float> Action;

    /// <summary>
    /// An easing function for the animation.
    /// </summary>
    public readonly EasingFunctions.EasingDelegate EasingFunction;
    
    /// <summary>
    /// Is set to true once the animation has finished.
    /// </summary>
    public bool Finished;

    private float elapsedTime = 0;


    /// <summary>
    /// Creates a new tween object.
    /// </summary>
    /// <param name="action">The animation action.</param>
    /// <param name="duration">The length of the animation, in seconds.</param>
    /// <param name="easingFunction">The animation easing (default = null - linear).</param>
    public Tween(Action<float> action, float duration, EasingFunctions.EasingDelegate? easingFunction = null)
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