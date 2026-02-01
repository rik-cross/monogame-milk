//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

// This is the public engine 'API', but exposed
// through classes which subclass this class.
// For example, Scene is a subclass of MilkMethods
// so the user can call Scenes.SetSCene... or
// Controls.IsKeyPressed, without having to call
// the top-level Milk.Scenes, Milk.Controls, etc.

using Microsoft.Xna.Framework;

namespace milk.Core;

public abstract class MilkMethods
{
    protected milk.Core.SceneManager Scenes => milk.Core.Milk.Scenes;
    protected milk.Core.SystemManager Systems => milk.Core.Milk.Systems;
    protected milk.Core.InputManager Controls => milk.Core.Milk.Controls;
    protected Microsoft.Xna.Framework.Graphics.SpriteBatch Graphics => milk.Core.Milk.Graphics;
    protected Microsoft.Xna.Framework.Content.ContentManager Content => milk.Core.Milk.Content;
    protected Vector2 Size => milk.Core.Milk.Size;
    protected void Quit() => milk.Core.Milk.Quit();
}