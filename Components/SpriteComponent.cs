//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using milk.Core;

namespace milk.Components;

/// <summary>
/// Associates one or more texture lists with a state, 
/// along with some associated preferences for each list.
/// </summary>
public class SpriteComponent : Component
{

    // Links Sprites to states
    private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    
    /// <summary>
    /// Plays / pauses the sprite's animation.
    /// </summary>
    public bool Play { get; set; } = true;

    /// <summary>
    /// Creates an empty SpriteComponent.
    /// </summary>
    public SpriteComponent()
    {
        sprites = new Dictionary<string, Sprite>();
    }

    /// <summary>
    /// Creates a new SpriteComponent containing a single texture.
    /// </summary>
    /// <param name="texture">The texture to display.</param>
    /// <param name="state">The state to associate the texture with (default = "default" state).</param>
    /// <param name="resizeToEntity">Resize the texture to match the entity's TransformComponent dimensions (default = false).</param>
    /// <param name="offset">The (x, y) position to offset the texture by (default = null - (0, 0)).</param>
    /// <param name="scale">The (w, h) scale factor for the texture (default = null - use original size).</param>
    /// <param name="hue">Add a color filter to the texture (default = null - Color.White (no filter)).</param>
    /// <param name="alpha">The texture transparency (default = 1.0f - fully visible).</param>
    /// <param name="flipH">Horizontally flip the texture (default = false).</param>
    /// <param name="flipV">Vertically flip the texture (default = false).</param>
    public SpriteComponent(
        Texture2D texture, 
        string state = "default",
        bool resizeToEntity = false,
        Vector2? offset = null,
        Vector2? scale = null,
        Color? hue = null,
        float alpha = 1.0f,
        bool flipH = false,
        bool flipV = false
    ) : this(
            new List<Texture2D>(){texture},
            state,
            0,
            resizeToEntity,
            offset,
            scale,
            hue,
            alpha,
            flipH,
            flipV
        ) {}

    /// <summary>
    /// Creates a new SpriteComponent containing a list of textures.
    /// </summary>
    /// <param name="textureList">The textures to display.</param>
    /// <param name="state">The state to associate the texture with (default = "default" state).</param>
    /// <param name="duration">The duration of a single animation cycle, in seconds (default = null - 0s).</param>
    /// <param name="resizeToEntity">Resize the texture to match the entity's TransformComponent dimensions (default = false).</param>
    /// <param name="offset">The (x, y) position to offset the texture by (default = null - (0, 0)).</param>
    /// <param name="scale">The (w, h) scale factor for the texture (default = null - use original size).</param>
    /// <param name="hue">Add a color filter to the texture (default = null - Color.White (no filter)).</param>
    /// <param name="alpha">The texture transparency (default = 1.0f - fully visible).</param>
    /// <param name="flipH">Horizontally flip the texture (default = false).</param>
    /// <param name="flipV">Vertically flip the texture (default = false).</param>
    public SpriteComponent(
        List<Texture2D> textureList, 
        string state = "default",
        double duration = 0,
        bool resizeToEntity = false,
        Vector2? offset = null,
        Vector2? scale = null,
        Color? hue = null,
        float alpha = 1.0f,
        bool flipH = false,
        bool flipV = false
    )
    {
        sprites = new Dictionary<string, Sprite>();
        sprites.Add(
            state,
            new Sprite(new List<SpriteTextureList>()
            {
                new SpriteTextureList(
                    textureList,
                    offset: offset,
                    scale: scale,
                    hue: hue,
                    alpha: alpha,
                    flipH: flipH,
                    flipV: flipV
                )
            },
            duration: duration,
            loop: true,
            resizeToEntity: resizeToEntity)
        );
        sprites[state].CalculateFrames();
    }

