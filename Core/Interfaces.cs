//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;

namespace milk.Core;

public interface INameable
{
    string? Name { get; set; }
}

public interface IVisible
{
    bool Visible { get; set; }
}

public interface ISceneParent
{
    Scene ParentScene { get; set; }
}

public interface IUpdateable
{
    void Update(GameTime gameTime);
}

public interface IDrawable
{
    void Draw();
}