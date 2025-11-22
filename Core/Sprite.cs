using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Core;

public class Sprite
{

    public List<Texture2D> textureList;
    public Color hue;
    public Vector2 offset;
    public bool flipH;
    public bool flipV;
    public Vector2 scale;

    // animated stuff
    public int numberOfFrames;
    public int currentFrame;
    public double duration;
    public double timePerFrame;
    public double timeOnCurrentFrame;
    public bool resizeToEntity = true;
    public bool loop;
    
    public Sprite(
        List<Texture2D> textureList,
        //float duration = default,
        bool resizeToEntity = true,
        double duration = 0,
        Vector2 offset = default,
        Vector2 scale = default,
        bool loop = true,
        Color? hue = null,
        bool flipH = false,
        bool flipV = false
    )
    {
        //
        this.textureList = textureList;
        this.resizeToEntity = resizeToEntity;
        this.duration = duration;

        if (offset == default)
            offset = Vector2.Zero;
        else
            this.offset = offset;

        if (scale == default)
            this.scale = Vector2.One;
        else
            this.scale = scale;

        this.loop = loop;
        
        this.hue = hue ?? Color.White;

        this.flipH = flipH;
        this.flipV = flipV;

        //
        currentFrame = 0;

        // 
        UpdateAnimationInfo();
    }

    public void UpdateAnimationInfo()
    {
        numberOfFrames = textureList.Count;
        timePerFrame = duration / numberOfFrames;
    }

    public Texture2D GetCurrentTexture()
    {
        return textureList[currentFrame];
    }

}