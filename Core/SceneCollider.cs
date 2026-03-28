//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;

namespace milk.Core;

/// Defines a collidable area of a scene.
public class SceneCollider : ISceneParent, INameable
{
    public Scene? ParentScene { get; set; }
    // TODO unique names
    
    private string? _name;
    /// <summary>
    /// The name of the collider, unique to a scene.
    /// Names are stored trimmed and in lowercase.
    /// </summary>
    public string? Name
    {
        get
        { return _name; }
        set
        {
            if (value == null)
                _name = value;
            else
                _name = value.Trim().ToLower();
        }
    }

    public Vector2 Position;
    public Vector2 Size;
    public SceneCollider(Vector2 position, Vector2 size, string? name = null) 
    { 
        Position = position;
        Size = size;
        Name = name;
    }

}