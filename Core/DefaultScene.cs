using System;
using System.Data;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace milk;

public class DefaultScene : Scene
{

    private Image milkImage;
    private Text infoText1;
    private Text infoText2;

    public override void Init()
    {
        BackgroundColor = Color.CornflowerBlue;

        milkImage = new Image(
            texture: content.Load<Texture2D>("images/milk"),
            size: new Vector2(40, 60),
            position: new Vector2(20, 20),
            anchor: Anchor.TopLeft
        );

        infoText1 = new Text(
            caption: "milk v0.1",
            font: content.Load<SpriteFont>("Fonts/Medium"),
            position: new Vector2(75, 30),
            anchor: Anchor.TopLeft,
            color: Color.White,
            outlineWidth: 3
        );

        infoText2 = new Text(
            caption: "rik-cross.github.io/milk-docs",
            font: content.Load<SpriteFont>("Fonts/Medium"),
            position: new Vector2(75, 50),
            anchor: Anchor.TopLeft,
            color: Color.White,
            outlineWidth: 3
        );

    }

    public override void Input(GameTime gameTime)
    {
        // Press [Esc] to exit
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            game.Quit();
    }

    public override void Draw()
    {
        milkImage.Draw();
        infoText1.Draw();
        infoText2.Draw();
    }

}
