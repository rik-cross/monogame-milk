//   [ ] Finished
//   
//   Monogame ECS Engine
//   By Rik Cross
//   -- github.com/rik-cross/monogame-ecs
//   Shared under the MIT licence
//
//   ------------------------------------
//
//   MonogameECS.Scene
//   ==================
//  
//   A scene runs a set of systems on a set of entities
//   (with the required components).

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameECS;

public abstract class Scene {

    // Local references to global objects
    protected ContentManager content = EngineGlobals.game.content;
    protected Game game = EngineGlobals.game;
    private EntityManager _entityManager = EngineGlobals.game.entityManager;
    private SystemManager _systemManager = EngineGlobals.game.systemManager;
    private SceneManager _sceneManager = EngineGlobals.game.sceneManager;
    private GraphicsDevice _graphicsDevice = EngineGlobals.game.graphicsDevice;
    protected SpriteBatch spriteBatch = EngineGlobals.game.spriteBatch;
    public readonly Vector2 Size;
    public readonly Vector2 Middle;
    public float elapsedTime;
    public List<Entity> entities;
    public List<System> systems;
    public static Texture2D whiteRectangle;
    public Color backgroundColor = Color.Transparent;
    public bool IncludeAlRegisteredSystems = true;
    protected List<Camera> cameras = new List<Camera>();
    public bool UpdateSceneBelow = true;
    public bool InputSceneBelow = false;
    public bool DrawSceneBelow = true;
    public List<TimedAction> timedActions = new List<TimedAction>();

    public Vector2? mapSize = null;

    public void AddTimedAction(float elapsedTime, Action<GameTime> action, string name = null)
    {
        timedActions.Add(new TimedAction(elapsedTime, action, name));
    }

    public void RemoveTimedActionByName(string name)
    {
        for (int i = timedActions.Count - 1; i >= 0; i--)
        {
            if (timedActions[i].Name.ToLower() == name.ToLower())
                timedActions.RemoveAt(i);
        }
    }

    public Scene()
    {


        Size = EngineGlobals.game._size;

        Middle = new Vector2(Size.X / 2, Size.Y / 2);

        elapsedTime = 0f;
        entities = new List<Entity>();
        systems = new List<System>();
        _sceneManager.allScenes.Add(this);

        whiteRectangle = new Texture2D(_graphicsDevice, 1, 1);
        whiteRectangle.SetData(new[] { Color.White });

        if (IncludeAlRegisteredSystems == true)
            _systemManager.AddAllRegisteredSystemsToScene(this);

        Init();
    }

    internal void _Update(GameTime gameTime, List<Scene> scenes) {

        // update scene below
        if (UpdateSceneBelow == true && GetSceneBelow(scenes) != null)
            GetSceneBelow(scenes)._Update(gameTime, scenes);

        elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

        // Update entity State here??
        foreach (Entity e in entities)
        {
            e._previousState = e.State;
        }

        // Process the timed actions
        for (int i = timedActions.Count - 1; i >= 0; i--)
        {
            if (elapsedTime >= timedActions[i].ElapsedTime)
            {
                timedActions[i].Execute(gameTime);
                timedActions.RemoveAt(i);
            }
        }

        // high-level update
        foreach (System system in systems)
        {
            system.Update(gameTime, this);
        }

        // entity-specific update
        foreach (System system in systems)
        {
            foreach (Entity entity in entities)
            {
                BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                temp.And(entity.bitMask);
                if (Utils.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
                    system.UpdateEntity(gameTime, this, entity);
            }
        }

        foreach (Camera camera in cameras)
            camera.Update(gameTime, this);

        Update(gameTime);
        
        // delete all entities
        EngineGlobals.game.entityManager.DeleteEntities();
        
    }

    public void _Input(GameTime gameTime, List<Scene> scenes) {

        // Process the scene below's input
        if (GetSceneBelow(scenes) != null && InputSceneBelow == true)
            GetSceneBelow(scenes)._Input(gameTime, scenes);

        // Call the user-defined Input method
        Input(gameTime);
        
    }

    public void _Draw(RenderTarget2D renderTarget, List<Scene> scenes, bool drawSceneBelow = true)
    {

        _graphicsDevice.SetRenderTarget(renderTarget);

        if (drawSceneBelow == true && GetSceneBelow(scenes) != null)
            GetSceneBelow(scenes)._Draw(renderTarget, scenes);

        // add lighting shader here...

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        //spriteBatch.Begin(blendState: BlendState.AlphaBlend, effect: lightingShader);
        //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, lightingShader);

        // Scene background
        spriteBatch.Draw(whiteRectangle, new Rectangle(0, 0, (int)Size.X, (int)Size.Y), backgroundColor);

        spriteBatch.End();

        // CAMERA loop,

        foreach (Camera camera in cameras)
        {

            _graphicsDevice.Viewport = camera.getViewport();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(whiteRectangle, new Rectangle(0, 0, (int)Size.X, (int)Size.Y), camera.BackgroundColor);
            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.getTransformMatrix());

            // Scene map

            if (mapSize.HasValue)
                spriteBatch.Draw(whiteRectangle, new Rectangle(0, 0, (int)mapSize.Value.X, (int)mapSize.Value.Y), Color.Black * 0.2f);


            // high-level draw for those above entities
            foreach (System system in systems)
            {
                if (system.DrawAboveEntities == false)
                {
                    system.Draw(this);
                }
            }

            // Draw below entities
            // entity-specific update
            foreach (System system in systems)
            {

                foreach (Entity entity in entities)
                {
                    BitArray temp = (BitArray)system.RequiredComponentBitMask.Clone();
                    temp.And(entity.bitMask);
                    if (Utils.CompareBitArrays(temp, system.RequiredComponentBitMask) == true)
                    {
                        system.DrawEntity(this, entity);
                    }
                }
            }

            spriteBatch.End();

            _graphicsDevice.Viewport = new Viewport(0, 0, (int)Size.X, (int)Size.Y);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            // Camera border
            if (camera.BorderWidth > 0)
                spriteBatch.DrawRectangle(
                    new RectangleF(
                        camera.ScreenPosition.X - camera.BorderWidth,
                        camera.ScreenPosition.Y - camera.BorderWidth,
                        camera.ScreenSize.X + camera.BorderWidth,
                        camera.ScreenSize.Y + camera.BorderWidth
                    ),
                    color: camera.BorderColor,
                    thickness: camera.BorderWidth
                );

            // Lighting is applied now
            spriteBatch.End();
        }

        _graphicsDevice.Viewport = new Viewport(0, 0, (int)Size.X, (int)Size.Y);

        // no shader here
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // high-level draw for those above entities
        foreach (System system in systems)
        {
            if (system.DrawAboveEntities == true)
            {
                system.Draw(this);
            }
        }

        Draw();

        spriteBatch.End();
        
    }

