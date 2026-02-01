//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using milk.Core;
using MonoGame.Extended;
namespace milk.Transitions;

/// <summary>
/// Fades to black between scenes.
/// </summary>
public class TransitionFadeToBlack : Transition
{

    /// <summary>
    /// Creates a new transition to fade between scenes.
    /// </summary>
    /// <param name="duration">The duration of the transition, in seconds (default = 1s).</param>
    /// <param name="easingFunction">The easing function for the transition (default = null - linear)</param>    
    public TransitionFadeToBlack(
        float duration = 1.0f,
        EasingFunctions.EasingDelegate? easingFunction = null
    ) : base(duration, easingFunction) { }

    protected internal override void Draw(RenderTarget2D existingScenesRenderTarget, RenderTarget2D newScenesRenderTarget)
    {
        if (easedPercentage < 0.5f)
            Milk.Graphics.Draw(existingScenesRenderTarget, Vector2.Zero, Color.White);
        else
            Milk.Graphics.Draw(newScenesRenderTarget, Vector2.Zero, Color.White);

        double alpha = 1 - Math.Abs(easedPercentage * 2 - 1);
        Milk.Graphics.FillRectangle(
            new Rectangle(
                0,
                0,
                // TODO: where is the game size stored?
                (int)Milk.Size.X,
                (int)Milk.Size.Y
            ),
            Color.Black * (float)alpha
        );
    }
}