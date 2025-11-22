//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: github.com/rik-cross/milk-docs
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework.Graphics;
using milk.Core;

namespace milk.Components;

/// <summary>
/// Associates one or more textures (in a `Sprite`) with a state.
/// </summary>
public class SpriteComponent : Component
{

    private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    
    /// <summary>
    /// Plays / pauses the sprite's animation.
    /// </summary>
    public bool Play { get; set; }

    /// <summary>
    /// Simple creation of a sprite component with a single image.
    /// </summary>
    /// <param name="texture"></param>
    public SpriteComponent(Texture2D? texture = null, string state = "default")
    {
        if (texture != null)
        {
            AddSprite(
                new Sprite(new List<Texture2D>() { texture }),
                state: state
            );
        }
        this.Play = true;
    }

    /// <summary>
    /// Add a sprite (1 or more textures) to the component.
    /// </summary>
    /// <param name="sprite">A sprite object.</param>
    /// <param name="state">An optional state to associate the sprite with.</param>
    public void AddSprite(Sprite sprite, string state = "default")
    {
        sprites[state] = sprite;
    }

    /// <summary>
    /// Returns 'true' if a sprite exists for the specified state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns>A bool, true if a sprite exists.</returns>
    public bool HasSpriteForState(string state)
    {
        return sprites.ContainsKey(state) == true && sprites[state] != null;
    }

    /// <summary>
    /// Returns the sprite object for the specified state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns>A sprite object, or null.</returns>
    public Sprite? GetSpriteForState(string state)
    {
        return sprites[state];
    }

    //public Texture2D GetCurrentTexture(string state = "default")
    //{   
    //}

}