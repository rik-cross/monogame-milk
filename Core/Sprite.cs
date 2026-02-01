

namespace milk.Core;

internal class Sprite
{
    
    internal List<SpriteTextureList> spriteTextureList;

    /// <summary>
    /// The current texture index.
    /// </summary>
    internal int currentFrame;

    /// <summary>
    /// Maximum number of textures to display.
    /// </summary>
    internal int numberOfFrames;

    /// <summary>
    /// The total time for the animation loop.
    /// </summary>
    internal double duration;

    /// <summary>
    /// The time to display each texture.
    /// </summary>
    internal double timePerFrame;

    /// <summary>
    /// The elapsed time since the texture became current.
    /// </summary>
    internal double timeOnCurrentFrame;

    /// <summary>
    /// If true, the size of the sprite is the same as the
    /// entity size, stored in the TransformComponent.
    /// </summary>
    internal bool resizeToEntity;

    /// <summary>
    /// Repeatedly loop the animation.
    /// </summary>
    internal bool Loop { get; set; }

    internal Sprite(
        List<SpriteTextureList> spriteTextureList,
        double duration = 0,
        bool loop = true,
        bool resizeToEntity = false
    )
    {
        this.spriteTextureList = spriteTextureList;
        this.duration = duration;
        this.Loop = loop;
        this.resizeToEntity = resizeToEntity;
        CalculateFrames();
    }

    internal void CalculateFrames()
    {
        if (spriteTextureList == null || spriteTextureList.Count == 0)
            return;

        // Calculate longest sprite list
        int highestTextureListLength = 0;
        foreach(SpriteTextureList stl in spriteTextureList)
        {
            if (stl.textureList.Count > highestTextureListLength)
            {
                highestTextureListLength = stl.textureList.Count;
            }
        }

        // Set the time per frame accordingly
        timePerFrame = duration / highestTextureListLength;

        // Set the maximum number of frames
        numberOfFrames = highestTextureListLength;

    }

}