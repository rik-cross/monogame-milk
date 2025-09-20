using Microsoft.Xna.Framework;

namespace MonoGameECS;

public class LightingSystem : System
{
    public int maxLights = 10;
    //public Vector2[] lightPositions;
    public override void Update(GameTime gameTime, Scene scene)
    {
        //int numLights = 0;
        //lightPositions = new Vector2[10];
        //List<Light> lights = new List<Light>();
        //Vector2[] positions = new Vector2[10];

        //numLights++;
        //for (int i = 0; i < 10; i++)
        //{
            //lights.Add(new Light(new Vector2(i * 50, i * 50), 200));
            //positions[i] = new Vector2(i * 50, i * 50);
            //lightPositions[i] = new Vector2(i * 30, i * 30);
        //}

        //Scene.lightingShader.Parameters["PositionT"]?.SetValue(new Vector2(300, 300));
        //Scene.lightingShader.Parameters["LightPositions"]?.SetValue(positions);

        //Matrix projection = Matrix.CreateOrthographicOffCenter(
        //    0, EngineGlobals.game.graphicsDevice.Viewport.Width, EngineGlobals.game.graphicsDevice.Viewport.Height, 0, 0, 1);
        //Matrix view = Matrix.Identity; // Or a camera matrix
        //Matrix world = Matrix.Identity; // Or your object's world matrix

        //Matrix worldViewProjection = world * view * projection;
        //Scene.lightingShader.Parameters["WorldViewProjection"]?.SetValue(worldViewProjection);

    }
}