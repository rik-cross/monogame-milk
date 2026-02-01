//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using milk.Core;
namespace milk.Transitions;

/// <summary>
/// Fades a new scene into the current scene.
/// </summary>
public class TransitionFadeIn : Transition
{
    
    /// <summary>
    /// Creates a new transition to fade between scenes.
    /// </summary>
    /// <param name="duration">The duration of the transition, in seconds (default = 1s).</param>
    /// <param name="easingFunction">The easing function for the transition (default = null - linear)</param>
    public TransitionFadeIn(
        float duration = 1.0f,
        EasingFunctions.EasingDelegate? easingFunction = null
    ) : base(duration, easingFunction) { }

    protected internal override void Draw(RenderTarget2D existingScenesRenderTarget, RenderTarget2D newScenesRenderTarget)
    {
        Milk.Graphics.Draw(existingScenesRenderTarget, Vector2.Zero, Color.White);
        Milk.Graphics.Draw(newScenesRenderTarget, Vector2.Zero, Color.White * (easedPercentage));
    }

}