//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using milk.Core;
using milk.Components;

namespace milk.Systems;

internal class InputSystem : milk.Core.System
{

    /// <summary>
    /// Initialises the input system, which only processes entities
    /// with an input component.
    /// </summary>
    public override void Init()
    {
        AddRequiredComponentType<InputComponent>();
    }

    /// <summary>
    /// Processes the input of each entity with an input component.
    /// </summary>
    /// <param name="gameTime">The MonoGame gameTime object for measuring elapsed time.</param>
    /// <param name="scene">The scene containing the system.</param>
    /// <param name="entity">The entity to be processed.</param>
    public override void InputEntity(GameTime gameTime, Scene scene, Entity entity)
    {
        InputComponent inputComponent = entity.GetComponent<InputComponent>();
        
        // Don't process input if there's an active transition
        bool isSuspended = Milk.Scenes.currentTransition != null;
        
        inputComponent.inputController(scene, entity, isSuspended);

    }

}
