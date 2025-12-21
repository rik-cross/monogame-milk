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
/// A camera can be added to a Scene via scene.AddCamera(),
/// and draws a scene's world to the screen. 
/// </summary>
public class Camera
{

    /// <summary>
    /// The parent scene of the camera.
    /// </summary>
    public Scene? Scene;

    private string? _name;
    /// <summary>
    /// The name of the camera.
    /// Useful for getting / removing a specific camera.
    /// </summary>
    public string? Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }

    /// <summary>
    /// The (x, y) top-left scene position of the camera.
    /// </summary>
    public Vector2 ScreenPosition;

    /// <summary>
    /// The (x, y) size of the camera.
    /// </summary>
    public Vector2 ScreenSize;

    private bool firstUpdate = true;

    /// <summary>
    /// The (x, y) target world position that the camera is focused on.
    /// </summary>
    public Vector2 WorldPosition;
    private Vector2 _currentWorldPosition;

    /// <summary>
    /// If set to true, the camera is locked to the map dimensions,
    /// and won't show any of the scene outside of the map.
    /// </summary>
    public bool ClampToMap;

    /// <summary>
    /// The amount that the camera moves to its target position
    /// (0 = no movement, 1 = instant snap to new position).
    /// Values between 0 and 1 offer a degree of 'lazy follow'.
    /// </summary>
    public float FollowPercentage;

    /// <summary>
    /// The background color of the camera.
    /// </summary>
    public Color BackgroundColor;

    /// <summary>
    /// The width of the border.
    /// </summary>
    public int BorderWidth;

    /// <summary>
    /// The color of the border.
    /// </summary>
    public Color BorderColor;


    //
    // Zoom
    //

    public float _currentZoom;
    public float _startZoom;
    private float _targetZoom;
    private float _startZoomLog;
    private float _targetZoomLog;
    private float _lerpProgress = 1.0f;
    private float _zoomDuration = 0;

    /// <summary>
    /// The optional entity to track.
    /// </summary>
    public Entity? TrackedEntity;

    /// <summary>
    /// Creates a new camera object.
    /// </summary>
    /// <param name="screenSize">The (x, y) screen size of the camera.</param>
    /// <param name="screenPosition">The (x, y) screen position (default = (0, 0)).</param>
    /// <param name="worldPosition">The (x, y) world position (default = (0, 0)).</param>
    /// <param name="zoom">The initial zoom level (default = 1).</param>
    /// <param name="clampToMap">Clamp the camera to the map dimensions (default = false).</param>
    /// <param name="backgroundColor">The background color (default = transparent).</param>
    /// <param name="borderWidth">The width of the border (default = 0 - no border).</param>
    /// <param name="borderColor">The color of the border (defaul = black).</param>
    /// <param name="name">The name of the camera (default = null).</param>
    /// <param name="trackedEntity">An optional entity to track (default = null).</param>
    /// <param name="followPercentage">The amount to lazy follow (default = 1 - instant).</param>
    public Camera(
        Vector2 screenSize,
        Vector2? screenPosition = null,
        Vector2? worldPosition = null,
        float zoom = 1,
        bool clampToMap = false,
        Color? backgroundColor = null,
        int borderWidth = 0,
        Color? borderColor = null,
        string? name = null,
        Entity? trackedEntity = null,
        float followPercentage = 1.0f
    )
    {
        ScreenSize = screenSize;
        ScreenPosition = screenPosition ?? Vector2.Zero;
        WorldPosition = worldPosition ?? Vector2.Zero;
        _targetZoom = zoom;
        ClampToMap = clampToMap;
        BackgroundColor = backgroundColor ?? Color.Transparent;
        BorderWidth = borderWidth;
        BorderColor = borderColor ?? Color.Black;
        Name = name;
        TrackedEntity = trackedEntity;
        FollowPercentage = followPercentage;

    }

    internal void Update(GameTime gameTime)
    {

        if (TrackedEntity != null && Scene != null &&
            Scene.entities.Contains(TrackedEntity) &&
            TrackedEntity.HasComponent<TransformComponent>())
        {
            TransformComponent transformComponent = TrackedEntity.GetComponent<TransformComponent>();
            WorldPosition = new Vector2(
                transformComponent.Center * -1,
                transformComponent.Middle * -1
            );
        }

        UpdateZoom(gameTime);
        UpdatePosition();
        UpdateClamp();
        
        if (firstUpdate == true)
        {

            firstUpdate = false;

            _currentWorldPosition = WorldPosition;

            _currentZoom = _targetZoom;
            _startZoom = _targetZoom;
            _zoomDuration = 0;
            _lerpProgress = 1.0f;        

        }
      
    }

    private void UpdateClamp()
    {
        if (ClampToMap == true && Scene != null && Scene.mapSize.HasValue)
        {

            // Set the camera world center to the map center
            // if the map*zoom is smaller than the camera screen size

            // X
            if (ScreenSize.X > Scene.mapSize.Value.X * _currentZoom)
                _currentWorldPosition.X = Scene.mapSize.Value.X / 2 * -1;
            // else ensure no off-world is seen
            else
            {
                // left
                if (_currentWorldPosition.X * -1 < ScreenSize.X / _currentZoom / 2)
                    _currentWorldPosition.X = ScreenSize.X / _currentZoom / 2 * -1;
                // right
                if (_currentWorldPosition.X * -1 > (Scene.mapSize.Value.X - (ScreenSize.X / _currentZoom / 2)))
                    _currentWorldPosition.X = (Scene.mapSize.Value.X - (ScreenSize.X / _currentZoom / 2)) * -1;
            }

            // Y
            if (ScreenSize.Y > Scene.mapSize.Value.Y * _currentZoom)
                _currentWorldPosition.Y = Scene.mapSize.Value.Y / 2 * -1;
            // else ensure no off-world is seen
            else
            {
                // left
                if (_currentWorldPosition.Y * -1 < ScreenSize.Y / _currentZoom / 2)
                    _currentWorldPosition.Y = ScreenSize.Y / _currentZoom / 2 * -1;
                // right
                if (_currentWorldPosition.Y * -1 > (Scene.mapSize.Value.Y - (ScreenSize.Y / _currentZoom / 2)))
                    _currentWorldPosition.Y = (Scene.mapSize.Value.Y - (ScreenSize.Y / _currentZoom / 2)) * -1;
            }

        }

    }

    private void UpdatePosition()
    {
        _currentWorldPosition.X = (_currentWorldPosition.X * (1 - FollowPercentage)) + (WorldPosition.X * FollowPercentage);
        _currentWorldPosition.Y = (_currentWorldPosition.Y * (1 - FollowPercentage)) + (WorldPosition.Y * FollowPercentage);
    }

    private void UpdateZoom(GameTime gameTime)
    {
        if (_lerpProgress < 1.0f)
        {
            // Increase the progress based on the elapsed time
            _lerpProgress += (float)gameTime.ElapsedGameTime.TotalSeconds / _zoomDuration;

            // Clamp the progress to ensure it doesn't go over 1.0
            _lerpProgress = MathHelper.Clamp(_lerpProgress, 0.0f, 1.0f);

            // Perform the linear interpolation on the logarithmic values
            float interpolatedLogZoom = MathHelper.Lerp(_startZoomLog, _targetZoomLog, _lerpProgress);

            // Convert back to a linear zoom value
            _currentZoom = (float)Math.Exp(interpolatedLogZoom);
        } 
    }

    /// <summary>
    /// Sets the zoom level of the camera.
    /// </summary>
    /// <param name="zoom">The new target zoom level (default = 1).</param>
    /// <param name="duration">The zoom duration (defaul = 0 - instant).</param>
    public void SetZoom(float zoom = 1, float duration = 0)
    {

        // Do nothing if the camera already has the required zoom level as the target
        if (zoom == _targetZoom)
            return;

        // Instantly set the new zoom level if no duration is specified
        if (duration == 0)
        {
            _currentZoom = zoom;
            _startZoom = zoom;
            _targetZoom = zoom;
            _lerpProgress = 1.0f;
            _zoomDuration = 0;
        }

        // Set up a 'lerp' to the new required zoom level
        else
        {
            // Reset the lerp progress and store the new values
            _lerpProgress = 0.0f;
            _startZoom = _currentZoom;
            _targetZoom = zoom;
            _zoomDuration = duration;

            // Calculate the logarithmic values to use for the lerp
            _startZoomLog = (float)Math.Log(_startZoom);
            _targetZoomLog = (float)Math.Log(_targetZoom);
        }

    }

    internal Viewport getViewport()
    {
        return new Viewport(
            (int)ScreenPosition.X, (int)ScreenPosition.Y,
            (int)ScreenSize.X, (int)ScreenSize.Y
        );
    }

    internal Matrix getTransformMatrix()
    {
        Vector2 centrePosition = _currentWorldPosition;
        centrePosition.X = _currentWorldPosition.X + ScreenSize.X / 2 / _currentZoom;
        centrePosition.Y = _currentWorldPosition.Y + ScreenSize.Y / 2 / _currentZoom;

        return Matrix.CreateTranslation(
                new Vector3(centrePosition.X, centrePosition.Y, 0.0f)) *
                Matrix.CreateRotationZ(0.0f) *
                Matrix.CreateScale(_currentZoom, _currentZoom, 1.0f) *
                Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, 0.0f)
        );
    }

    /// <summary>
    /// Sets a new target world position.
    /// </summary>
    /// <param name="newPosition">The new (x,y) world position.</param>
    /// <param name="instant">Sets the new position instantly, ignoring any lazy follow, etc. (default = false).</param>
    public void SetWorldPosition(Vector2 newPosition, bool instant = false)
    {

        TrackedEntity = null;

        // Set the target position
        WorldPosition = newPosition * -1;

        // If no duration, then immediately set the new position
        if (instant == true)
        {
            _currentWorldPosition = WorldPosition;
            UpdateClamp();
        }

    }

    /// <summary>
    /// Gets the target world position.
    /// </summary>
    /// <returns>The (x, y) camera world target.</returns>
    public Vector2 GetWorldPosition()
    {
        return WorldPosition * -1;
    }

}
