//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

namespace milk.Core;

/// Defines a collidable area of a scene.
internal struct SceneCollider
{
    internal float X, Y, Width, Height;
    internal SceneCollider(float x, float y, float w, float h) 
    { 
        X = x; Y = y; Width = w; Height = h;
    }
}