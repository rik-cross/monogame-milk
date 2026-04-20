//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using milk.Components;

namespace milk.Core;

/// <summary>
/// A marker points to a 'point of interest' within a scene,
/// which is either an entity, or a position in the world. When
/// pointing to an entity, the marker points to the entity
/// center-top position. When a point of interest is outside a
/// camera's viewport, the marker sticks to the edge of the camera
/// and points in the direction of the entity or position.
/// </summary>
public class Marker : ISceneParent, INameable, IVisible, IUpdateable, IDrawable
{

    /// <summary>
    /// The parent scene, which is set by the scene
    /// when added via Scene.AddMarker().
    /// </summary>
    public Scene ParentScene { get; set; }

    private string? _name;
    /// <summary>
    /// The name of the marker, unique to a scene.
    /// Names are stored trimmed and in lowercase.
    /// </summary>
    public string? Name
    {
        get { return _name; }
        set
        {
            if (value == null)
                _name = value;
            else
                _name = value.Trim().ToLower();
        }
    }

    /// <summary>
    /// An optional entity to point to. If an entity is
    /// specified, the marker points to its TransformComponent
    /// center-top position (which the entity must have).
    /// </summary>
    public Entity? TargetEntity { get; set; }

    /// <summary>
    /// The (x, y) world position to point to. The position
    /// is either manually specified, or calculated if an
    /// entity is specified instead.
    /// </summary>
    public Vector2 TargetPosition { get; set; }

    /// <summary>
    /// The visibility of the marker.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Optional: The text to display with the marker.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// The color of the text to display with the marker.
    /// </summary>
    public Color TextColor { get; set; }

    /// <summary>
    /// The (x, y) size of the marker to display.
    /// </summary>
    public Vector2 Size { get; private set; }
    
    /// <summary>
    /// The marker texture to display.
    /// </summary>
    public Texture2D Texture { get; private set; }

    /// <summary>
    /// A list of camera names to exclude when drawing
    /// markers in cameras.
    /// </summary>
    public readonly List<string> CamerasToExclude;

    /// <summary>
    /// The minimum border between the marker / text
    /// and the edge of the camera. 
    /// </summary>
    public int CameraBorder { get; set; }

    /// <summary>
    /// The number of times the marker bounces per second.
    /// </summary>
    public float BounceFrequency { get; set; }

    /// <summary>
    /// The maximum displacement of the marker
    /// away from the target position, in pixels.
    /// </summary>
    public int BounceAmplitude { get; set; }

    public Marker(
        string? name = null,
        Entity? targetEntity = null,
        Vector2? targetPosition = null,
        bool visible = true,
        string? text = null,
        Color? textColor = null,
        Vector2? size = null,
        Texture2D? texture = null,
        List<string>? camerasToExclude = null,
        int cameraBorder = 20,
        float bounceFrequency = 1,
        int bounceAmplitude = 10
    )
    {

        Name = name;
        
        TargetEntity = targetEntity;
        TargetPosition = targetPosition ?? Vector2.Zero;
        if (TargetEntity != null && TargetEntity.HasComponent<TransformComponent>() == true)
        {
            TargetPosition = new Vector2(
                TargetEntity.GetComponent<TransformComponent>()!.Center,
                TargetEntity.GetComponent<TransformComponent>()!.Position.Y
            );
        }
        
        Visible = visible;
        Text = text;
        TextColor = textColor ?? Color.White;
        Texture = texture ?? EngineGlobals.game._engineResources.ImgPOIMarker;
        Size = size ?? new Vector2(Texture.Width, Texture.Height);
    
        CamerasToExclude = new List<string>();
        if (camerasToExclude != null)
        {
            foreach (string s in camerasToExclude)
            {
                CamerasToExclude!.Append(s.Trim().ToLower());
            }
        }

        CameraBorder = cameraBorder;
        BounceFrequency = bounceFrequency;
        BounceAmplitude = bounceAmplitude;

    }

    public void Update(GameTime gameTime)
    {
        if (TargetEntity != null && TargetEntity.HasComponent<TransformComponent>() == true)
        {
            TargetPosition = new Vector2(
                TargetEntity.GetComponent<TransformComponent>()!.Center,
                TargetEntity.GetComponent<TransformComponent>()!.Position.Y
            );
        }
    }

    public void Draw()
    {
        
    }

}