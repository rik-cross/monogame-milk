//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Core;

/// <summary>
/// A sprite is added to a SpriteComponent, and is a collection 
/// of textures, along with some additional data. A sprite with more
/// than one texture automatically becomes an animated sprite.
/// </summary>
public class Sprite
{

    /// <summary>
    /// A list of one or more textures to display.
    /// </summary>
    public List<Texture2D> textureList;

    /// <summary>
    /// Sprite hue.
    /// </summary>
    public Color hue;

    /// <summary>
    /// The sprite offset from the entity position.
    /// </summary>
    public Vector2 offset;

    /// <summary>
    /// Horizontally flip the sprite.
    /// </summary>
    public bool flipH;

    /// <summary>
    /// Vertically flip the sprite.
    /// </summary>
    public bool flipV;

    /// <summary>
    /// Texture scaling.
    /// </summary>
    public Vector2 scale;

    // animated stuff

    /// <summary>
    /// The number of textures.
    /// </summary>
    public int numberOfFrames;

    /// <summary>
    /// The current texture index.
    /// </summary>
    public int currentFrame;

    /// <summary>
    /// The total time for the animation loop.
    /// </summary>
    public double duration;

    /// <summary>
    /// The time to display each texture.
    /// </summary>
    public double timePerFrame;

    /// <summary>
    /// The elapsed time since the texture became current.
    /// </summary>
    public double timeOnCurrentFrame;

    /// <summary>
    /// If true, the size of the sprite is the same as the
    /// entity size, stored in the TransformComponent.
    /// </summary>
    public bool resizeToEntity;

    /// <summary>
    /// Repeatedly loop the animation.
    /// </summary>
    public bool loop;
    
    /// <summary>
    /// Creates a new sprite object.
    /// </summary>
    /// <param name="textureList">One or more textures to display.</param>
    /// <param name="resizeToEntity">Sprite size is set to the entity size (default = true).</param>
    /// <param name="duration">The total time to display the animation sequence (default = 0).</param>
    /// <param name="offset">The distance between the entity position and the sprite (default = none).</param>
    /// <param name="scale">The scale factor for the textures (default = 1 - no scale factor).</param>
    /// <param name="loop">Loop the textures (default = true).</param>
    /// <param name="hue">The hue to recolor the sprite (default = white / no recolor).</param>
    /// <param name="flipH">Horizontally flip the textures (default = false).</param>
    /// <param name="flipV">Vertically flip the textures (default = false).</param>
    public Sprite(
        List<Texture2D> textureList,
        bool resizeToEntity = true,
        double duration = 0,
        Vector2? offset = null,
        Vector2? scale = null,
        bool loop = true,
        Color? hue = null,
        bool flipH = false,
        bool flipV = false
    )
    {

        this.textureList = textureList;
        this.resizeToEntity = resizeToEntity;
        this.duration = duration;

        this.offset = offset ?? Vector2.Zero;
        this.scale = scale ?? Vector2.One;

        this.loop = loop;
        
        this.hue = hue ?? Color.White;

        this.flipH = flipH;
        this.flipV = flipV;

        currentFrame = 0;

        UpdateAnimationInfo();
    }

    private void UpdateAnimationInfo()
    {
        numberOfFrames = textureList.Count;
        timePerFrame = duration / numberOfFrames;
    }

    /// <summary>
    /// Returns the currently displayed texture.
    /// </summary>
    /// <returns>The currently displayed texture.</returns>
    public Texture2D GetCurrentTexture()
    {
        return textureList[currentFrame];
    }

}