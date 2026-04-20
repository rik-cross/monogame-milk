//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;

namespace milk.Core;

/// <summary>
/// Nameable objects must have a Name property.
/// </summary>
public interface INameable
{
    string? Name { get; set; }
}

/// <summary>
/// Visible objects must have a Visible property.
/// </summary>
public interface IVisible
{
    bool Visible { get; set; }
}

/// <summary>
/// SceneParent objects must have a Parent property.
/// </summary>
public interface ISceneParent
{
    Scene ParentScene { get; set; }
}

/// <summary>
/// Updateable objects must have an Update() method
/// </summary>
public interface IUpdateable
{
    void Update(GameTime gameTime);
}

/// <summary>
/// Drawable objects must have a Draw() method
/// </summary>
public interface IDrawable
{
    void Draw();
}