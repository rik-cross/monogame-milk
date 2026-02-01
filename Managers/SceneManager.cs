//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Core;

public class SceneManager
{
    // should this store one of each type of scene?
    // and then use LoadScene<>() etc...
    internal List<Scene> allScenes = new List<Scene>();
    internal List<Scene> _currentSceneList = new List<Scene>();
    internal List<Scene> _newSceneList = new List<Scene>();
    internal int numberOfScenesToRemove = 1;
    internal bool unload = true;


    internal Transition? currentTransition = null;
    private RenderTarget2D existingScenesRenderTarget = new RenderTarget2D(
        EngineGlobals.game.graphicsDevice,
        EngineGlobals.game.graphicsDevice.PresentationParameters.BackBufferWidth,
        EngineGlobals.game.graphicsDevice.PresentationParameters.BackBufferHeight,
        false,
        EngineGlobals.game.graphicsDevice.PresentationParameters.BackBufferFormat,
        DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents
    );
    private RenderTarget2D newScenesRenderTarget = new RenderTarget2D(
        EngineGlobals.game.graphicsDevice,
        EngineGlobals.game.graphicsDevice.PresentationParameters.BackBufferWidth,
        EngineGlobals.game.graphicsDevice.PresentationParameters.BackBufferHeight,
        false,
        EngineGlobals.game.graphicsDevice.PresentationParameters.BackBufferFormat,
        DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents
    );

    private SpriteBatch spriteBatch = EngineGlobals.game.spriteBatch;
    private GraphicsDevice graphicsDevice = EngineGlobals.game.graphicsDevice;

    public void RemoveEntityFromAllScenes(Entity entity)
    {
        foreach (Scene scene in allScenes)
            scene.RemoveEntity(entity);
    }

    internal void Update(GameTime gameTime)
    {

        if (_currentSceneList.Count > 0)
            _currentSceneList[0]._Update(gameTime, _currentSceneList);

        // if there's a transition then process it
        if (currentTransition != null)
        {
            currentTransition._Update(gameTime, _newSceneList, unload);
            if (currentTransition.finished)
            {
                currentTransition = null;
            }
        }

        if (currentTransition != null && _newSceneList != null && _newSceneList.Count > 0)
            _newSceneList[0]._Update(gameTime, _newSceneList);

    }

    internal void Input(GameTime gameTime)
    {

        //if (currentTransition != null)
        //    return;

        // Call the Input method for the top scene on the stack
        if (_currentSceneList.Count > 0)
            _currentSceneList[0]._Input(gameTime, _currentSceneList);
    }

    internal void Draw()
    {

        // Clear the main (null) render target
        EngineGlobals.game.graphicsDevice.SetRenderTarget(null);
        EngineGlobals.game.graphicsDevice.Clear(Color.Black); //CornflowerBlue);


        // Clear the render target for drawing current scenes
        EngineGlobals.game.graphicsDevice.SetRenderTarget(existingScenesRenderTarget);
        EngineGlobals.game.graphicsDevice.Clear(Color.Transparent);

        // Draw current scene(s) if there are any
        if (_currentSceneList.Count > 0)
            _currentSceneList[0]._Draw(existingScenesRenderTarget, _currentSceneList);

        // If there's no active transition, just draw the scenes
        if (currentTransition == null)
        {
            graphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            spriteBatch.Draw(existingScenesRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
        // If there's an active transition, defer drawing to the transition
        else
            currentTransition._Draw(
                graphicsDevice,
                existingScenesRenderTarget,
                newScenesRenderTarget,
                _currentSceneList,
                _newSceneList
            );

    }

    public Scene? GetCurrentScene()
    {
        if (_currentSceneList.Count == 0)
            return null;
        return _currentSceneList[0];
    }

    public void SetScene(
        Scene scene,
        Transition? transition = null,
        bool keepExistingScenes = false,
        bool calledFromTransition = false
    ) => SetScene([scene], transition, keepExistingScenes, calledFromTransition);

    public void SetScene(
        List<Scene> sceneList,
        Transition? transition = null,
        bool keepExistingScenes = false,
        bool calledFromTransition = false
    )
    {

        if (sceneList.Count == 0)
            return;

        if (transition == null)
        {
            if (_currentSceneList.Count > 0)
            {

                foreach (System system in _currentSceneList[0].systems)
                {
                    system.OnExitScene(_currentSceneList[0]);
                }

                if (calledFromTransition == false)
                    _currentSceneList[0].OnExit();
            }

            if (keepExistingScenes == false)
                _currentSceneList.Clear();

            for (int i = sceneList.Count - 1; i >= 0; i--)
                _currentSceneList.Insert(0, sceneList[i]);

            foreach (System system in _currentSceneList[0].systems)
            {
                system.OnEnterScene(_currentSceneList[0]);
            }

            _currentSceneList[0].OnEnter();

        }
        else
        {
            currentTransition = transition;

            if (_currentSceneList.Count > 0)
                _currentSceneList[0].OnExit();
            
            _newSceneList = sceneList;
            unload = !keepExistingScenes;
            numberOfScenesToRemove = 1;

        }

        

    }

    public void RemoveScene(
        Transition? transition = null,
        int nScenesToRemove = 1,
        bool calledFromTransition = false
    )
    {
        _newSceneList.Clear();
        if (transition == null)
        {
            while (_currentSceneList.Count > 0 && nScenesToRemove > 0)
            {

                // systems.onExit(scene)
                foreach (System system in _currentSceneList[0].systems)
                {
                    system.OnExitScene(_currentSceneList[0]);
                }

                //Console.WriteLine("Exit");
                if (calledFromTransition == false)
                    _currentSceneList[0].OnExit();

                _currentSceneList.RemoveAt(0);
                nScenesToRemove--;
            }
            if (_currentSceneList.Count > 0)
            {

                foreach (System system in _currentSceneList[0].systems)
                {
                    system.OnEnterScene(_currentSceneList[0]);
                }

                // ??
                //if (calledFromTransition == false) {
                    //Console.WriteLine("Enter -- no transition");
                    _currentSceneList[0].OnEnter();
                //}
            } 
                
        }
        else
        {
            currentTransition = transition;

            _currentSceneList[0].OnExit();
            //_currentSceneList[nScenesToRemove].OnEnter();

            unload = false;
            this.numberOfScenesToRemove = nScenesToRemove;
        }
    }

    public void ClearScenes()
    {
        while (_currentSceneList.Count > 0)
        {

            foreach(System system in _currentSceneList[0].systems)
            {
                system.OnExitScene(_currentSceneList[0]);
            }

            _currentSceneList[0].OnExit();
            _currentSceneList.RemoveAt(0);
        }
    }

}