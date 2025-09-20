using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameECS.Transitions;

public class TransitionFadeToBlack : Transition
{
    
    public TransitionFadeToBlack(
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

        if (p < 0.5f)
            spriteBatch.Draw(existingScenesRenderTarget, Vector2.Zero, Color.White);
        else
            // should be newSceneSSS
            spriteBatch.Draw(newScenesRenderTarget, Vector2.Zero, Color.White);

        double alpha = 1 - Math.Abs(p * 2 - 1);
        spriteBatch.Draw(Scene.whiteRectangle, new Rectangle(0, 0, (int)EngineGlobals.game._size.X, (int)EngineGlobals.game._size.Y), Color.Black * (float)alpha);

    }
}