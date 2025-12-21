//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using milk.Systems;

namespace milk.Core;

/// <summary>
/// A game is the top-level class, and is a subclass of
/// the MonoGame Game class.
/// </summary>
public class Game : Microsoft.Xna.Framework.Game
{

    /// <summary>
    /// The game title, displayed on the game window.
    /// </summary>
    public readonly string Title;

    /// <summary>
    /// The (x, y) size of the game window.
    /// </summary>
    public readonly Vector2 Size;

    /// <summary>
    /// A callback funtion, called just before the game loop runs.
    /// </summary>
    public Action Init;
    
    private readonly int _maxEntities;
    private readonly int _maxComponents;

    // MonoGame graphics helper instances 
    internal ContentManager content;
    internal GraphicsDeviceManager graphics;
    internal GraphicsDevice graphicsDevice;
    internal SpriteBatch spriteBatch;

    // Manager instances
    internal EntityManager entityManager;
    internal ComponentManager componentManager;
    internal SceneManager sceneManager;
    internal SystemManager systemManager;
    
    /// <summary>
    /// The game's input manager, used to query game input.
    /// </summary>
    public InputManager inputManager;

    // TODO
    // This is set to 'true' initially, and then set to 'false' after the first frame
    // It prevents the window looking 'glitchy' on the first game frame 
    private bool firstFrame;

    /// <summary>
    /// Setting debug mode to true displays various game, scene and entity information.
    /// </summary>
    public bool Debug = false;

    /// <summary>
    /// Creates a new game.
    /// </summary>
    /// <param name="title">Game window title (default = "milk").</param>
    /// <param name="size">Window size (default = (800, 480)).</param>
    /// <param name="isMouseVisible">Sets mouse visibility (default = true).</param>
    /// <param name="maxEntities">The maximum number of game entities (default = 1024).</param>
    /// <param name="maxComponents">The maximum number of game components (default = 128).</param>
    /// <param name="debug">Sets debug mode (default = false).</param>
    public Game(
        string? title = null,
        Vector2? size = null,
        bool isMouseVisible = true,
        int maxEntities = 1024,
        int maxComponents = 128,
        bool debug = false
    )
    {

        // TODO
        // I'm not sure of the best place to store and pass the main game object
        // An alternative might be to pass this around to other objects, but that feels clunky
        EngineGlobals.game = this;

        graphics = new GraphicsDeviceManager(this);
        content = Content;
        Content.RootDirectory = "Content";

        // Use arguments to set game properties
        Title = title ?? "milk";
        Size = size ?? new Vector2(800, 480);
        IsMouseVisible = isMouseVisible;
        _maxEntities = maxEntities;
        _maxComponents = maxComponents;

        //bool firstFrame = true;

        Debug = debug;

    }

    //
    // Overridden MonoGame.Game methods
    //

    /// <summary>
    /// Do not call this.
    /// </summary>
    protected override void Initialize()
    {
        // Set the window (and therefore future Scene) size
        graphics.PreferredBackBufferWidth = (int)Size.X;
        graphics.PreferredBackBufferHeight = (int)Size.Y;
        graphics.ApplyChanges();
        base.Initialize();
    }

    /// <summary>
    /// Do not call this.
    /// </summary>
    protected override void LoadContent()
    {

        //EngineGlobals.gameWindow = Window;
        Window.Title = Title;
        //EngineGlobals.content = Content;
        graphicsDevice = GraphicsDevice;
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // Ensure that render targets aren't cleared when swapping between them
        GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

        // Create 'manager' instances
        sceneManager = new SceneManager();
        componentManager = new ComponentManager(maxComponents: _maxComponents, maxEntities: _maxComponents);
        systemManager = new SystemManager();
        entityManager = new EntityManager(maxExtities: _maxEntities);
        inputManager = new InputManager();

        // Register all provided systems
        RegisterSystem(new InputSystem());
        RegisterSystem(new SpriteSystem());
        RegisterSystem(new TriggerSystem());
        RegisterSystem(new PhysicsSystem());

        // Run the Init callback if one has been specified
        if (Init != null)
            Init();

        // Set a default scene if the scene stack is empty
        if (sceneManager._currentSceneList.Count == 0)
            SetScene(new DefaultScene());

    }

    //
    // System methods
    //

    /// <summary>
    /// Registers a new game system.
    /// All systems must be registered before use.
    /// </summary>
    /// <param name="system">The system to register.</param>
    public void RegisterSystem(System system)
    {
        systemManager.RegisterSystem(system);
    }

