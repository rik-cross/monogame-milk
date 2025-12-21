//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

using milk.Core;
using milk.Components;

namespace milk.Systems;

internal class SpriteSystem : milk.Core.System
{

    /// <summary>
    /// Initialises the sprite system, which only processes entities
    /// with a transform component and a sprite component.
    /// </summary>
    public override void Init()
    {
        AddRequiredComponentType<TransformComponent>();
        AddRequiredComponentType<SpriteComponent>();
    }

    /// <summary>
    /// Updates each of the entity sprites if animated
    /// (i.e. if >1 texture is specified for the current state).
    /// </summary>
    /// <param name="gameTime">The MonoGame gameTime object for measuring elapsed time.</param>
    /// <param name="scene">The scene containing the system.</param>
    /// <param name="entity">The entity to be processed.</param>
    public override void UpdateEntity(GameTime gameTime, Scene scene, Entity entity)
    {

        SpriteComponent spriteComponent = entity.GetComponent<SpriteComponent>();

        if (spriteComponent.Play == false)
            return;

        // Update if animated
        
        if (spriteComponent.HasSpriteForState(entity.State))
        {

            Sprite current = spriteComponent.GetSpriteForState(entity.State);

            // reset if new state
            if (entity.State != entity.PreviousState)
                current.currentFrame = 0;

            if (current.textureList.Count > 1) {
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
            }
        }
    }

    /// <summary>
    /// Draws each entity sprite for the correct state and index.
    /// </summary>
    /// <param name="scene">The scene containing the system.</param>
    /// <param name="entity">The entity to be processed.</param>
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
            x = (int)(transformComponent.X + currentSprite.offset.X);
            y = (int)(transformComponent.Y + currentSprite.offset.Y);
            w = (int)(currentTexture.Width * currentSprite.scale.X);
            h = (int)(currentTexture.Height * currentSprite.scale.Y);
        }

        SpriteEffects flipEffect = SpriteEffects.None;
        if (spriteComponent.GetSpriteForState(entity.State).flipH)
            flipEffect = flipEffect | SpriteEffects.FlipHorizontally;
        if (spriteComponent.GetSpriteForState(entity.State).flipV)
            flipEffect = flipEffect | SpriteEffects.FlipVertically;

        spriteBatch.Draw(
            texture: spriteComponent.GetSpriteForState(entity.State).GetCurrentTexture(),
            destinationRectangle: new Rectangle((int)x, (int)y, (int)w, (int)h),
            sourceRectangle: null,
            rotation: 0f,
            origin: Vector2.Zero,
            color: currentSprite.hue,
            effects: flipEffect,
            layerDepth: 0f
        );

        if (scene.game.Debug == true)
        {
            spriteBatch.DrawRectangle(
                new Rectangle(
                    (int)x,
                    (int)y,
                    (int)w,
                    (int)h
                ),
                Color.White,
                1.0f
            );
        }

    }

}