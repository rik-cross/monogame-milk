using Microsoft.Xna.Framework;
using milk.Core;
using milk.Components;
using MonoGame.Extended;

namespace milk.Systems;

/// <summary>
/// Diaplays an (animated) emote above an entity.
/// </summary>
public class EmoteSystem : milk.Core.System
{

    /// <summary>
    /// Emotes require a transform and emote component.
    /// </summary>
    public override void Init()
    {
        AddRequiredComponentType<TransformComponent>();
        AddRequiredComponentType<EmoteComponent>();
        DrawAboveMap = true;
    }

    /// <summary>
    /// Updates the (possibly animated) emote component.
    /// </summary>
    /// <param name="gameTime">Elapsed game time.</param>
    /// <param name="scene">The scene containing the entity.</param>
    /// <param name="entity">The entity containing the emote.</param>
    public override void UpdateEntity(GameTime gameTime, Scene scene, Entity entity)
    {

        EmoteComponent emoteComponent = entity.GetComponent<EmoteComponent>()!;
        
        //
        // Update animation
        //

        if (emoteComponent.TextureList.Count > 1 && emoteComponent.Duration > 0)
        {
            emoteComponent.CurrentTime += gameTime.ElapsedGameTime.TotalSeconds;

            while (emoteComponent.CurrentTime >= emoteComponent.TimePerFrame)
            {
                emoteComponent.CurrentTime -= emoteComponent.TimePerFrame;
                
                if (
                    emoteComponent.Index < emoteComponent.TextureList.Count - 1 || 
                    emoteComponent.Index >= emoteComponent.TextureList.Count - 1 && emoteComponent.Loop == true
                ) {
                    emoteComponent.Index += 1;
                    if (emoteComponent.Index > emoteComponent.TextureList.Count - 1)
                        emoteComponent.Index = 0;
                }     
            }

        }

        //
        // Update alpha
        //

        if (emoteComponent.CurrrentAlpha < emoteComponent.TargetAlpha)
        {
            if (emoteComponent.FadeDuration == 0)
                emoteComponent.CurrrentAlpha = emoteComponent.TargetAlpha;
            else
                emoteComponent.CurrrentAlpha = (float)Math.Min(
                    emoteComponent.CurrrentAlpha + gameTime.ElapsedGameTime.TotalSeconds / emoteComponent.FadeDuration,
                    emoteComponent.TargetAlpha
                );
        }
        
        else if (emoteComponent.CurrrentAlpha > emoteComponent.TargetAlpha)
        {
            if (emoteComponent.FadeDuration == 0)
                emoteComponent.CurrrentAlpha = emoteComponent.TargetAlpha;
            else
                emoteComponent.CurrrentAlpha = (float)Math.Max(
                    emoteComponent.CurrrentAlpha - (float)gameTime.ElapsedGameTime.TotalSeconds / emoteComponent.FadeDuration,
                    0
                );
        }

    }

    /// <summary>
    /// Draws the emote above the entity.
    /// </summary>
    /// <param name="scene">The scene containing the entity.</param>
    /// <param name="entity">The entity containing the emote.</param>
    public override void DrawEntity(Scene scene, Entity entity)
    {

        TransformComponent transform = entity.GetComponent<TransformComponent>()!;
        EmoteComponent emote = entity.GetComponent<EmoteComponent>()!;

        // Defer to the custom draw method if one exists
        if (emote.CustomDrawMethod != null)
        {
            emote.CustomDrawMethod.Invoke(entity);
            return;
        }

        if (emote.Visible == false && emote.CurrrentAlpha == 0)
            return;

        Rectangle dest2Rect = new Rectangle(
            (int)(transform.Center - (emote.Size.X / 2) - emote.Margin),
            (int)(transform.Y - emote.Size.Y - (emote.Margin * 4) - emote.Margin),
            (int)emote.Size.X + emote.Margin * 2,
            (int)emote.Size.Y + emote.Margin * 2
        );

        Milk.Graphics.FillRectangle(
            dest2Rect,
            emote.BackgroundColor
        );

        Rectangle destRect = new Rectangle(
            (int)(transform.Center - (emote.Size.X / 2)),
            (int)(transform.Y - emote.Size.Y - (emote.Margin * 4)),
            (int)emote.Size.X,
            (int)emote.Size.Y
        );

        Milk.Graphics.Draw(
            emote.TextureList[emote.Index],
            destRect,
            Color.White * emote.CurrrentAlpha
        );

        dest2Rect.Inflate(emote.BorderWidth, emote.BorderWidth);
        Milk.Graphics.DrawRectangle(dest2Rect, emote.BorderColor, emote.BorderWidth);

    }

}