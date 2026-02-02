//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using milk.UI;

namespace milk.Core;

/// <summary>
/// A scene is a collection of entities and systems, and can be thought
/// of as game 'screens'. Each system in a scene acts on all entities in
/// the scene with the required set of components.
/// </summary>
public abstract class Scene : MilkMethods
{

    private EntityManager _entityManager = EngineGlobals.game.entityManager;
    private GraphicsDevice _graphicsDevice = EngineGlobals.game.graphicsDevice;
    private List<Entity> entitiesToRemoveFromScene = new List<Entity>();

    /// <summary>
    /// The parent game object.
    /// </summary>
    public Game game = EngineGlobals.game;
    
    /// <summary>
    /// The (x,y) scene dimensions.
    /// </summary>
    public readonly Vector2 Size;

    /// <summary>
    /// The (x,y) center point of the scene.
    /// </summary>
    public readonly Vector2 Middle;
    public float elapsedTime;
    internal List<Entity> entities;
    internal List<System> systems;
    private UIMenu UIMenu;
    internal Animator animator;

    /// <summary>
    /// Background color.
    /// </summary>
    public Microsoft.Xna.Framework.Color BackgroundColor = Microsoft.Xna.Framework.Color.Transparent;
    
    /// <summary>
    /// By default all systems are present in all scenes.
    /// Setting this to `false` allows control over which
    /// systems should be present within a scene.
    /// </summary>
    public bool IncludeAlRegisteredSystems = true;
    protected List<Camera> cameras = new List<Camera>();

    /// <summary>
    /// Specifies whether to update the scene below this one in the scene stack.
    /// </summary>
    public bool UpdateSceneBelow = true;

    /// <summary>
    /// Specifies whether to process input in the scene below this one in the scene stack.
    /// </summary>
    public bool InputSceneBelow = false;

    /// <summary>
    /// Specifies whether to draw the scene below this one in the scene stack.
    /// </summary>
    public bool DrawSceneBelow = true;

    private List<TimedAction> timedActions = new List<TimedAction>();

    /// <summary>
    /// Set a method for sorting entities.
    /// </summary>
    public Comparison<Entity>? EntitySortMethod = null;

    /// <summary>
    /// Set the size of the map. The size is set automatically if
    /// adding a Tiled Map.
    /// </summary>
    public Vector2? mapSize = null;

    private TiledMapRenderer? _mapRenderer = null;
    private TiledMap? _map = null;

    /// <summary>
    /// A Tiled map.
    /// </summary>
    public TiledMap? Map
    {
        get
        {
            return _map;
        }
        set
        {
            _map = value;

            if (value != null)
            {
                mapSize = new Vector2(_map.Width * _map.TileWidth, _map.Height * _map.TileHeight);
                _mapRenderer = new TiledMapRenderer(_graphicsDevice, _map);
            } else
            {
                mapSize = Vector2.Zero;
                _mapRenderer = null;
            }

            TiledMapObjectLayer collisionLayer = _map.GetLayer<TiledMapObjectLayer>("collisions");
            if (collisionLayer != null) {
                ClearSceneColliders();
                foreach (TiledMapObject collisionObject in collisionLayer.Objects)
                {
                    AddSceneCollider(
                        (int)collisionObject.Position.X,
                        (int)collisionObject.Position.Y,
                        (int)collisionObject.Size.Width,
                        (int)collisionObject.Size.Height
                    );
                }
            }

        }
    }

    /// <summary>
    /// Collidable areas of the scene.
    /// </summary>
    internal List<SceneCollider> sceneColliders;

    /// <summary>
    /// The scene's velocity, which is applied to all entities.
    /// </summary>
    public Vector2 Velocity = Vector2.Zero;

    /// <summary>
    /// The scene's acceleration, which is applied to all entities.
    /// </summary>
    public Vector2 Acceleration = Vector2.Zero;

