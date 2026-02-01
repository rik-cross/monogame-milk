//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

// This is the public 'API' for the engine. Instead
// of allowing the user to call game.SetScene(), etc.
// they will instead call Milk.Scenes.SetScene().
// This means that the top-level Game class does not
// need to be exposed, which would expose methods like
// Update(), Draw(), LoadContent(), etc. which are
// not intended to be used directly in this engine.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace milk.Core;

public static class Milk
{
    
    public static SceneManager Scenes { get; } = EngineGlobals.game.sceneManager;
    public static SystemManager Systems { get; } = EngineGlobals.game.systemManager;
    public static InputManager Controls { get; } = EngineGlobals.game.inputManager;
    public static SpriteBatch Graphics { get; } = EngineGlobals.game.spriteBatch;
    public static ContentManager Content { get; } = EngineGlobals.game.Content;
    public static Vector2 Size { get; } = EngineGlobals.game.Size;
    public static void Quit() => EngineGlobals.game.Quit();

}