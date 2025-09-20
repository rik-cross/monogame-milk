using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameECS;

public class PhysicsSystem : System
{
    public override void Init()
    {
        AddRequiredComponentType<TransformComponent>();
    }
    public override void Update(GameTime gameTime, Scene scene)
    {
        //Console.WriteLine("dd");
    }
    public override void UpdateEntity(GameTime gameTime, Scene scene, Entity entity)
    {
        //Console.WriteLine("dd++");
    }
    public override void Draw(Scene scene)
    {
        //Console.WriteLine("dd");
    }
    public override void DrawEntity(Scene scene, Entity entity)
    {
        TransformComponent transformComponent = entity.GetComponent<TransformComponent>();

        

        /*spriteBatch.DrawRectangle(
            new Rectangle(
                (int)transformComponent.X,
                (int)transformComponent.Y,
                (int)transformComponent.Width,
                (int)transformComponent.Height
            ),
            Color.White,
            1.0f
        );*/

        /*spriteBatch.Draw(
            spriteComponent.GetSpriteForState(entity.State).GetCurrentTexture(),
            new Rectangle(
                // offset * scale
                (int)transformComponent.X - (int)(spriteComponent.GetSpriteForState(entity.State).offset.X * spriteComponent.GetSpriteForState(entity.State).scale.X),
                (int)transformComponent.Y - (int)(spriteComponent.GetSpriteForState(entity.State).offset.Y * spriteComponent.GetSpriteForState(entity.State).scale.Y),
                (int)(w * spriteComponent.GetSpriteForState(entity.State).scale.X),
                (int)(h * spriteComponent.GetSpriteForState(entity.State).scale.Y)
            ),
            Color.White
        );*/

    }
}