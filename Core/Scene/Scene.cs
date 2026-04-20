//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using milk.Components;
using milk.UI;

namespace milk.Core;

//
// Partial class files:
// Scene.cs
// -- Scene.CollectionGenerics.cs
// -- Scene.Colliders.cs
// -- Scene.Cameras.cs
// -- Scene.Markers.cs
// -- Scene.TimedAcions.cs
// -- Scene.Tweens.cs
//

/// <summary>
/// A scene is a collection of entities and systems, and can be thought
/// of as a game 'screen'. Each system in a scene acts on all entities in
/// the scene with the required set of components.
/// </summary>
public abstract partial class Scene : MilkMethods
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
    public float ElapsedTime;

    /// <summary>
    /// Scene entities.
    /// </summary>
    public List<Entity> Entities;
    
    /// <summary>
    /// Scene systems.
    /// </summary>
    public List<System> systems;
    
    private UIMenu UIMenu;

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
                if (_graphicsDevice != null)
                    _mapRenderer = new TiledMapRenderer(_graphicsDevice, _map);
            } else
            {
                mapSize = Vector2.Zero;
                _mapRenderer = null;
            }

            //
            // Collisions
            //

            if (value != null && _graphicsDevice != null)
            {

                TiledMapObjectLayer collisionLayer = _map.GetLayer<TiledMapObjectLayer>("collisions");
                if (collisionLayer != null) {
                    RemoveSceneCollider();
                    foreach (TiledMapObject collisionObject in collisionLayer.Objects)
                    {
                        AddSceneCollider(
                            new SceneCollider(
                                position: new Vector2(collisionObject.Position.X, collisionObject.Position.Y),
                                size: new Vector2(collisionObject.Size.Width, collisionObject.Size.Height),
                                name: collisionObject.Name  
                            )
                        );
                    }
                }
            
            }

            //
            // Entities
            //

            if (value != null && _graphicsDevice != null)
            {

                var entityLayer = _map.GetLayer<TiledMapObjectLayer>("entities");
                if (entityLayer != null)
                {
                    foreach (var obj in entityLayer.Objects)
                    {
                        Entity? e = Milk.Entities.CreateFromPrototype(obj.Name, obj.Position);

                        if (e != null) {

                            if (e.HasComponent<TransformComponent>()) {
                                TransformComponent tc = e.GetComponent<TransformComponent>()!;
                                tc.X -= tc.Width / 2;
                                tc.Y -= tc.Height;
                            }

                            AddEntity(e);

                        }
            
                    }
                }

            }

        }
    }

    /// <summary>
    /// The scene's velocity, which is applied to all entities
    /// with a PhysicsComponent.
    /// </summary>
    public Vector2 Velocity = Vector2.Zero;

    /// <summary>
    /// The scene's acceleration, which is applied to all entities
    /// with a PhysicsComponent.
    /// </summary>
    public Vector2 Acceleration = Vector2.Zero;

    /// <summary>
    /// A scene is a bit like a game 'screen' and holds
    /// entities and systems, along with other functionalilty
    /// such as cameras and a map. Systems in a scene act on
    /// scene entities with the required set of components present.
    /// </summary>
    public Scene()
    {

        Size = game.Size;
        Middle = new Vector2(Size.X / 2, Size.Y / 2);
        ElapsedTime = 0f;
        Entities = new List<Entity>();
        systems = new List<System>();
        milk.Core.Milk.Scenes.allScenes.Add(this);

        if (IncludeAlRegisteredSystems == true)
            milk.Core.Milk.Systems.AddAllRegisteredSystemsToScene(this);

        UIMenu = new UIMenu(this);

        Init();

    }

    internal void _Update(GameTime gameTime, List<Scene> scenes)
    {

        // Update scene below
        Scene? sceneBelow = GetSceneBelow(scenes);
        if (sceneBelow!= null && UpdateSceneBelow == true)
            sceneBelow._Update(gameTime, scenes);

        // Update the scene-specific elapsed time
        ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Update entity states
        foreach (Entity entity in Entities)
            entity.PreviousState = entity.State;

        // High-level system updates
        foreach (System system in systems.ToList())
            system.Update(gameTime, this);

        // Entity-specific update
        foreach (System system in systems.ToList())
        {
            foreach (Entity entity in Entities.ToList())
            {
                BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                temp.And(entity.bitMask);
                if (Utilities.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
                    system.UpdateEntity(gameTime, this, entity);
            }
        }

        // Update scene collections
        UpdateTimedActions(gameTime);
        UpdateCameras(gameTime);
        UpdateMarkers(gameTime);
        UpdateTweens(gameTime);

        // Call the user-defined scene Update method
        Update(gameTime);

        // Remove entities
        RemoveEntitiesFromSceneAtEndOfUpdate();

        // delete all entities
        _entityManager.DeleteEntities();

        // Sort entities
        if (EntitySortMethod != null)
            Entities.Sort(EntitySortMethod);

    }

    internal void _Input(GameTime gameTime, List<Scene> scenes)
    {

        // Process the scene below's input, if required
        Scene? sceneBelow = GetSceneBelow(scenes);
        if (sceneBelow != null && InputSceneBelow == true)
            sceneBelow._Input(gameTime, scenes);

        // Run the high-level Input() method for each of the scene's active systems
        foreach (System system in systems.ToList())
            system.Input(gameTime, this);

        //
        // Call InputEntity() for each entity for each system.
        // If a system 'consumes' an entity, the entity is added
        // to the consumedEntities list, so that subsequent systems
        // don't run their InputEntity() method.
        //

        // A list of entities that have had their input consumed
        HashSet<Entity> consumedEntities = new HashSet<Entity>();

        // Iterate over all entities in each system
        foreach (System system in systems.ToList())
        {
            foreach (Entity entity in Entities.ToList())
            {
                // Compare bitArrays, and only run InputEntity() for those
                // entities with all of the required components
                BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                temp.And(entity.bitMask);
                if (Utilities.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
                {
                    // Don't run if the entities input has already been consumed
                    if (consumedEntities.Contains(entity))
                        continue;    
                    else
                    {
                        // Run InputEntity(), and add the entity to the consumedEntities
                        // list if the system has consumed the input
                        bool consume = system.InputEntity(gameTime, this, entity);
                        if (consume == true)
                            consumedEntities.Add(entity);
                    }
                    
                }
            }
        }

        // Call the user-defined scene Input method
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
        foreach (Camera camera in Cameras)
        {

            // Set the viewport to the camera
            _graphicsDevice.Viewport = camera.GetViewport();

            // Draw camera background
            milk.Core.Milk.Graphics.Begin(samplerState: SamplerState.PointClamp);
            milk.Core.Milk.Graphics.FillRectangle(new Rectangle(0, 0, (int)Size.X, (int)Size.Y), camera.BackgroundColor);
            milk.Core.Milk.Graphics.End();

            milk.Core.Milk.Graphics.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.GetTransformMatrix());

            // Draw the map layers marked as 'belowEntities: true'
            DrawMapLayers(camera, GetFilteredMapLayers(includeAbove: false));

            // Call DrawEntity() for those systems with DrawAboveMap == false
            foreach (System system in systems)
            {
                if (system.DrawAboveMap == false) {
                    foreach (Entity entity in Entities)
                    {
                        BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                        temp.And(entity.bitMask);
                        if (Utilities.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
                            system.DrawEntity(this, entity);
                    }
                }
            }

            milk.Core.Milk.Graphics.End();

            // Draw the map layers marked as 'belowEntities: true'
            milk.Core.Milk.Graphics.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.GetTransformMatrix());
            DrawMapLayers(camera, GetFilteredMapLayers(includeBelow: false));
            milk.Core.Milk.Graphics.End();

            milk.Core.Milk.Graphics.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.GetTransformMatrix());

            // Call DrawEntity() for those systems with DrawAboveMap == true
            foreach (System system in systems)
            {
                if (system.DrawAboveMap == true) {
                    foreach (Entity entity in Entities)
                    {
                        BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                        temp.And(entity.bitMask);
                        if (Utilities.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
                            system.DrawEntity(this, entity);
                    }
                }
            }

            // Debug draw scene colliders
            if (game.Debug == true) {
                foreach (SceneCollider sceneCollider in SceneColliders)
                {
                    milk.Core.Milk.Graphics.DrawRectangle(
                        new RectangleF(
                            sceneCollider.Position.X,
                            sceneCollider.Position.Y,
                            sceneCollider.Size.X,
                            sceneCollider.Size.Y
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

        foreach (System system in systems)
            system.Draw(this);
        
        //
        // Markers
        // TODO -- add to camera update?
        //

        foreach (Marker marker in Markers)
        {
            if (marker.Visible == false)
                continue;

            foreach (Camera camera in Cameras)
            {

                bool isExcluded = false;
                if (camera.Name != null)
                {
                    foreach (string excludedName in marker.CamerasToExclude)
                    {
                        if (excludedName.ToLower() == camera.Name.ToLower())
                        {
                            isExcluded = true;
                            break;
                        }
                    }
                }

                if (isExcluded)
                    continue;

                Vector2 markerScreenPosition = camera.WorldToScreenPosition(marker.TargetPosition);

                // If the marker position is within the camera viewport
                // then show the marker in the target position, with some bounce
                if (camera.IsWorldPositionVisible(marker.TargetPosition) == true)
                {

                    // Point downwards
                    Milk.Graphics.Draw(
                        marker.Texture,
                        new Rectangle(
                            (int)markerScreenPosition.X,
                            (int)(markerScreenPosition.Y + Math.Sin(ElapsedTime * marker.BounceFrequency * MathHelper.TwoPi) * marker.BounceAmplitude),
                            (int)marker.Size.X,
                            (int)marker.Size.Y
                        ),
                        null,
                        Color.White,
                        (float)(2 * 3.14 / 4),
                        new Vector2(marker.Texture.Width, marker.Texture.Height / 2),
                        SpriteEffects.None,
                        0
                    );

                }
                // Otherwise, if the marker is not within the camera viewport
                // then clamp the marker to the edge of the camera and point
                // towards the marker's target position.
                else
                {
                    Vector2 clampedScreenPosition = new Vector2(
                        
                        Math.Clamp(
                            markerScreenPosition.X,
                            camera.ScreenPosition.X + marker.CameraBorder,
                            camera.ScreenPosition.X + camera.ScreenSize.X - marker.CameraBorder
                        ),

                        Math.Clamp(
                            markerScreenPosition.Y,
                            camera.ScreenPosition.Y + marker.CameraBorder,
                            camera.ScreenPosition.Y + camera.ScreenSize.Y - marker.CameraBorder
                        )

                    );

                    Vector2 direction = 
                        markerScreenPosition - 
                        camera.GetScreenCenter();

                    Milk.Graphics.Draw(
                        marker.Texture,
                        new Rectangle(
                            (int)clampedScreenPosition.X,
                            (int)clampedScreenPosition.Y,
                            (int)marker.Size.X,
                            (int)marker.Size.Y
                        ),
                        null,
                        Color.White,
                        (float)Math.Atan2(direction.Y, direction.X),
                        new Vector2(marker.Texture.Width, marker.Texture.Height / 2),
                        SpriteEffects.None,
                        0
                    );

                    if (marker.Text != null)
                    {
                        Vector2 toMarker = clampedScreenPosition - camera.GetScreenCenter();

                        if (toMarker != Vector2.Zero)
                            toMarker.Normalize();

                        Vector2 textPos = clampedScreenPosition - (toMarker * 45f);

                        SpriteFont font = EngineGlobals.game._engineResources.FontSmall;
                        Vector2 textSize = font.MeasureString(marker.Text);
                        Vector2 textOrigin = textSize / 2;

                        // Calculate min / max text position for clamping
                        float minX = camera.ScreenPosition.X + (textSize.X / 2) + marker.CameraBorder;
                        float maxX = camera.ScreenPosition.X + camera.ScreenSize.X - (textSize.X / 2) - marker.CameraBorder;
                        float minY = camera.ScreenPosition.Y + (textSize.Y / 2) + marker.CameraBorder;
                        float maxY = camera.ScreenPosition.Y + camera.ScreenSize.Y - (textSize.Y / 2) - marker.CameraBorder;

                        // Clamp the text position
                        textPos.X = Math.Clamp(textPos.X, minX, maxX);
                        textPos.Y = Math.Clamp(textPos.Y, minY, maxY);

                        Milk.Graphics.DrawString(
                            font,
                            marker.Text,
                            textPos,
                            Color.White,
                            0f,
                            textOrigin,
                            1.0f,
                            SpriteEffects.None,
                            0
                        );
                    }

                }

            }
        }

        Draw();
        UIMenu.Draw();

        if (Milk.Debug == true)
        {
            MouseState mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            milk.Core.Milk.Graphics.DrawCircle(mousePosition, 10, 10, Color.Blue);

            milk.Core.Milk.Graphics.DrawString(
                milk.Core.Milk.Assets.FontSmall,
                "Scene pos: " + mousePosition.X + ", " + mousePosition.Y,
                mousePosition + new Vector2(10, 0),
                Color.White
            );

            foreach (Camera camera in Cameras)
            {
                Rectangle r = new Rectangle(
                    (int)camera.ScreenPosition.X,
                    (int)camera.ScreenPosition.Y,
                    (int)camera.ScreenSize.X,
                    (int)camera.ScreenSize.Y
                );
                if (r.Contains(new Point((int)mousePosition.X, (int)mousePosition.Y)))
                {
                    Vector2 worldPos = camera.ScreenToWorldPosition(mousePosition);
                    milk.Core.Milk.Graphics.DrawString(
                        milk.Core.Milk.Assets.FontSmall,
                        "World pos: " + (int)worldPos.X + ", " + (int)worldPos.Y,
                        mousePosition + new Vector2(10, milk.Core.Milk.Assets.FontSmall.MeasureString("X").Y),
                        Color.White
                    );
                    break;
                }
            }

        }

        milk.Core.Milk.Graphics.End();

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
        return Entities.Contains(entity);
    }

    /// <summary>
    /// Adds an entity object to the scene.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public void AddEntity(Entity entity)
    {

        // only allow each entity to be added once
        if (Entities.Contains(entity) == false)
        {

            Entities.Add(entity);

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
        foreach (Camera camera in Cameras)
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
        foreach (Entity entity in Entities)
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
            if (Entities.Contains(entity))
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

                foreach (Camera camera in Cameras)
                {
                    if (camera.TrackedEntity == entity)
                        camera.TrackedEntity = null;
                }

                OnEntityRemoved(entity);
                Entities.Remove(entity);

            }
        }
        entitiesToRemoveFromScene.Clear();
    }

    /// <summary>
    /// Gets the entity with the provided (unique) name.
    /// The name supplied is converted to lower-case.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>An entity if one is found, or null.</returns>
    public Entity? GetEntityByName(string name)
    {
        return _entityManager.GetEntityByNameInList(Entities, name);
    }

    /// <summary>
    /// Gets all entities that include all specified tags.
    /// </summary>
    /// <param name="tags">One or more tags to check.</param>
    /// <returns>A list of all entities with all tags. This is always a list, and could be empty.</returns>
    public List<Entity> GetEntitiesByTag(params string[] tags)
    {
        return _entityManager.GetEntitiesByTagInList(Entities, tags);
    }

    /// <summary>
    /// Gets all entities that match the specified type.
    /// </summary>
    /// <param name="type">The entity type to get.</param>
    /// <returns>A list of all entities of the specified type.</returns>
    public List<Entity> GetEntitiesByType(string type)
    {
        return _entityManager.GetEntitiesByTypeInList(Entities, type);
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
        foreach (Entity entity in Entities)
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
        foreach (Entity entity in Entities)
        {
            if (entity.HasTag(tags))
                entity.Delete = true;
        }
    }

    /// <summary>
    /// Marks all entities containing the specified type for deletion.
    /// The type is checked in lower-case.
    /// </summary>
    /// <param name="type">The entity type to remove.</param>
    public void DeleteEntitiesByType(string type)
    {
        foreach (Entity entity in Entities)
        {
            if (entity.Type == type)
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

    /// <summary>
    /// Generates a string showing all scene systems.
    /// </summary>
    /// <returns>A string list of all systems.</returns>
    public string PrintSystems()
    {
        return milk.Core.Milk.Systems.PrintSystemsInList(systems);
    }

    /// <summary>
    /// Positions the system of the specified type at the position specified.
    /// This is useful for altering the order in which systems execute.
    /// (Use `PrintSystems()` to see the order of systems.)
    /// </summary>
    /// <typeparam name="T">The type of system to move.</typeparam>
    /// <param name="position">The new position of the system.</param>
    public void SetSystemPosition<T>(int position)
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
            if (
                property == null ||
                (property != null && layer.Properties.TryGetValue(property, out string propValue) && propValue == value)
            )
                _mapRenderer.Draw(layer, camera.GetTransformMatrix());
        }

    }

    public void DrawMapLayers(Camera camera, List<TiledMapLayer> layerList)
    {
        if (_mapRenderer == null)
            return;
        
        foreach (TiledMapLayer layer in layerList)
            _mapRenderer.Draw(layer, camera.GetTransformMatrix());
    }

    /// <summary>
    /// Filters map tile layers based on scene depth and custom property criteria.
    /// </summary>
    public List<TiledMapLayer> GetFilteredMapLayers(
        bool includeBelow = true, 
        bool includeAbove = true,
        Dictionary<string, string>? mustHave = null,
        Dictionary<string, string>? mustNotHave = null)
    {
        var filteredLayers = new List<TiledMapLayer>();
        if (_map == null) return filteredLayers;

        foreach (var layer in _map.TileLayers)
        {
            // 1. Scene Depth Logic (The "Base" Filter)
            bool hasBelowProp = layer.Properties.TryGetValue("belowEntities", out string belowValue);
            
            // Layers with no property are 'below' by default
            bool isBelowLayer = !hasBelowProp || belowValue == "true";
            bool isAboveLayer = hasBelowProp && belowValue == "false";

            if (isBelowLayer && !includeBelow) continue;
            if (isAboveLayer && !includeAbove) continue;

            // 2. Custom Inclusion Logic
            if (mustHave != null && MatchAll(layer.Properties, mustHave) == false) 
                continue;

            // 3. Custom Exclusion Logic
            if (mustNotHave != null && MatchAny(layer.Properties, mustNotHave)) 
                continue;

            filteredLayers.Add(layer);
        }

        return filteredLayers;
    }

    // Helper to keep the main loop clean
    private bool MatchAll(TiledMapProperties props, Dictionary<string, string> criteria)
    {
        foreach (var kvp in criteria)
            if (!props.TryGetValue(kvp.Key, out string val) || val != kvp.Value) return false;
        return true;
    }

    private bool MatchAny(TiledMapProperties props, Dictionary<string, string> criteria)
    {
        foreach (var kvp in criteria)
            if (props.TryGetValue(kvp.Key, out string val) && val == kvp.Value) return true;
        return false;
    }


public string? GetTilePropertyAt(Vector2 worldPosition, List<TiledMapLayer> layers, string propertyName)
{
    if (_map == null || layers == null) return null;

    int x = (int)(worldPosition.X / _map.TileWidth);
    int y = (int)(worldPosition.Y / _map.TileHeight);

    if (x < 0 || y < 0 || x >= _map.Width || y >= _map.Height) return null;

    // Loop backwards through the list to check the 'top' layer first
    for (int i = layers.Count - 1; i >= 0; i--)
    {
        TiledMapLayer layer = layers[i];

        if (layer is TiledMapTileLayer tileLayer)
        {
            if (tileLayer.TryGetTile((ushort)x, (ushort)y, out TiledMapTile? tile))
            {
                // If the tile is null or blank, it doesn't 'exist' here, so keep looking down
                if (tile == null || tile.Value.IsBlank) continue;

                // WE FOUND A TILE. 
                uint gid = (uint)tile.Value.GlobalIdentifier;

                foreach (var tileset in _map.Tilesets)
                {
                    uint firstGid = (uint)_map.GetTilesetFirstGlobalIdentifier(tileset);
                    
                    if (gid >= firstGid && gid < firstGid + (uint)tileset.TileCount)
                    {
                        int localId = (int)(gid - firstGid);
                        
                        // Find the metadata for this specific tile
                        TiledMapTilesetTile? tilesetTile = null;
                        for (int j = 0; j < tileset.Tiles.Count; j++)
                        {
                            if (tileset.Tiles[j].LocalTileIdentifier == localId)
                            {
                                tilesetTile = tileset.Tiles[j];
                                break;
                            }
                        }

                        // Return the property value if it exists, otherwise return null.
                        // Either way, the method ends here because we found the top-most tile.
                        if (tilesetTile != null && tilesetTile.Properties.TryGetValue(propertyName, out string? value))
                        {
                            return value;
                        }
                        
                        // Tile found but no property exists on it
                        return null;
                    }
                }
                
                // If we reach here, we found a tile but it's not in any tileset (shouldn't happen)
                return null;
            }
        }
    }

    return null;
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
    /// Called once when a scene is first created.
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
