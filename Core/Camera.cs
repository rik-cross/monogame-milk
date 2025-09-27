using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk;

public class Camera
{
    public string Name;
    public Vector2 ScreenPosition;
    public Vector2 ScreenSize;

    //
    // Position
    //



    // should have a getter and setter to swap to negative * -1!!!
    public Vector2 WorldPosition; // target
    public Vector2 _currentWorldPosition; // current

    public bool ClampToMap;

    // Set between 0 and 1 only
    // private...
    public float FollowPercentage = 0.1f;


    public Color BackgroundColor;
    public int BorderWidth;
    public Color BorderColor;


    //
    // Zoom
    //

    private const float _defaultZoomIncrement = 0.05f;

    public float _currentZoom;
    public float _startZoom;
    private float _targetZoom;
    private float _zoomIncrement = _defaultZoomIncrement;
    private float _startZoomLog;
    private float _targetZoomLog;
    private float _lerpProgress = 1.0f;
    private float _zoomDuration = 0;
    public Entity TrackedEntity;

    public Camera(Vector2 size, Vector2 screenPosition = default, Vector2 worldPosition = default, float zoom = 1, bool clampToMap = false, Color backgroundColor = default, int borderWidth = 0, Color borderColor = default, string name = null, Entity trackedEntity = null)
    {
        ScreenSize = size;

        if (screenPosition == default)
            ScreenPosition = new Vector2(0, 0);
        else
            ScreenPosition = screenPosition;

        if (worldPosition == default)
            SetWorldPosition(new Vector2(0, 0), instant: true);
        else
            SetWorldPosition(worldPosition, instant: true);

        _currentZoom = zoom;
        _targetZoom = zoom;
        _startZoom = zoom;

        //_zoomIncrement = _defaultZoomIncrement;

        ClampToMap = clampToMap;

        if (backgroundColor == default)
            BackgroundColor = Color.Transparent;
        else
            BackgroundColor = backgroundColor;

        BorderWidth = borderWidth;

        if (borderColor == default)
            BorderColor = Color.Black;
        else
            BorderColor = borderColor;

        // unique name
        // need to pass scene in order to check??
        if (name == null)
            Name = "";
        else
        {
            Name = name;
        }

        this.TrackedEntity = trackedEntity;

    }

    public void Update(GameTime gameTime, Scene scene)
    {

        if (TrackedEntity != null)
        {
            TransformComponent transformComponent = TrackedEntity.GetComponent<TransformComponent>();
            WorldPosition = new Vector2(
                transformComponent.Center * -1,
                transformComponent.Middle * -1
            );
        }

        UpdateClamp(scene);
        UpdatePosition();
        UpdateZoom(gameTime);
 
    }

    private void UpdateClamp(Scene scene)
    {
        if (ClampToMap == true && scene.mapSize.HasValue)
        {

            // Set the camera world center to the map center
            // if the map*zoom is smaller than the camera screen size

            // X
            if (ScreenSize.X > scene.mapSize.Value.X * _currentZoom)
            {
                //Console.WriteLine("yep");
                WorldPosition.X = scene.mapSize.Value.X / 2 * -1;
            }
            // else ensure no off-world is seen
            else
            {
                // left
                if (WorldPosition.X * -1 < ScreenSize.X / _currentZoom / 2)
                {
                    WorldPosition.X = ScreenSize.X / _currentZoom / 2 * -1;
                    //Console.WriteLine("left");
                }
                // right
                if (WorldPosition.X * -1 > (scene.mapSize.Value.X - (ScreenSize.X / _currentZoom / 2)))
                {
                    //Console.WriteLine("right");
                    WorldPosition.X = (scene.mapSize.Value.X - (ScreenSize.X / _currentZoom / 2)) * -1;
                }
            }

            // Y
            if (ScreenSize.Y > scene.mapSize.Value.Y * _currentZoom)
            {
                //Console.WriteLine("yep");
                WorldPosition.Y = scene.mapSize.Value.Y / 2 * -1;
            }
            // else ensure no off-world is seen
            else
            {
                // left
                if (WorldPosition.Y * -1 < ScreenSize.Y / _currentZoom / 2)
                {
                    WorldPosition.Y = ScreenSize.Y / _currentZoom / 2 * -1;
                    //Console.WriteLine("left");
                }
                // right
                if (WorldPosition.Y * -1 > (scene.mapSize.Value.Y - (ScreenSize.Y / _currentZoom / 2)))
                {
                    //Console.WriteLine("right");
                    WorldPosition.Y = (scene.mapSize.Value.Y - (ScreenSize.Y / _currentZoom / 2)) * -1;
                }
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

    public Viewport getViewport()
    {
        return new Viewport(
            (int)ScreenPosition.X, (int)ScreenPosition.Y,
            (int)ScreenSize.X, (int)ScreenSize.Y
        );
    }

    public Matrix getTransformMatrix()
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

    // use a getter and setter???
    public void SetWorldPosition(Vector2 newPosition, bool instant = false)
    {

        TrackedEntity = null;

        // Set the target position
        WorldPosition = newPosition * -1;

        // If no duration, then immediately set the new position
        if (instant == true)
        {
            _currentWorldPosition = WorldPosition;
        }

    }

    public Vector2 GetWorldPosition()
    {
        return WorldPosition * -1;
    }

}