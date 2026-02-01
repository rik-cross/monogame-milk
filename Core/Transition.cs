//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Core;

/// <summary>
/// Transition base class. Not to be instantiated directly,
/// but new transitions should subclass this base class.
/// </summary>
public abstract class Transition {
    public float elapsedDuration;
    public float totalDuration;
    public float percentageComplete;
    public float easedPercentage;
    public bool finished = false;
    public EasingFunctions.EasingDelegate? easing;

    protected internal Transition(
        float duration = 1.0f,
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

    internal void _Draw(
        GraphicsDevice graphicsDevice,
        RenderTarget2D existingScenesRenderTarget,
        RenderTarget2D newScenesRenderTarget,
        List<Scene> currentScenes,
        List<Scene> toScenes
    )
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
            if (Milk.Scenes._currentSceneList.Count > Milk.Scenes.numberOfScenesToRemove)
                Milk.Scenes._currentSceneList[Milk.Scenes.numberOfScenesToRemove]._Draw(newScenesRenderTarget, currentScenes);
        }

        graphicsDevice.SetRenderTarget(null);
        
        Milk.Graphics.Begin();
        Draw(existingScenesRenderTarget, newScenesRenderTarget);
        Milk.Graphics.End();

    }

    internal void _Update(GameTime gameTime, List<Scene> toScenes, bool unload)
    {
        percentageComplete = elapsedDuration / totalDuration;
        easedPercentage = easing(percentageComplete);
        elapsedDuration += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (percentageComplete >= 1)
        {
            finished = true;
            if (toScenes.Count > 0)
                Milk.Scenes.SetScene(toScenes, null, !unload, true);
            else
                Milk.Scenes.RemoveScene(null, Milk.Scenes.numberOfScenesToRemove, true);
        }
    }

    protected internal abstract void Draw(RenderTarget2D existingScenesRenderTarget, RenderTarget2D newScenesRenderTarget);

}