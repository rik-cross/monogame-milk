//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

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

        if (spriteComponent.GetSpriteForState(entity.State) == null)
            return;

        Sprite current = spriteComponent.GetSpriteForState(entity.State);

        // reset if new state
        if (entity.State != entity.PreviousState)
            current.currentFrame = 0;

        if (current.duration == 0)
            return;

        // TODO: store an 'animated' property that gets updated
        // Should be false if no layer has more than 1 texture
        if (current.numberOfFrames < 2)
            return;
        
        current.timeOnCurrentFrame += gameTime.ElapsedGameTime.TotalSeconds;

        while (current.timeOnCurrentFrame >= current.timePerFrame)
        {
            current.timeOnCurrentFrame -= current.timePerFrame;
            
            if (
                current.currentFrame < current.numberOfFrames - 1 || 
                current.currentFrame >= current.numberOfFrames - 1 && current.Loop == true
            ) {
                current.currentFrame += 1;
                if (current.currentFrame > current.numberOfFrames - 1)
                {
                    current.currentFrame = 0;
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

        // Get the required components
        TransformComponent transformComponent = entity.GetComponent<TransformComponent>();
        SpriteComponent spriteComponent = entity.GetComponent<SpriteComponent>();

        // Return if no sprite
        if (spriteComponent.GetSpriteForState(entity.State) == null)
            return;

        // Get the sprite
        Sprite currentSprite = spriteComponent.GetSpriteForState(entity.State);

        // Return if no texture in sprite
        if (currentSprite.spriteTextureList == null || currentSprite.spriteTextureList.Count == 0)
            return;

        // Iterate over each of the texture lists in the sprite
        foreach (SpriteTextureList textureList in currentSprite.spriteTextureList)
        {

            // Ignore if there are fewer textures in this list than in other texture lists
            if (currentSprite.currentFrame >= textureList.textureList.Count)
                continue;
            
            // Get the current texture
            Texture2D currentTexture = textureList.textureList[currentSprite.currentFrame];

            // Store the texture dimensions,
            // these will change depending on the parameters chosen
            int x, y, w, h;

            // If 'resizeToEntity' is true, then set the texture
            // size to the entity's TransformComponent dimensions
            if (currentSprite.resizeToEntity == true)
            {
                x = (int)transformComponent.X;
                y = (int)transformComponent.Y;
                w = (int)transformComponent.Width;
                h = (int)transformComponent.Height;
            }
            // If 'resizeToEntity' is false, then
            // use the offset and scale values instead
            else
            {
                x = (int)(transformComponent.X + textureList.offset.X * -1);
                y = (int)(transformComponent.Y + textureList.offset.Y * -1);
                w = (int)(currentTexture.Width * textureList.scale.X);
                h = (int)(currentTexture.Height * textureList.scale.Y);
            }

            // Set the texture horizontal and vertical flip
            SpriteEffects flipEffect = SpriteEffects.None;
            if (textureList.flipH)
                flipEffect = flipEffect | SpriteEffects.FlipHorizontally;
            if (textureList.flipV)
                flipEffect = flipEffect | SpriteEffects.FlipVertically;

            // Draw the texture
            spriteBatch.Draw(
                texture: currentTexture,
                destinationRectangle: new Rectangle((int)x, (int)y, (int)w, (int)h),
                sourceRectangle: null,
                rotation: 0f,
                origin: Vector2.Zero,
                color: textureList.hue * textureList.Alpha,
                effects: flipEffect,
                layerDepth: 0f
            );

            // Debug draw the texture outline
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

}