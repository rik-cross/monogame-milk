//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using milk.Core;
using milk.Transitions;
using milk.UI;

namespace milk.Core;

internal class SplashScene : Scene
{

    Image imgMilk;
    Text txtMilk;

    public override void Init()
    {

        BackgroundColor = Color.Black;

        // milk logo
        imgMilk = new Image(
            texture: game._engineResources.ImgMilk,
            size: new Vector2(205 / 4, 265 / 4),
            position: Middle,
            anchor: Anchor.MiddleCenter,
            alpha: 0f
        );

        // 'Made with milk' text
        txtMilk = new Text(
            caption: "Made with milk",
            anchor: Anchor.MiddleCenter,
            position: new Vector2(Middle.X, Middle.Y + 55),
            color: new Color(244, 249, 252),
            alpha: 0f
        );

        // Fade in the image and text after 0.5 seconds,
        // by setting their alpha values to 1
        AddTimedAction(
            elapsedTime: 500,
            action: () =>
            {
                animator.AddTween(
                    new Tween(
                        action: (float t) => {
                            imgMilk.Alpha = MathHelper.Lerp(0, 1, t);
                            txtMilk.Alpha = MathHelper.Lerp(0, 1, t);
                        },
                        duration: 0.5f
                    )
                );
            }
        );

        // Fade out the image and text after 2 seconds,
        // by setting their alpha values to 0
        AddTimedAction(
            elapsedTime: 2000,
            action: () =>
            {
                animator.AddTween(
                    new Tween(
                        action: (float t) => {
                            imgMilk.Alpha = MathHelper.Lerp(1, 0, t);
                            txtMilk.Alpha = MathHelper.Lerp(1, 0, t);
                        },
                        duration: 0.5f
                    )
                );
            }
        );

        // Remove this splash scene after 2.5 seconds
        AddTimedAction(
            elapsedTime: 2500,
            action: () =>
            {
                EngineGlobals.game.RemoveScene(
                    transition: new TransitionFadeToBlack(duration: 1000)
                );
            }
        );

    }

    public override void Draw()
    {

        imgMilk.Draw();
        txtMilk.Draw();
    
    }

}