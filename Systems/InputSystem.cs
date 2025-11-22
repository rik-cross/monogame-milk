using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using milk.Core;
using milk.Components;
namespace milk.Systems;

public class InputSystem : milk.Core.System
{

    public override void Init()
    {
        AddRequiredComponentType<InputComponent>();
    }

    public override void Input(GameTime gameTime, Scene scene)
    {
    }

    public override void InputEntity(GameTime gameTime, Scene scene, Entity entity)
    {
        InputComponent inputComponent = entity.GetComponent<InputComponent>();
        if (inputComponent.inputController != null)
        {
            inputComponent.inputController(scene, entity);
        }
    }

}
