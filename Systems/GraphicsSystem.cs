using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameECS;

public class GraphicsSystem : System
{
    public override void Init()
    {
        AddRequiredComponentType<TransformComponent>();
        AddRequiredComponentType<SpriteComponent>();
    }

    public override void UpdateEntity(GameTime gameTime, Scene scene, Entity entity)
    {

        SpriteComponent spriteComponent = entity.GetComponent<SpriteComponent>();

        // Update if animated
        
        if (spriteComponent.HasSpriteForState(entity.State))
        {

            Sprite current = spriteComponent.GetSpriteForState(entity.State);

            // reset if new state
            if (entity.State != entity._previousState)
            {
                current.currentFrame = 0;
            }

            // if not on last frame
            // or
            // on last frame and loop
            // ...

            //if (
            //    current.currentFrame < current.textureList.Count - 1
            //    ||
            //    (current.currentFrame >= current.textureList.Count - 1 && current.loop == true)
            //)
            //{

                current.timeOnCurrentFrame += gameTime.ElapsedGameTime.TotalSeconds;
                //Console.WriteLine(current.timeOnCurrentFrame);
                while (current.timeOnCurrentFrame >= current.timePerFrame)
                {
                    current.timeOnCurrentFrame -= current.timePerFrame;
                // advance
                    if (
                        current.currentFrame < current.textureList.Count - 1 || 
                        current.currentFrame >= current.textureList.Count - 1 && current.loop == true
                    ) {
                        current.currentFrame += 1;
                        if (current.currentFrame > current.numberOfFrames - 1)
                        {
                            current.currentFrame = 0;
                        }
                    }
                }
            //}
        }
    }

    public override void DrawEntity(Scene scene, Entity entity)
    {

        TransformComponent transformComponent = entity.GetComponent<TransformComponent>();
        SpriteComponent spriteComponent = entity.GetComponent<SpriteComponent>();

        if (spriteComponent.HasSpriteForState(entity.State) == false)
            return;

        Sprite currentSprite = spriteComponent.GetSpriteForState(entity.State);
        Texture2D currentTexture = currentSprite.GetCurrentTexture();

        int x, y, w, h;

        // resize == true
        // means ignore offset and scale
        if (currentSprite.resizeToEntity == true)
        {
            x = (int)transformComponent.X;
            y = (int)transformComponent.Y;
            w = (int)transformComponent.Width;
            h = (int)transformComponent.Height;
        }

        // resize == false
        // means use offset and scale
        else
        {
            x = (int)(transformComponent.X - currentSprite.offset.X);
            y = (int)(transformComponent.Y - currentSprite.offset.Y);
            w = (int)(currentTexture.Width * currentSprite.scale.X);
            h = (int)(currentTexture.Height * currentSprite.scale.Y);
        }

        //Console.WriteLine(w);

        spriteBatch.Draw(
            spriteComponent.GetSpriteForState(entity.State).GetCurrentTexture(),
            new Rectangle(
                // offset * scale
                (int)x,
                (int)y,
                (int)w,
                (int)h
            ),
            Color.White
        );

        /*spriteBatch.DrawRectangle(
            new Rectangle(
                (int)x,
                (int)y,
                (int)w,
                (int)h
            ),
            Color.White,
            1.0f
        );*/

        /*spriteBatch.Draw(
            Scene.whiteRectangle,
            new Rectangle(
                (int)transformComponent.X,
                (int)transformComponent.Y,
                (int)transformComponent.Width,
                (int)transformComponent.Height
            ),
            Color.White
        );*/

    }

}