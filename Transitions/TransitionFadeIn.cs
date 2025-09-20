using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameECS.Transitions;

public class TransitionFadeIn : Transition
{
    
    public TransitionFadeIn(
        float duration = 1000,
        EasingFunctions.EasingDelegate? easingFunction = null
    ) : base(duration, easingFunction) { }


    //public override void Update(GameTime gameTime) { }

    public override void Draw(RenderTarget2D existingScenesRenderTarget, RenderTarget2D newScenesRenderTarget)
    {

        // this should be in the update
        int w = (int)EngineGlobals.game._size.X;// graphicsDevice.PresentationParameters.BackBufferWidth;
        int h = (int)EngineGlobals.game._size.Y;// graphicsDevice.PresentationParameters.BackBufferHeight;
        float p = easedPercentage; // not percentage

        spriteBatch.Draw(existingScenesRenderTarget, Vector2.Zero, Color.White);
        spriteBatch.Draw(newScenesRenderTarget, Vector2.Zero, Color.White * (p));
    }
}