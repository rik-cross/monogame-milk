//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

namespace milk.Core;

public class CraftingRecipe
{
    
    public string EntityTypeCreated;
    public Dictionary<string, int> Ingredients;
    public int Duration;

    public CraftingRecipe(
        string entityTypeCreated,
        Dictionary<string, int> ingredients,
        int duration = 0
    )
    {
        EntityTypeCreated = entityTypeCreated;
        Ingredients = ingredients;
        Duration = duration;
    }

}