    /// <summary>
    /// A scene is a bit like a game 'screen' and holds
    /// entities and systems, along with other functionalilty
    /// such as cameras. Systems in a scene act on scene entities
    /// with the required set of components present.
    /// </summary>
    public Scene()
    {

        Size = game.Size;
        Middle = new Vector2(Size.X / 2, Size.Y / 2);
        elapsedTime = 0f;
        entities = new List<Entity>();
        systems = new List<System>();
        milk.Core.Milk.Scenes.allScenes.Add(this);

        if (IncludeAlRegisteredSystems == true)
            milk.Core.Milk.Systems.AddAllRegisteredSystemsToScene(this);

        UIMenu = new UIMenu(this);
        animator = new Animator();

        sceneColliders = new List<SceneCollider>();

        Init();

    }

    internal void _Update(GameTime gameTime, List<Scene> scenes)
    {

        // update scene below
        Scene? sceneBelow = GetSceneBelow(scenes);
        if (sceneBelow!= null && UpdateSceneBelow == true)
            sceneBelow._Update(gameTime, scenes);

        elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Update entity State here??
        foreach (Entity e in entities)
            e.PreviousState = e.State;

        // Process the timed actions
        for (int i = timedActions.Count - 1; i >= 0; i--)
        {
            if (elapsedTime >= timedActions[i].ElapsedTime)
            {
                timedActions[i].Execute();
                timedActions.RemoveAt(i);
            }
        }

        // high-level update
        foreach (System system in systems)
            system.Update(gameTime, this);

        // entity-specific update
        foreach (System system in systems)
        {
            foreach (Entity entity in entities)
            {
                BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                temp.And(entity.bitMask);
                if (Utilities.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
                    system.UpdateEntity(gameTime, this, entity);
            }
        }

        foreach (Camera camera in cameras)
            camera.Update(gameTime);

        Update(gameTime);

        // Remove entities
        RemoveEntitiesFromSceneAtEndOfUpdate();

        // delete all entities
        _entityManager.DeleteEntities();

        // Sort entities
        if (EntitySortMethod != null)
            entities.Sort(EntitySortMethod);
        
        animator.Update(gameTime);

    }

    internal void _Input(GameTime gameTime, List<Scene> scenes)
    {

        // Process the scene below's input
        Scene? sceneBelow = GetSceneBelow(scenes);
        if (sceneBelow != null && InputSceneBelow == true)
            sceneBelow._Input(gameTime, scenes);

        // high-level input
        foreach (System system in systems)
            system.Input(gameTime, this);

        // entity-specific update
        foreach (System system in systems)
        {
            foreach (Entity entity in entities)
            {
                BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                temp.And(entity.bitMask);
                if (Utilities.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
                    system.InputEntity(gameTime, this, entity);
            }
        }

        // Call the user-defined Input method
        Input(gameTime);

        // Update the menu (only if there's no active scene transition)
        if (game.sceneManager.currentTransition == null)
            UIMenu.Input(gameTime, this);

    }

    internal void _Draw(RenderTarget2D renderTarget, List<Scene> scenes, bool drawSceneBelow = true)
    {

        _graphicsDevice.SetRenderTarget(renderTarget);

        // Draw scene below
        Scene? sceneBelow = GetSceneBelow(scenes);
        if (sceneBelow != null && drawSceneBelow == true)
            sceneBelow._Draw(renderTarget, scenes);

        // TODO
        // Add lighting shader here
        //milk.Core.Milk.Graphics.Begin(blendState: BlendState.AlphaBlend, effect: lightingShader);
        //milk.Core.Milk.Graphics.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, lightingShader);

        // Draw scene background
        milk.Core.Milk.Graphics.Begin(samplerState: SamplerState.PointClamp);
        
        milk.Core.Milk.Graphics.FillRectangle(
            new Rectangle(0, 0, (int)Size.X, (int)Size.Y),
            BackgroundColor
        );

        milk.Core.Milk.Graphics.End();

        // Iterate over each camera in the scene
        foreach (Camera camera in cameras)
        {

            // Set the viewport to the camera
            _graphicsDevice.Viewport = camera.getViewport();

            // Draw camera background
            milk.Core.Milk.Graphics.Begin(samplerState: SamplerState.PointClamp);
            milk.Core.Milk.Graphics.FillRectangle(new Rectangle(0, 0, (int)Size.X, (int)Size.Y), camera.BackgroundColor);
            milk.Core.Milk.Graphics.End();

            milk.Core.Milk.Graphics.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.getTransformMatrix());

            // Draw the map layers marked as 'belowEntities: true'
            DrawMap(camera, "belowEntities", "true");

            // High-level System.Draw() -- below entities
            foreach (System system in systems)
            {
                if (system.DrawAboveEntities == false)
                    system.Draw(this);
            }

            // Entity-specific system drawing
            foreach (System system in systems)
            {
                foreach (Entity entity in entities)
                {
                    BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                    temp.And(entity.bitMask);
                    if (Utilities.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
                        system.DrawEntity(this, entity);
                }
            }

            milk.Core.Milk.Graphics.End();

            // High-level System.Draw() -- below map
            milk.Core.Milk.Graphics.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.getTransformMatrix());
            foreach (System system in systems)
            {
                if (system.DrawAboveMap == false)
                    system.Draw(this);
            }
            milk.Core.Milk.Graphics.End();

            // Draw the map layers marked as 'belowEntities: true'
            milk.Core.Milk.Graphics.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.getTransformMatrix());
            DrawMap(camera, "belowEntities", "false");
            milk.Core.Milk.Graphics.End();

            // High-level System.Draw() -- above all
            milk.Core.Milk.Graphics.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.getTransformMatrix());
            foreach (System system in systems)
            {
                if (system.DrawAboveEntities == true && system.DrawAboveMap == true)
                    system.Draw(this);
            }

            // Debug draw scene colliders
            if (game.Debug == true) {
                foreach (SceneCollider sceneCollider in sceneColliders)
                {
                    milk.Core.Milk.Graphics.DrawRectangle(
                        new RectangleF(
                            sceneCollider.X,
                            sceneCollider.Y,
                            sceneCollider.Width,
                            sceneCollider.Height
                        ),
                        Color.Red,
                        1.0f
                    );
                }
            }

            milk.Core.Milk.Graphics.End();

            _graphicsDevice.Viewport = new Viewport(0, 0, (int)Size.X, (int)Size.Y);

            milk.Core.Milk.Graphics.Begin(samplerState: SamplerState.PointClamp);

            // Camera border
            if (camera.BorderWidth > 0)
                milk.Core.Milk.Graphics.DrawRectangle(
                    new RectangleF(
                        camera.ScreenPosition.X - camera.BorderWidth,
                        camera.ScreenPosition.Y - camera.BorderWidth,
                        camera.ScreenSize.X + camera.BorderWidth * 2,
                        camera.ScreenSize.Y + camera.BorderWidth * 2
                    ),
                    color: camera.BorderColor,
                    thickness: camera.BorderWidth
                );

            milk.Core.Milk.Graphics.End();

        }

