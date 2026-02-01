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
    private readonly bool _showSplash;

    // MonoGame graphics helper instances 
    internal ContentManager content;
    internal GraphicsDeviceManager graphics;
    internal GraphicsDevice graphicsDevice;
    public SpriteBatch spriteBatch;
    public EngineResourceManager _engineResources;

    // Manager instances
    internal EntityManager entityManager;
    internal ComponentManager componentManager;
    internal SceneManager sceneManager;
    internal SystemManager systemManager;
    
    /// <summary>
    /// The game's input manager, used to query game input.
    /// </summary>
    public InputManager inputManager;

    // This is set to 'true' initially, and then set to 'false' after the first frame
    // It prevents the window looking 'glitchy' on the first game frame 
    private bool firstFrame = true;

    /// <summary>
    /// Setting debug mode to true displays various game, scene and entity information.
    /// </summary>
    public bool Debug;

    /// <summary>
    /// Creates a new game.
    /// </summary>
    /// <param name="title">Game window title (default = "milk").</param>
    /// <param name="size">Window size (default = (800, 480)).</param>
    /// <param name="isMouseVisible">Sets mouse visibility (default = true).</param>
    /// <param name="maxEntities">The maximum number of game entities (default = 1024).</param>
    /// <param name="maxComponents">The maximum number of game components (default = 128).</param>
    /// <param name="showSplash">Choose to show the milk splash scene (default = true).</param>
    /// <param name="debug">Sets debug mode (default = false).</param>
    public Game(
        string? title = null,
        Vector2? size = null,
        bool isMouseVisible = true,
        int maxEntities = 1024,
        int maxComponents = 128,
        bool showSplash = true,
        bool debug = false
    )
    {

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

        _showSplash = showSplash;
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

        // Initialize the manager with Services
        _engineResources = new EngineResourceManager(this.Services);
        // Load the internal assets
        _engineResources.LoadEngineContent();

        // Ensure that render targets aren't cleared when swapping between them
        GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

        // Create 'manager' instances
        sceneManager = new SceneManager();
        componentManager = new ComponentManager(maxComponents: _maxComponents, maxEntities: _maxComponents);
        systemManager = new SystemManager();
        entityManager = new EntityManager(maxExtities: _maxEntities);
        inputManager = new InputManager();

        // Register all provided systems
        systemManager.RegisterSystem(new InputSystem());
        systemManager.RegisterSystem(new SpriteSystem());
        systemManager.RegisterSystem(new TriggerSystem());
        systemManager.RegisterSystem(new PhysicsSystem());

        if (_showSplash == true)
        {

            // Run the Init callback if one has been specified
            if (Init != null)
                Init();

            // Set a default scene if the scene stack only contains the splash scene
            if (sceneManager._currentSceneList.Count == 0 && sceneManager.currentTransition == null)
                sceneManager.SetScene(new DefaultScene());

            // Add the splash scene
            sceneManager.SetScene(new SplashScene(), keepExistingScenes: true);

        }
        else
        {
            // Run the Init callback if one has been specified
            if (Init != null)
                Init();    

            // Set a default scene if the scene stack is empty
            if (sceneManager._currentSceneList.Count == 0 && sceneManager.currentTransition == null)
                sceneManager.SetScene(new DefaultScene());
        }

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
        sceneManager.Draw();
        base.Draw(gameTime);

    }

    /// <summary>
    /// Ends the game.
    /// </summary>
    public void Quit()
    {
        sceneManager.ClearScenes();
        Exit();
    }

}