    //
    // Cameras
    //
    protected void AddCamera(Camera camera)
    {
        cameras.Add(camera);
    }

    // Returns a camera with the specified name, and returns the
    // first camera with the name if more than one exists
    protected Camera GetCameraByName(string name)
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

    public void AddEntity(Entity entity)
    {
        if (entities.Contains(entity) == false)
        {
            entities.Add(entity);
            OnEntityAdded(entity);
        }
    }

    public bool HasEntity(Entity entity)
    {
        return entities.Contains(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        if (entities.Contains(entity) == true)
        {
            OnEntityRemoved(entity);
            entities.Remove(entity);
        }
    }

    public void RemoveAllEntities()
    {
        entities.Clear();
    }

    public Entity GetEntityByName(string name)
    {
        return _entityManager.GetEntityByNameInList(entities, name);
    }

    public List<Entity> GetEntitiesByTag(string tag)
    {
        return _entityManager.GetEntitiesByTagInList(entities, tag);
    }

    public void RemoveEntityByName(string name)
    {
        _entityManager.RemoveEntityByNameInList(entities, name);
    }

    public void RemoveEntitiesByTag(params string[] tags)
    {
        _entityManager.RemoveEntitiesByTagInList(entities, tags);
    }

    //
    // System methods
    //

    // Add a (registered) system type to a scene (Calling this
    // method isn't required if IncludeAlRegisteredSystems == true)
    public void AddSystem<T>() where T : System
    {
        // Can only add registered systems to a scene
        if (_systemManager.IsRegistered<T>() == false)
            return;

        // Add the instance of the specified type
        systems.Add(_systemManager.GetSystem<T>());
    }

    // Removes the first/only instance of the specified type
    // in the systems list, defering to the system manager's method
    public void RemoveSystem<T>() where T : System
    {
        _systemManager.RemoveSystemOfTypeFromList<T>(systems);
    }

    // Removes all systems from the scene
    public void RemoveAllSystems()
    {
        systems.Clear();
    }

    // Defers to the system manager's method, and returns true
    // If a system of the specified type exists in the list of
    // scene systems
    public bool HasSystem<T>() where T : System
    {
        return _systemManager.HasSystemOfTypeInList<T>(systems);
    }

    public Scene GetSceneBelow(List<Scene> scenes) {
        for (int i=0; i < scenes.Count - 1; i++) {
            if (scenes[i] == this) {
                return scenes[i+1];
            }
        }
        return null;
    }

    public virtual void Init() {}
    public virtual void Update(GameTime gameTime) {}
    public virtual void Input(GameTime gameTime) {}
    public virtual void Draw() {}
    public virtual void OnEnter() {}
    public virtual void OnExit() {}
    public virtual void OnEntityAdded(Entity entity) {}
    public virtual void OnEntityRemoved(Entity entity) {}

}