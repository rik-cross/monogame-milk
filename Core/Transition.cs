//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Core;

public abstract class Transition {
    public float elapsedDuration;
    public float totalDuration;
    public float percentageComplete;
    public float easedPercentage;
    public bool finished = false;
    public EasingFunctions.EasingDelegate? easing;
    public SpriteBatch spriteBatch = EngineGlobals.game.spriteBatch;
    public GraphicsDevice graphicsDevice = EngineGlobals.game.graphicsDevice;
    public Transition(
        float duration = 1000,
        EasingFunctions.EasingDelegate? easing = null)
    {
        totalDuration = duration;
        elapsedDuration = 0f;
        percentageComplete = 0f;
        easedPercentage = 0f;

        if (easing == null)
            this.easing = EasingFunctions.Linear;
        else
            this.easing = easing;
    }

    public void _Draw(RenderTarget2D existingScenesRenderTarget, RenderTarget2D newScenesRenderTarget, List<Scene> currentScenes, List<Scene> toScenes)
    {

        graphicsDevice.SetRenderTarget(newScenesRenderTarget);
        graphicsDevice.Clear(Color.Transparent);

        if (toScenes.Count > 0)
        {
            toScenes[0]._Draw(newScenesRenderTarget, toScenes); // this used to be currentScenes, which I think was wrong
        }

        if (toScenes.Count == 0)
        {
            graphicsDevice.Clear(Color.CornflowerBlue);
            if (EngineGlobals.game.sceneManager._currentSceneList.Count > EngineGlobals.game.sceneManager.numberOfScenesToRemove)
                EngineGlobals.game.sceneManager._currentSceneList[EngineGlobals.game.sceneManager.numberOfScenesToRemove]._Draw(newScenesRenderTarget, currentScenes);
        }

        graphicsDevice.SetRenderTarget(null);
        
        spriteBatch.Begin();
        Draw(existingScenesRenderTarget, newScenesRenderTarget);
        spriteBatch.End();

    }
    public void _Update(GameTime gameTime, List<Scene> toScenes, bool unload)
    {
        percentageComplete = elapsedDuration / totalDuration;
        easedPercentage = easing(percentageComplete);
        elapsedDuration += gameTime.ElapsedGameTime.Milliseconds;
        //Console.WriteLine(percentageComplete);
        if (percentageComplete >= 1)
        {
            finished = true;
            if (toScenes.Count > 0)
                EngineGlobals.game.sceneManager.SetScene(toScenes, null, !unload, true);
            else
                EngineGlobals.game.sceneManager.RemoveScene(null, EngineGlobals.game.sceneManager.numberOfScenesToRemove, true);
        }
        //Update(gameTime);
    }
    //public abstract void Update(GameTime gameTime);
    public abstract void Draw(RenderTarget2D existingScenesRenderTarget, RenderTarget2D newScenesRenderTarget);
}