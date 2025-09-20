/*
   [ ] Finished
   
   Monogame ECS Engine
   By Rik Cross
   -- github.com/rik-cross/monogame-ecs
   Shared under the MIT licence

   ------------------------------------

   MonogameECS.Game
   ================
  
   A Game is a subclass of a MonoGame.Game, with
   some additional methods for managing scenes, systems, etc.
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoGameECS;

public class Game : Microsoft.Xna.Framework.Game
{
    // Init callback funtion, called just before the game runs
    public Action? Init;
    public readonly string _title;
    public readonly Vector2 _size;

    internal ContentManager content;
    internal GraphicsDeviceManager graphics;
    internal GraphicsDevice graphicsDevice;
    internal SpriteBatch spriteBatch;

    internal EntityManager entityManager;
    internal ComponentManager componentManager;
    internal SceneManager sceneManager;
    internal SystemManager systemManager;

    public Game(
        Vector2 size = default,
        string? title = "MonoGame ECS")
    {

        EngineGlobals.game = this;

        graphics = new GraphicsDeviceManager(this);
        content = Content;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Init = null;

        _title = title;
        if (size == default)
            _size = new Vector2(800, 480);
        else
            _size = size;
    }

    protected override void Initialize()
    {

        // Set the size used for scenes
        graphics.PreferredBackBufferWidth = (int)_size.X;
        graphics.PreferredBackBufferHeight = (int)_size.Y;
        graphics.ApplyChanges();

        base.Initialize();
    }

    public void RegisterSystem(System system)
    {
        systemManager.RegisterSystem(system);
    }

    protected override void LoadContent()
    {

        //EngineGlobals.gameWindow = Window;
        Window.Title = _title;
        //EngineGlobals.content = Content;
        graphicsDevice = GraphicsDevice;
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // Ensure that render targets aren't cleared when swapping between them
        GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

        // Create 'manager' instances
        sceneManager = new SceneManager();
        componentManager = new ComponentManager(maxComponents: 4, maxEntities: 4);
        systemManager = new SystemManager();
        entityManager = new EntityManager(maxExtities: 4);

        // Register all provided systems
        RegisterSystem(new PhysicsSystem());
        RegisterSystem(new GraphicsSystem());
        RegisterSystem(new LightingSystem());

        // Run the Init callback if one has been specified
        if (Init != null)
            Init();

    }

    //
    // SCENES
    //

    public void SetScene(Scene scene, Transition transition = null, bool keepExistingScenes = false)
    {
        EngineGlobals.game.sceneManager.SetScene([scene], transition, keepExistingScenes);
    }

    public void SetScene(List<Scene> sceneList, Transition transition = null, bool keepExistingScenes = false)
    {
        EngineGlobals.game.sceneManager.SetScene(sceneList, transition, keepExistingScenes);
    }

    public void RemoveScene(Transition transition = null, int numberOfScenesToRemove = 1)
    {
        EngineGlobals.game.sceneManager.RemoveScene(transition, numberOfScenesToRemove);
    }

    public void ClearAllScenes()
    {
        EngineGlobals.game.sceneManager.ClearAllScenes();
    }

    protected override void Update(GameTime gameTime)
    {

        EngineGlobals.game.sceneManager.Input(gameTime);

        if (EngineGlobals.game.sceneManager._currentSceneList.Count == 0 && EngineGlobals.game.sceneManager.currentTransition == null)
            Exit();

        // Defer to the scene manager's update method
        EngineGlobals.game.sceneManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Defer to the scene manager's draw method
        EngineGlobals.game.sceneManager.Draw();
        //EngineGlobals.inputManager.Draw();
        base.Draw(gameTime);
    }

    public void Quit()
    {
        ClearAllScenes();
    }

}
