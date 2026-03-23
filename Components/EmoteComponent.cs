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
/// An emote component displays one or more textures above an entity.
/// </summary>
public class EmoteComponent : Component
{
    
    /// <summary>
    /// The texture to display, or animate for more than one texture.
    /// </summary>
    public List<Texture2D> TextureList { get; private set; } = new List<Texture2D>();
    
    /// <summary>
    /// The size of the texture to display.
    /// </summary>
    public Vector2 Size;

    /// <summary>
    /// The margin to add between the texture and the border.
    /// </summary>
    public int Margin;

    /// <summary>
    /// The width of the border.
    /// </summary>
    public int BorderWidth;

    /// <summary>
    /// The background color.
    /// </summary>
    public Color BackgroundColor;

    /// <summary>
    /// The border color.
    /// </summary>
    public Color BorderColor;

    /// <summary>
    /// The time taken (in seconds) to fade in/out.
    /// </summary>
    public double FadeDuration;

    /// <summary>
    /// Allows the use of a custom draw method.
    /// </summary>
    public Action<Entity>? CustomDrawMethod;

    /// <summary>
    /// The current transparency value.
    /// </summary>
    public float CurrrentAlpha;

    // Target transparency value.
    internal float TargetAlpha;
    
    private bool _visible;
    /// <summary>
    /// Shows / hides the component.
    /// </summary>
    public bool Visible
    {
        get { return _visible; }
        set
        {
            _visible = value;
            if (_visible == true)
                TargetAlpha = 1;
            else
                TargetAlpha = 0;
        }
    }
    
    //
    // Animation information
    //

    /// <summary>
    /// The current frame.
    /// </summary>
    public int Index;

    /// <summary>
    /// Sets the animation duration.
    /// </summary>
    public double Duration { get; internal set; }

    // Duration to display each frame
    internal double TimePerFrame;

    // Time on the current frame
    internal double CurrentTime;

    /// <summary>
    /// Animation looping (if more than one texture).
    /// </summary>
    public bool Loop { get; set; }

    /// <summary>
    /// Create a new emote component with a single texture.
    /// </summary>
    /// <param name="texture">The texture to display.</param>
    /// <param name="size">The (x, y) size of the texture (default = null - use texture size).</param>
    /// <param name="margin">The spacing between the texture and the border (default = 4).</param>
    /// <param name="borderWidth">Border thickness (default = 1).</param>
    /// <param name="backgroundColor">The backgrund color (default = null - Color.White).</param>
    /// <param name="borderColor">The border color (default = null - Color.Black).</param>
    /// <param name="fadeDuration">The time taken (in seconds) to fade in/out (default = 0.25s).</param>
    /// <param name="customDrawMethod">Custom draw method (default = null - use draw method provided).</param>
    /// <param name="visible">Visibility of the emote (default = true - visible).</param>
    public EmoteComponent(
        Texture2D texture,
        Vector2? size = null,
        int margin = 4,
        int borderWidth = 1,
        Color? backgroundColor = null,
        Color? borderColor = null,
        double fadeDuration = 0.25,
        Action<Entity>? customDrawMethod = null,
        bool visible = true
    )
    {
        _CreateComponent(
            new List<Texture2D>(){texture},
            size,
            margin,
            borderWidth,
            backgroundColor,
            borderColor,
            fadeDuration,
            customDrawMethod,
            visible,
            0,
            false
        );
    }

    /// <summary>
    /// Create a new animated emote component with multiple textures.
    /// </summary>
    /// <param name="textureList">The textures to display.</param>
    /// <param name="size">The (x, y) size of the texture (default = null - use texture size).</param>
    /// <param name="margin">The spacing between the texture and the border (default = 4).</param>
    /// <param name="borderWidth">Border thickness (default = 1).</param>
    /// <param name="backgroundColor">The backgrund color (default = null - Color.White).</param>
    /// <param name="borderColor">The border color (default = null - Color.Black).</param>
    /// <param name="fadeDuration">The time taken (in seconds) to fade in/out (default = 0.25s).</param>
    /// <param name="customDrawMethod">Custom draw method (default = null - use draw method provided).</param>
    /// <param name="visible">Visibility of the emote (default = true - visible).</param>
    /// <param name="duration">The time taken per animation loop (default = 1s).</param>
    /// <param name="loop">Sets whether the animation should loop, or play once (default = true).</param>
    public EmoteComponent(
        List<Texture2D> textureList,
        Vector2? size = null,
        int margin = 4,
        int borderWidth = 1,
        Color? backgroundColor = null,
        Color? borderColor = null,
        double fadeDuration = 0.25,
        Action<Entity>? customDrawMethod = null,
        bool visible = true,
        double duration = 1,
        bool loop = true
    )
    {
        _CreateComponent(
            textureList,
            size,
            margin,
            borderWidth,
            backgroundColor,
            borderColor,
            fadeDuration,
            customDrawMethod,
            visible,
            duration,
            loop
        );
    }

    // This method is called by both constructors,
    // and completes all the required setup
    private void _CreateComponent(
        List<Texture2D> textureList,
        Vector2? size = null,
        int margin = 4,
        int borderWidth = 1,
        Color? backgroundColor = null,
        Color? borderColor = null,
        double fadeDuration = 0.25f,
        Action<Entity>? customDrawMethod = null,
        bool visible = true,
        double duration = 0,
        bool loop = true
    )
    {
        TextureList = textureList;
        Size = size ?? new Vector2(textureList[0].Width, textureList[0].Height);
        Margin = margin;
        BorderWidth = borderWidth;
        BackgroundColor = backgroundColor ?? Color.White;
        BorderColor = borderColor ?? Color.Black;
        FadeDuration = fadeDuration;
        CustomDrawMethod = customDrawMethod;
        Visible = visible;
        CurrrentAlpha = Visible == true ? 1 : 0;
        TargetAlpha = CurrrentAlpha;
        Index = 0;
        Duration = duration;
        TimePerFrame = Duration / TextureList.Count;
        CurrentTime = 0;
        Loop = loop;
    }

}