        // Reset the viewport to the whole scene
        _graphicsDevice.Viewport = new Viewport(0, 0, (int)Size.X, (int)Size.Y);

        // no shader here
        milk.Core.Milk.Graphics.Begin(samplerState: SamplerState.PointClamp);

        // high-level draw for those above entities
        foreach (System system in systems)
        {
            if (system.DrawAboveEntities == true)
                system.Draw(this);
        }

        Draw();
        UIMenu.Draw();

        milk.Core.Milk.Graphics.End();

    }

    //
    // Cameras
    //

    /// <summary>
    /// Adds a camera object to a scene.
    /// </summary>
    /// <param name="camera">The camera to add to the scene.</param>
    protected void AddCamera(Camera camera)
    {
        cameras.Add(camera);
        camera.Scene = this;
    }

    /// <summary>
    /// Returns a camera with the specified (unique) name.
    /// The name is checked in lower-case.
    /// </summary>
    /// <param name="name">The name of the camera to find.</param>
    /// <returns>A camera, or null if no camera exists with the specified name.</returns>
    public Camera? GetCameraByName(string name)
    {
        foreach (Camera camera in cameras)
        {
            if (camera.Name.ToLower() == name.ToLower())
                return camera;
        }
        return null;
    }

    //
    // Entity methods
    //

    /// <summary>
    /// Returns `true` is the entity provided exists in the scene.
    /// </summary>
    /// <param name="entity">The entity to check.</param>
    /// <returns>A Boolean indicating the presence of the entity in the scene.</returns>
    public bool HasEntity(Entity entity)
    {
        return entities.Contains(entity);
    }

    /// <summary>
    /// Adds an entity object to the scene.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public void AddEntity(Entity entity)
    {

        // only allow each entity to be added once
        if (entities.Contains(entity) == false)
        {

            entities.Add(entity);

            // call OnEntityAddedToScene for all relevant systems in the scene
            foreach (System system in systems)
            {

                // only if the entity has the required components for the system
                BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                temp.And(entity.bitMask);
                if (Utilities.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
                    system.OnEntityAddedToScene(this, entity);

            }

            OnEntityAdded(entity);
        }
    }

    //
    // Entity removal (not deletion)
    //

    /// <summary>
    /// Removes the specified entity from the scene.
    /// Note that the entity isn't 'deleted'.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    public void RemoveEntity(Entity entity)
    {
        // only add the entity to a scene once
        if (entitiesToRemoveFromScene.Contains(entity) == false)
            entitiesToRemoveFromScene.Add(entity);
        
        // Unset tracked entity
        foreach (Camera camera in cameras)
        {
            if (camera.TrackedEntity == entity)
                camera.TrackedEntity = null;
        }
    }

    /// <summary>
    /// Removes the entity with the specified name from the scene.
    /// </summary>
    /// <param name="name">The name of the entity to remove.</param>
    public void RemoveEntityByName(string name)
    {
        Entity? entity = GetEntityByName(name);
        if (entity != null)
            RemoveEntity(entity);
    }

    /// <summary>
    /// Removes the entities with the specified tags from the scene.
    /// </summary>
    /// <param name="tags">One or more tags that the entities must have.</param>
    public void RemoveEntitiesByTag(params string[] tags)
    {
        List<Entity> entities = GetEntitiesByTag(tags);
        foreach (Entity entity in entities)
            RemoveEntity(entity);
    }

    /// <summary>
    /// Removes all entities from the scene.
    /// </summary>
    public void RemoveAllEntities()
    {
        foreach (Entity entity in entities)
            RemoveEntity(entity);
    }

    //
    // entities are deleted by adding them to the entitiesToRemoveFromScene list.
    // at the end of each game loop, all entities in this list are removed from the scene.
    // this stops corruption caused by removal of entities when processing the scene.
    internal void RemoveEntitiesFromSceneAtEndOfUpdate()
    {
        foreach (Entity entity in entitiesToRemoveFromScene)
        {
            if (entities.Contains(entity))
            {

                // OnEntityRemovedFromScene
                // ...for each system

                foreach (System system in systems)
                {

                    // only if the entity has the required components for the system

                    BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                    temp.And(entity.bitMask);
                    if (Utilities.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
                        system.OnEntityRemovedFromScene(this, entity);

                }

                foreach (Camera camera in cameras)
                {
                    if (camera.TrackedEntity == entity)
                        camera.TrackedEntity = null;
                }

                OnEntityRemoved(entity);
                entities.Remove(entity);

            }
        }
        entitiesToRemoveFromScene.Clear();
    }

    public List<Entity> GetAllEntities()
    {
        return entities;
    }

    /// <summary>
    /// Gets the entity with the provided (unique) name.
    /// The name supplied is converted to lower-case.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>An entity if one is found, or null.</returns>
    public Entity? GetEntityByName(string name)
    {
        return _entityManager.GetEntityByNameInList(entities, name);
    }

    /// <summary>
    /// Gets all entities that include all supplied tags.
    /// </summary>
    /// <param name="tags">One or more tags to check.</param>
    /// <returns>A list of all entities with all tags. This is always a list, and could be empty.</returns>
    public List<Entity> GetEntitiesByTag(params string[] tags)
    {
        return _entityManager.GetEntitiesByTagInList(entities, tags);
    }

    //
    // Entity deletion
    //

    /// <summary>
    /// Marks entity in the scene with the specified name for deletion.
    /// The name supplied is checked in lower-case.
    /// </summary>
    /// <param name="name">The name of the entity to delete.</param>
    public void DeleteEntityByName(string name)
    {
        foreach (Entity entity in entities)
        {
            if (entity.Name != null && entity.Name.ToLower() == name.ToLower())
            {
                entity.Delete = true;
                // entity names are unique, so nothing more to do.
                return;
            }
        }
    }

    /// <summary>
    /// Marks all entities containing the specified tags for deletion.
    /// Tags are checked in lower-case.
    /// </summary>
    /// <param name="tags">One or more tags to</param>
    public void DeleteEntitiesByTag(params string[] tags)
    {
        foreach (Entity entity in entities)
        {
            if (entity.HasTag(tags))
                entity.Delete = true;
        }
    }

    //
    // System methods
    //

    /// <summary>
    /// Add a (registered) system type to a scene (calling this
    /// method isn't required if IncludeAlRegisteredSystems == true, 
    /// which it is by default).
    /// </summary>
    /// <typeparam name="T">The type of the registered system to add.</typeparam>
    public void AddSystem<T>() where T : System
    {
        // Can only add registered systems to a scene
        if (milk.Core.Milk.Systems.IsRegistered<T>() == false)
            return;

        // Add the instance of the specified type
        systems.Add(milk.Core.Milk.Systems.GetSystem<T>());

        // OnAddedToScene callback
        milk.Core.Milk.Systems.GetSystem<T>().OnAddedToScene(this);
    }

    /// <summary>
    /// Removes the sysytem of the specified type from the scene.
    /// </summary>
    /// <typeparam name="T">The type of system to remove.</typeparam>
    public void RemoveSystem<T>() where T : System
    {
        milk.Core.Milk.Systems.RemoveSystemOfTypeFromList<T>(systems);
        
        // OnRemovedFromScene callback
        milk.Core.Milk.Systems.GetSystem<T>().OnRemovedFromScene(this);
    }

    /// <summary>
    /// Removes all systems from the scene
    /// </summary>
    public void RemoveAllSystems()
    {
        foreach (System system in systems)
            system.OnRemovedFromScene(this);
        systems.Clear();
    }

    /// <summary>
    /// Returns true if a system of the specified type
    /// exists in the scene.
    /// </summary>
    /// <typeparam name="T">The type of system to find.</typeparam>
    /// <returns>Bool, true if a system of the specified type exists.</returns>
    public bool HasSystem<T>() where T : System
    {
        return milk.Core.Milk.Systems.HasSystemOfTypeInList<T>(systems);
    }

    public string PrintSystems()
    {
        return milk.Core.Milk.Systems.PrintSystemsInList(systems);
    }

    public void PositionSystem<T>(int position)
    {
        milk.Core.Milk.Systems.PositionSystemTypeInList<T>(position, systems);
    }

    //
    // Scene
    //

    /// <summary>
    /// Gets the scene below this scene in the scene list
    /// (or null if there isn't one).
    /// </summary>
    /// <param name="scenes">The scene list to check against.</param>
    /// <returns>A scene, or null if no scene exists.</returns>
    public Scene? GetSceneBelow(List<Scene>? scenes = null)
    {

        if (scenes == null)
            scenes = milk.Core.Milk.Scenes._currentSceneList;

        for (int i = 0; i < scenes.Count - 1; i++)
        {
            if (scenes[i] == this)
                return scenes[i + 1];
        }
        return null;
    }

    //
    // Scene collision
    //

    public void AddSceneCollider(int x, int y, int w, int h)
    {
        sceneColliders.Add(new SceneCollider(x, y, w, h));
    }

    public void ClearSceneColliders()
    {
        sceneColliders.Clear();
    }

    //
    // Map
    //

    private void DrawMap(Camera camera, string? property = null, string? value = null)
    {

        if (_map == null)
            return;

        // Draw all map layers (if no property specified)
        // Or those layers with a matching property value
        foreach (TiledMapLayer layer in _map.Layers)
        {
            if (property == null || (property != null && layer.Properties.TryGetValue(property, out string propValue) && propValue == value))
                _mapRenderer.Draw(layer, camera.getTransformMatrix());
        }

    }

    //
    // Timed actions.
    //

    /// <summary>
    /// Adds an action to execute after a set time.
    /// </summary>
    /// <param name="elapsedTime">The delay before executing the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="name">Optional name, to facilitate deletion.</param>
    public void AddTimedAction(float elapsedTime, Action action, string? name = null)
    {
        timedActions.Add(new TimedAction(elapsedTime, action, name));
    }

    /// <summary>
    /// Remove a timed action by name, if one has been set.
    /// </summary>
    /// <param name="name">The name of the action to remove.</param>
    public void RemoveTimedActionByName(string name)
    {
        for (int i = timedActions.Count - 1; i >= 0; i--)
        {
            if (timedActions[i].Name.ToLower() == name.ToLower())
                timedActions.RemoveAt(i);
        }
    }

    //
    // Animations / Tweens
    //

    /// <summary>
    /// Adds a new animation to by specifying a 'lerp' function.
    /// </summary>
    /// <param name="action">The 'lerp' function that controls the changing value(s).</param>
    /// <param name="duration">The length of the lerp or animation.</param>
    /// <param name="easingFunction">The easing function (default = null - linear easing).</param>
    public void AddAnimation(
        Action<float> action,
        float duration,
        EasingFunctions.EasingDelegate? easingFunction = null)
    {
        animator.AddTween(new Tween(action, duration, easingFunction));
    }

    //
    // UI
    //

    /// <summary>
    /// Add a UI element to the scene.
    /// </summary>
    /// <param name="element">The element to add.</param>
    public void AddUIElement(UIElement element)
    {
        UIMenu.AddElement(element);
        element.parentScene = this;
    }

    //
    // optional scene methods
    //

    /// <summary>
    /// Called once when a scene is created.
    /// </summary>
    public virtual void Init() { }

    /// <summary>
    /// Called once per frame of the game loop, and handles scene-level updating.
    /// </summary>
    /// <param name="gameTime">The MonoGame gameTime object.</param>
    public virtual void Update(GameTime gameTime) { }

    /// <summary>
    /// Called once per frame of the game loop, and handles scene-level input.
    /// </summary>
    /// <param name="gameTime">The MonoGame gameTime object.</param>
    public virtual void Input(GameTime gameTime) { }

    /// <summary>
    /// Called once per frame of the game loop, and handles scene-level drawing.
    /// </summary>
    public virtual void Draw() { }

    //
    // optional scene callback functions
    //

    /// <summary>
    /// Called once when the scene becomes the active scene.
    /// </summary>
    public virtual void OnEnter() { }

    /// <summary>
    /// Called once when the scene is no longer the active scene.
    /// </summary>
    public virtual void OnExit() { }
    
    /// <summary>
    /// Called once whenever an entity is added to the scene.
    /// </summary>
    /// <param name="entity">The entity that has been added.</param>
    public virtual void OnEntityAdded(Entity entity) { }

    /// <summary>
    /// Called once whenever an entity is removed from the scene.
    /// </summary>
    /// <param name="entity">The entity that is being removed.</param>
    public virtual void OnEntityRemoved(Entity entity) { }

}