    /// <summary>
    /// Adds a single texture to a Sprite, as a new texture layer.
    /// </summary>
    /// <param name="texture">The texture to add.</param>
    /// <param name="state">The state to associate the texture with (default = "default").</param>
    /// <param name="duration">The game time per animation cycle (default = 0 - no animation).</param>
    /// <param name="loop">Loops the animation at the end of each cycle (default = true).</param>
    /// <param name="resizeToEntity">Sprite size is set to the entity size (default = true).</param>
    /// <param name="offset">The distance between the entity position and the sprite (default = none).</param>
    /// <param name="scale">The scale factor for the textures (default = 1 - no scale factor).</param>
    /// <param name="hue">The hue to recolor the sprite (default = white / no recolor).</param>
    /// <param name="alpha">The texture transparency (default = 1.0f - fully visible).</param>
    /// <param name="flipH">Horizontally flip the textures (default = false).</param>
    /// <param name="flipV">Vertically flip the textures (default = false).</param>
    public void AddTextures(
        Texture2D texture,
        string state = "default",
        double duration = 0,
        bool loop = true,
        bool resizeToEntity = false,
        Vector2? offset = null,
        Vector2? scale = null,
        Color? hue = null,
        float alpha = 1.0f,
        bool flipH = false,
        bool flipV = false
    )
    {
        AddTextures(
            new List<Texture2D>() {texture},
            state: state,
            duration: duration,
            loop: loop,
            resizeToEntity: resizeToEntity,
            offset: offset,
            scale: scale,
            hue: hue,
            alpha: alpha,
            flipH: flipH,
            flipV: flipV
        );
    }

    /// <summary>
    /// Adds a single texture to a Sprite, as a new texture layer.
    /// </summary>
    /// <param name="textureList">The textures to add.</param>
    /// <param name="state">The state to associate the texture with (default = "default").</param>
    /// <param name="duration">The game time per animation cycle (default = 0 - no animation).</param>
    /// <param name="loop">Loops the animation at the end of each cycle (default = true).</param>
    /// <param name="resizeToEntity">Sprite size is set to the entity size (default = true).</param>
    /// <param name="offset">The distance between the entity position and the sprite (default = none).</param>
    /// <param name="scale">The scale factor for the textures (default = 1 - no scale factor).</param>
    /// <param name="hue">The hue to recolor the sprite (default = white / no recolor).</param>
    /// <param name="alpha">The texture transparency (default = 1.0f - fully visible).</param>
    /// <param name="flipH">Horizontally flip the textures (default = false).</param>
    /// <param name="flipV">Vertically flip the textures (default = false).</param>
    public void AddTextures(
        List<Texture2D> textureList,
        string state = "default",
        double duration = 0,
        bool loop = true,
        bool resizeToEntity = false,
        Vector2? offset = null,
        Vector2? scale = null,
        Color? hue = null,
        float alpha = 1.0f,
        bool flipH = false,
        bool flipV = false
    )
    {
        if (sprites.ContainsKey(state) == false)
        {
            sprites.Add(
                state,
                new Sprite(
                    spriteTextureList: new List<SpriteTextureList>()
                    {
                        new SpriteTextureList(
                            textureList: textureList,
                            offset: offset,
                            scale: scale,
                            hue: hue,
                            alpha: alpha,
                            flipH: flipH,
                            flipV: flipV
                        )
                    },
                    duration: duration,
                    loop: loop,
                    resizeToEntity: resizeToEntity
                )
            );
            sprites[state].CalculateFrames();
        }
        else
        {
            sprites[state].spriteTextureList.Add(
                new SpriteTextureList(
                    textureList: textureList,
                    offset: offset,
                    scale: scale,
                    hue: hue,
                    alpha: alpha,
                    flipH: flipH,
                    flipV: flipV
                )
            );
            sprites[state].CalculateFrames();
        }
    }

    /// <summary>
    /// Clear textures.
    /// </summary>
    /// <param name="state">The state to clear (default = null - clear textures for all states).</param>
    public void ClearTextures(string? state = null)
    {
        if (state == null)
            sprites.Clear();
        else
            sprites.Remove(state);
    }

    // Get the Sprite object associated with a state
    internal Sprite? GetSpriteForState(string state)
    {
        if (sprites == null || sprites.ContainsKey(state) == false || sprites[state] == null)
            return null;
        
        return sprites[state];
    }

}