using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace milk;

public class InputSystem : System
{
    public override void Init()
    {
        AddRequiredComponentType<InputComponent>();
    }
    public override void Update(GameTime gameTime, Scene scene)
    {

    }
    public override void UpdateEntity(GameTime gameTime, Scene scene, Entity entity)
    {
        InputComponent inputComponent = entity.GetComponent<InputComponent>();
        if (inputComponent.inputController != null)
        {
            inputComponent.inputController(scene, entity);
        }
    }
    public override void Draw(Scene scene)
    {
    }
    public override void DrawEntity(Scene scene, Entity entity)
    {
    }
}