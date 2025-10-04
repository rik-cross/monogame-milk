/*
   milk, By Rik Cross
   -- github.com/rik-cross/monogame-milk
   -- Shared under the MIT licence
*/

using System.Collections;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk;

public abstract class System {
    private ComponentManager _componentManager = EngineGlobals.game.componentManager;
    //private SystemManager _systemManager = EngineGlobals.game.systemManager;
    protected SpriteBatch spriteBatch = EngineGlobals.game.spriteBatch;


    private bool _drawAboveEntities = true;
    public bool DrawAboveEntities
    {
        get
        {
            return _drawAboveEntities;
        }
        set
        {
            _drawAboveEntities = value;
            if (value == false)
                _drawAboveMap = false;
        }
    }

    public bool _drawAboveMap = true;
    public bool DrawAboveMap
    {
        get
        {
            return _drawAboveMap;
        }
        set
        {
            _drawAboveMap = value;
            if (value == true)
                _drawAboveEntities = true;
        }
    }

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
    public virtual void Input(GameTime gameTime, Scene scene) { }
    public virtual void InputEntity(GameTime gameTime, Scene scene, Entity entity) { }    
    public virtual void Update(GameTime gameTime, Scene scene) { }
    public virtual void UpdateEntity(GameTime gameTime, Scene scene, Entity entity) { }
    public virtual void Draw(Scene scene) {}
    public virtual void DrawEntity(Scene scene, Entity entity) {}
    
}