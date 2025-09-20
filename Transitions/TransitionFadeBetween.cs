using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameECS.Transitions;

public class TransitionFadeBetween : Transition
{
    
    public TransitionFadeBetween(
        float duration = 1000,
        EasingFunctions.EasingDelegate? easingFunction = null
    ) : base(duration, easingFunction) { }

    public override void Draw(RenderTarget2D existingScenesRenderTarget, RenderTarget2D newScenesRenderTarget)
    {

        int w = (int)EngineGlobals.game._size.X;// graphicsDevice.PresentationParameters.BackBufferWidth;
        int h = (int)EngineGlobals.game._size.Y;// graphicsDevice.PresentationParameters.BackBufferHeight;
        float p = easedPercentage; // not percentage

        spriteBatch.Draw(existingScenesRenderTarget, Vector2.Zero, Color.White * (1 - p));
        spriteBatch.Draw(newScenesRenderTarget, Vector2.Zero, Color.White * (p));
    }
}