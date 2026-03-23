//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

namespace milk.Core;


public class CraftingSlot
{

    /// <summary>
    /// A crafting slot holds a recipe.
    /// </summary>
    public CraftingRecipe? Recipe;

    /// <summary>
    /// The game time at which the last slot update was made.
    /// </summary>
    public double LastUsed { get; internal set; }

    // Creates an empty slot
    internal CraftingSlot()
    {
        Clear();
    }

    internal void Clear()
    {
        Recipe = null;
        LastUsed = -1;
    }

}