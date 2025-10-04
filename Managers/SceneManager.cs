using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk;

public class SceneManager
{
    // should this store one of each type of scene?
    // and then use LoadScene<>() etc...
    public List<Scene> allScenes = new List<Scene>();
    public List<Scene> _currentSceneList = new List<Scene>();
    public List<Scene> _newSceneList = new List<Scene>();
    public int numberOfScenesToRemove = 1;
    public bool unload = true;


    public Transition currentTransition = null;
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

    public void Update(GameTime gameTime)
    {

        if (_currentSceneList.Count > 0)
            _currentSceneList[0]._Update(gameTime, _currentSceneList);

        // if there's a transition then process it
        if (currentTransition != null)
        {
            currentTransition._Update(gameTime, _newSceneList, unload);
            if (currentTransition.finished)
                currentTransition = null;
        }

        if (currentTransition != null && _newSceneList != null && _newSceneList.Count > 0)
            _newSceneList[0]._Update(gameTime, _newSceneList);

    }

    public void Input(GameTime gameTime)
    {
        // Cann the Input method for the top scene on the stack
        if (_currentSceneList.Count > 0)
            _currentSceneList[0]._Input(gameTime, _currentSceneList);
    }

    public void Draw()
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
            currentTransition._Draw(existingScenesRenderTarget, newScenesRenderTarget, _currentSceneList, _newSceneList);

    }

    internal void SetScene(List<Scene> sceneList, Transition transition = null, bool keepExistingScenes = false)
    {

        if (sceneList.Count == 0)
            return;

        if (transition == null)
        {
            if (_currentSceneList.Count > 0)
                _currentSceneList[0].OnExit();

            if (keepExistingScenes == false)
                _currentSceneList.Clear();

            for (int i = sceneList.Count - 1; i >= 0; i--)
                _currentSceneList.Insert(0, sceneList[i]);

            _currentSceneList[0].OnEnter();
        }
        else
        {
            currentTransition = transition;
            _newSceneList = sceneList;
            unload = !keepExistingScenes;
            numberOfScenesToRemove = 1;
        }

    }

    public void RemoveScene(Transition transition = null, int nScenesToRemove = 1)
    {
        _newSceneList.Clear();
        if (transition == null)
        {
            while (_currentSceneList.Count > 0 && nScenesToRemove > 0)
            {
                _currentSceneList[0].OnExit();
                _currentSceneList.RemoveAt(0);
                nScenesToRemove--;
            }
            if (_currentSceneList.Count > 0)
                _currentSceneList[0].OnEnter();
        }
        else
        {
            currentTransition = transition;
            unload = false;
            this.numberOfScenesToRemove = nScenesToRemove;
        }
    }

    public void ClearScenes()
    {
        while (_currentSceneList.Count > 0)
        {
            _currentSceneList[0].OnExit();
            _currentSceneList.RemoveAt(0);
        }
    }

    public override string ToString()
    {
        string output = Theme.CreateConsoleTitle("SceneManager");
        output += Theme.PrintConsoleVar("Active scenes", _currentSceneList.Count.ToString());
        string transition = "False";
        if (currentTransition != null)
        {
            transition = "True";
        }
        output += Theme.PrintConsoleVar("Transition", transition);
        return output;
    }

}