    //
    // Scene methods
    //

    /// <summary>
    /// Sets the current active game scene.
    /// </summary>
    /// <param name="scene">The scene to set as the active scene.</param>
    /// <param name="transition">Specifies a transition between any current scenes and the newly-active scene (default = null).</param>
    /// <param name="keepExistingScenes">Indicates whether to keep existing scenes on the scene stack, or remove them (default = false - remove all other scenes).</param>
    public void SetScene(Scene scene, Transition? transition = null, bool keepExistingScenes = false)
    {
        EngineGlobals.game.sceneManager.SetScene([scene], transition, keepExistingScenes);
    }

    /// <summary>
    /// Adds multiple scenes to the active scene stack.
    /// </summary>
    /// <param name="scenes">A list of scenes to add to the scene stack.</param>
    /// <param name="transition">Specifies a transition between any current scenes and the newly-active scene (default = null).</param>
    /// <param name="keepExistingScenes">Indicates whether to keep existing scenes on the scene stack, or remove them (default = false - remove all other scenes).</param>
    public void SetScene(List<Scene> scenes, Transition? transition = null, bool keepExistingScenes = false)
    {
        EngineGlobals.game.sceneManager.SetScene(scenes, transition, keepExistingScenes);
    }

    /// <summary>
    /// Removes the top, active scene from the scene stack.
    /// </summary>
    /// <param name="transition">Specifies a transition between any current scenes and the newly-active scene (default = null).</param>
    /// <param name="numberOfScenesToRemove">The number of scenes to remove from the scene stack (default = 1).</param>
    public void RemoveScene(Transition? transition = null, int numberOfScenesToRemove = 1)
    {
        EngineGlobals.game.sceneManager.RemoveScene(transition, numberOfScenesToRemove);
    }

    /// <summary>
    /// Removes all scenes from the scene stack
    /// (note: this will end the game loop).
    /// </summary>
    public void ClearScenes()
    {
        EngineGlobals.game.sceneManager.ClearScenes();
    }

    //
    // Game loop methods
    //

    /// <summary>
    /// Do not call this.
    /// </summary>
    /// <param name="gameTime"></param>
    protected override void Update(GameTime gameTime)
    {

        inputManager.Update();
        EngineGlobals.game.sceneManager.Input(gameTime);

        if (EngineGlobals.game.sceneManager._currentSceneList.Count == 0 && EngineGlobals.game.sceneManager.currentTransition == null)
            Exit();

        // Defer to the scene manager's update method
        EngineGlobals.game.sceneManager.Update(gameTime);
        base.Update(gameTime);

    }

    /// <summary>
    /// Do not call this.
    /// </summary>
    /// <param name="gameTime"></param>
    protected override void Draw(GameTime gameTime)
    {

        if (firstFrame == true)
        {
            firstFrame = false;
            return;
        }

        // Defer to the scene manager's draw method
        EngineGlobals.game.sceneManager.Draw();
        //EngineGlobals.inputManager.Draw();
        base.Draw(gameTime);

    }

    /// <summary>
    /// Ends the game.
    /// </summary>
    public void Quit()
    {
        ClearScenes();
        Exit();
    }

    //
    // System methods
    //

    /// <summary>
    /// Returns a string showing the order of execution of systems.
    /// </summary>
    /// <returns></returns>
    public string PrintSystems()
    {
        return systemManager.PrintSystems();
    }

    /// <summary>
    /// Positions the system of the specified type at the requested position.
    /// This is useful for setting the order in which systems execute.
    /// (Use `PrintSystems()` to see the order of systems.)
    /// </summary>
    /// <typeparam name="T">The type of system to move.</typeparam>
    /// <param name="position">The new position of the system.</param>
    public void PositionSystemType<T>(int position)
    {
        if (systemManager.IsRegistered<T>() && position >= 0 && position <= systemManager.registeredSystems.Count)
        {
            Type targetType = typeof(T);
            object systemToMove = null;
            for (int i = 0; i < systemManager.registeredSystems.Count; i++)
            {
                // Check the type of the current system in the list.
                if (systemManager.registeredSystems[i].GetType() == targetType)
                {
                    systemToMove = systemManager.registeredSystems[i];
                    break;
                }
            }
            if (systemToMove != null)
            {
                // 4. Remove the system from its current position.
                // This prevents duplication and ensures it's only in one place.
                systemManager.registeredSystems.Remove((System)systemToMove);
                // 6. Insert the system at the new position.
                systemManager.registeredSystems.Insert(position, (System)systemToMove);
            }
        }
    }

}
