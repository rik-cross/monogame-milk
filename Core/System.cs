//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Core;

/// <summary>
/// A system within a scene runs on entities with the reqiured set of components.
/// </summary>
public abstract class System {

    private ComponentManager _componentManager = EngineGlobals.game.componentManager;
    
    /// <summary>
    /// MonoGame spriteBatch used for drawing.
    /// </summary>
    protected SpriteBatch spriteBatch = EngineGlobals.game.spriteBatch;
    
    /// <summary>
    /// Decides whether the system DrawEntity() method is called before
    /// (below) or after (above) the top layer of the map is drawn.
    /// </summary>
    public bool DrawAboveMap = false;

    internal BitArray RequiredComponentBitMask;
    
    /// <summary>
    /// System constructor.
    /// </summary>
    public System() {
        RequiredComponentBitMask = new BitArray(_componentManager.maxComponents, false);
        Init();
    }

    /// <summary>
    /// Adds a component type that is required by an entity 
    /// in order to be processed by the system.
    /// </summary>
    /// <typeparam name="T">The type of the component required.</typeparam>
    public void AddRequiredComponentType<T>() where T : Component
    {

        if (_componentManager.IsComponentTypeRegistered(typeof(T)) == false)
            _componentManager.RegisterComponentType(typeof(T));

        int componentID = _componentManager.GetComponentTypeID(typeof(T));
        RequiredComponentBitMask.Set(componentID, true);

    }

    //
    // Optional game loop methods
    //

    /// <summary>
    /// Called once when a system is created.
    /// </summary>
    public virtual void Init() { }

    /// <summary>
    /// A top-level input method, called once per frame.
    /// </summary>
    /// <param name="gameTime">The MonoGame gameTime object.</param>
    /// <param name="scene">The scene that the system is in.</param>
    public virtual void Input(GameTime gameTime, Scene scene) { }

    /// <summary>
    /// An entity-specific input method, called once per frame
    /// for each entity with the required set of components.
    /// </summary>
    /// <param name="gameTime">The MonoGame gameTime object.</param>
    /// <param name="scene">The scene that the system is in</param>
    /// <param name="entity">The entity to be acted upon.</param>
    public virtual void InputEntity(GameTime gameTime, Scene scene, Entity entity) { }    

    /// <summary>
    /// A top-level update method, called once per frame.
    /// </summary>
    /// <param name="gameTime">The MonoGame gameTime object.</param>
    /// <param name="scene">The scene that the system is in.</param>
    public virtual void Update(GameTime gameTime, Scene scene) { }

    /// <summary>
    /// An entity-specific update method, called once per frame
    /// for each entity with the required set of components.
    /// </summary>
    /// <param name="gameTime">The MonoGame gameTime object.</param>
    /// <param name="scene">The scene that the system is in</param>
    /// <param name="entity">The entity to be acted upon.</param>
    public virtual void UpdateEntity(GameTime gameTime, Scene scene, Entity entity) { }

    /// <summary>
    /// A top-level draw method, called once per frame.
    /// </summary>
    /// <param name="scene">The scene that the system is in.</param>
    public virtual void Draw(Scene scene) {}

    /// <summary>
    /// An entity-specific draw method, called once per frame
    /// for each entity with the required set of components.
    /// </summary>
    /// <param name="scene">The scene that the system is in</param>
    /// <param name="entity">The entity to be acted upon.</param>
    public virtual void DrawEntity(Scene scene, Entity entity) { }

    //
    // Optional callbacks
    //

    /// <summary>
    /// Called once when the scene becomes active;
    /// </summary>
    /// <param name="scene">The scene that the system is in.</param>
    public virtual void OnEnterScene(Scene scene) { }

    /// <summary>
    /// Called once when the scene is no longer active.
    /// </summary>
    /// <param name="scene">The scene that the system is in.</param>
    public virtual void OnExitScene(Scene scene) { }

    /// <summary>
    /// Called once when an entity is added to the scene containing the system.
    /// </summary>
    /// <param name="scene">The scene containing the system that the entity is being added to.</param>
    /// <param name="entity">The entity being added to the scene.</param>
    public virtual void OnEntityAddedToScene(Scene scene, Entity entity) { }
    
    /// <summary>
    /// Called once when an entity is removed from the scene containing the system.
    /// </summary>
    /// <param name="scene">The scene containing the system that the entity is being removed from.</param>
    /// <param name="entity">The entity being removed from the scene.</param>
    public virtual void OnEntityRemovedFromScene(Scene scene, Entity entity) { }

    /// <summary>
    /// Called once when a system is added to a scene.
    /// </summary>
    /// <param name="scene">The scene the system is being added to.</param>
    public virtual void OnAddedToScene(Scene scene) { }

    /// <summary>
    /// Called once when a system is removed from a scene.
    /// </summary>
    /// <param name="scene">The scene the system is being removed from.</param>
    public virtual void OnRemovedFromScene(Scene scene) { }
    
}