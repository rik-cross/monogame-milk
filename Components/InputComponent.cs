//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using milk.Core;

namespace milk.Components;

/// <summary>
/// Supports entity input through an input controller.
/// </summary>
public class InputComponent : Component
{

    /// <summary>
    /// A function that describes entity input behaviour.
    /// </summary>
    public Action<Scene, Entity>? inputController;

    /// <summary>
    /// Creates a new input component.
    /// </summary>
    /// <param name="inputController">The input function to be called.</param>
    public InputComponent(Action<Scene, Entity>? inputController = null)
    {
        this.inputController = inputController;
    }

}