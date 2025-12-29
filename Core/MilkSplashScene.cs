using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using milk.Core;
using milk.Transitions;
using milk.UI;

namespace milk.Core;

internal class MilkSplashScene : Scene
{

    Image milkImage;
    Text infoLine1;

    public override void Init()
    {
        BackgroundColor = Color.Black;

        milkImage = new Image(
            texture: game._engineResources.ImgMilk,
            size: new Vector2(205 / 4, 265 / 4),
            position: Middle,
            anchor: Anchor.MiddleCenter,
            alpha: 0f
        );

        infoLine1 = new Text(
            caption: "Made with milk",
            anchor: Anchor.MiddleCenter,
            position: new Vector2(Middle.X, Middle.Y + 55),
            color: new Color(244, 249, 252),
            alpha: 0f
        );

        AddTimedAction(
            elapsedTime: 500,
            action: () =>
            {
                animator.AddTween(
                    new Tween(
                        action: (float t) => {
                            milkImage.Alpha = MathHelper.Lerp(0, 1, t);
                            infoLine1.Alpha = MathHelper.Lerp(0, 1, t);
                        },
                        duration: 0.5f
                    )
                );
            }
        );

        AddTimedAction(
            elapsedTime: 2000,
            action: () =>
            {
                animator.AddTween(
                    new Tween(
                        //element: infoLine1,
                        action: (float t) => {
                            //Console.WriteLine("Elapsed: " + t.ToString());
                            milkImage.Alpha = MathHelper.Lerp(1, 0, t);
                            infoLine1.Alpha = MathHelper.Lerp(1, 0, t);
                        },
                        duration: 0.5f
                    )
                );
            }
        );

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

    public override void Input(GameTime gameTime)
    {
        // Press [Esc] to quit
        if (game.inputManager.IsKeyPressed(Keys.Escape))
            game.Quit();
    }

    public override void Draw()
    {
        milkImage.Draw();
        infoLine1.Draw();
    }

}