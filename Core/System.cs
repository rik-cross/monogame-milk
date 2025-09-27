//   [x] Finished
//   
//   Monogame ECS Engine
//   By Rik Cross
//   -- github.com/rik-cross/monogame-ecs
//   Shared under the MIT licence
//
//   ------------------------------------
//
//   MonogameECS.System
//   ==================
//  
//   A system acts upon all entities with the required set of components.

using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk;

public abstract class System {
    private ComponentManager _componentManager = EngineGlobals.game.componentManager;
    //private SystemManager _systemManager = EngineGlobals.game.systemManager;
    protected SpriteBatch spriteBatch = EngineGlobals.game.spriteBatch;
    public bool DrawAboveEntities = true;
    public BitArray RequiredComponentBitMask;
    public System() {
        RequiredComponentBitMask = new BitArray(_componentManager.maxComponents, false);
        Init();
    }

    // Adds the specified component type as a requirement
    // (Systems only run on entities with the required components)
    public void AddRequiredComponentType<T>() where T : Component
    {

        if (_componentManager.IsComponentTypeRegistered(typeof(T)) == false)
            _componentManager.RegisterComponentType(typeof(T));

        int componentID = _componentManager.GetComponentTypeID(typeof(T));
        RequiredComponentBitMask.Set(componentID, true);

    }

    // Optional methods
    public virtual void Init() { }
    public virtual void Update(GameTime gameTime, Scene scene) {}
    public virtual void UpdateEntity(GameTime gameTime, Scene scene, Entity entity) { }
    public virtual void Draw(Scene scene) {}
    public virtual void DrawEntity(Scene scene, Entity entity) {}
    
}