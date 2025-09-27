using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk;

public class Sprite
{

    public List<Texture2D> textureList;
    private bool play;
    private Vector2 size;
    private float alpha;
    private Color hue;
    public Vector2 offset;
    private bool flipH;
    private bool flipV;
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
        bool loop = true
    )
    {
        //
        this.textureList = textureList;
        this.resizeToEntity = resizeToEntity;
        this.duration = duration;

        // offset * scale 
        if (offset == default)
            offset = Vector2.Zero;
        else
            this.offset = offset;

        if (scale == default)
            this.scale = Vector2.One;
        else
            this.scale = scale;

        this.loop = loop;

        //
        currentFrame = 0;

        // 
        UpdateAnimationInfo();
    }

    public void UpdateAnimationInfo()
    {
        numberOfFrames = textureList.Count;
        timePerFrame = duration / numberOfFrames;
        //Console.WriteLine(timePerFrame);
    }

    public Texture2D GetCurrentTexture()
    {
        return textureList[currentFrame];
    }

}