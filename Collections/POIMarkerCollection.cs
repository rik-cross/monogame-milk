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
/// A collection of scene markers.
/// </summary>
public class POIMarkerCollection : Collection<POIMarker>
{

    // If a POIMarker in the collection specifies a TargetEntity,
    // this method updates the TargetPosition each frame to the 
    // center-top position of the TargetEntity. Note that this only
    // happens if the TargetEntity has a TransformComponent.
    internal override void Update(GameTime gameTime, Scene scene)
    {
        
        foreach (POIMarker marker in _items)
        {
            if (marker.TargetEntity != null && marker.TargetEntity.HasComponent<TransformComponent>() == true)
            {
                marker.TargetPosition = new Vector2(
                    marker.TargetEntity.GetComponent<TransformComponent>()!.Center,
                    marker.TargetEntity.GetComponent<TransformComponent>()!.Position.Y
                );
            }
        }

    }

    // Draws each POIMarker
    internal override void Draw(Scene scene)
    {

        foreach (POIMarker marker in _items)
        {

            if (marker.Visible == false)
                continue;

            foreach (Camera camera in scene.cameras)
            {

                bool isExcluded = false;
                if (camera.Name != null)
                {
                    foreach (string excludedName in marker.CamerasToExclude)
                    {
                        if (excludedName.ToLower() == camera.Name.ToLower())
                        {
                            isExcluded = true;
                            break;
                        }
                    }
                }

                if (isExcluded)
                    continue;

                Vector2 cameraWorldCenter = camera._currentWorldPosition * -1;
                Vector2 cameraScreenCenter = camera.ScreenPosition + (camera.ScreenSize / 2);
                Vector2 cameraSize = camera.ScreenSize;

                double x = 
                    (marker.TargetPosition.X * camera._currentZoom) - 
                    (cameraWorldCenter.X * camera._currentZoom) +
                    cameraScreenCenter.X;

                double y =
                    (marker.TargetPosition.Y * camera._currentZoom) -
                    (cameraWorldCenter.Y * camera._currentZoom) +
                    cameraScreenCenter.Y;

                // Clamp values to screen
                double nx = Math.Clamp(x, camera.ScreenPosition.X + marker.CameraBorder, camera.ScreenPosition.X + camera.ScreenSize.X - marker.CameraBorder);
                double ny = Math.Clamp(y, camera.ScreenPosition.Y + marker.CameraBorder, camera.ScreenPosition.Y + camera.ScreenSize.Y - marker.CameraBorder);

                // If the value hasn't been clamped, then
                // the marker position is within the camera viewport
                if ( (int)nx == (int)x && (int)ny == (int)y )
                {

                    // Point downwards

                    Milk.Graphics.Draw(
                        marker.Texture,
                        new Rectangle(
                            (int)(nx),
                            (int)(ny + Math.Sin(scene.elapsedTime * marker.BounceFrequency * MathHelper.TwoPi) * marker.BounceAmplitude),
                            (int)(marker.Size.X),
                            (int)(marker.Size.Y)
                        ),
                        null,
                        Color.White,
                        (float)(2 * 3.14 / 4),
                        new Vector2(marker.Texture.Width, marker.Texture.Height / 2),
                        SpriteEffects.None,
                        0
                    );

                }
                else
                {
                    
                    Vector2 direction = 
                        new Vector2((float)x, (float)y) - 
                        new Vector2((float)cameraScreenCenter.X, (float)cameraScreenCenter.Y);

                    Milk.Graphics.Draw(
                        marker.Texture,
                        new Rectangle(
                            (int)(nx),
                            (int)(ny),
                            (int)(marker.Size.X),
                            (int)(marker.Size.Y)
                        ),
                        null,
                        Color.White,
                        (float)(Math.Atan2(direction.Y, direction.X)),
                        new Vector2(marker.Texture.Width, marker.Texture.Height / 2),
                        SpriteEffects.None,
                        0
                    );

                    if (marker.Text != null)
                    {
                        // 1. Get the direction from the screen center to the marker
                        Vector2 screenCenter = camera.ScreenPosition + (camera.ScreenSize / 2);
                        Vector2 toMarker = new Vector2((float)nx, (float)ny) - screenCenter;

                        // 2. Normalize to get a consistent push distance
                        if (toMarker != Vector2.Zero) toMarker.Normalize();

                        // 3. Initial text position (pushing back towards center by 45 pixels)
                        Vector2 textPos = new Vector2((float)nx, (float)ny) - (toMarker * 45f);

                        // 4. Setup Font and Measurements
                        string label = marker.Text;
                        SpriteFont font = EngineGlobals.game._engineResources.FontSmall;
                        Vector2 textSize = font.MeasureString(label);
                        Vector2 textOrigin = textSize / 2;

                        // 5. Clamp the text position so the EDGES of the text stay 20px from screen boundaries
                        // We calculate the min/max allowed X based on half the text width
                        float minX = camera.ScreenPosition.X + (textSize.X / 2) + marker.CameraBorder;
                        float maxX = camera.ScreenPosition.X + camera.ScreenSize.X - (textSize.X / 2) - marker.CameraBorder;

                        // We calculate the min/max allowed Y based on half the text height
                        float minY = camera.ScreenPosition.Y + (textSize.Y / 2) + marker.CameraBorder;
                        float maxY = camera.ScreenPosition.Y + camera.ScreenSize.Y - (textSize.Y / 2) - marker.CameraBorder;

                        textPos.X = Math.Clamp(textPos.X, minX, maxX);
                        textPos.Y = Math.Clamp(textPos.Y, minY, maxY);

                        // 6. Draw the string
                        Milk.Graphics.DrawString(
                            font,
                            label,
                            textPos,
                            Color.White,
                            0f,
                            textOrigin,
                            1.0f,
                            SpriteEffects.None,
                            0
                        );
                    }

                }

            }

        }

    }

}