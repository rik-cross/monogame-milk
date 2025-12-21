//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using milk.Components;

namespace milk.Core;

/// <summary>
/// Methods to control the draw order of entities in a scene.
/// </summary>
public static class EntitySortMethods
{

    /// <summary>
    /// Sorts entities in a scene by their bottom position
    /// (i.e. y position + transformComponent.height).
    /// </summary>
    public static Comparison<Entity> SortBottom => CompareBottom;

    private static int CompareBottom(Entity x, Entity y)
    {

        // Deal with null components
        if (x.HasComponent<TransformComponent>() == false && y.HasComponent<TransformComponent>() == false)
            return 0;
        else if (x.HasComponent<TransformComponent>() == false) return -1;
        else if (y.HasComponent<TransformComponent>() == false) return 1;

        // Get transform components to compare
        TransformComponent tx = x.GetComponent<TransformComponent>();
        TransformComponent ty = y.GetComponent<TransformComponent>();

        // If they are the same then use the Left position as a 'tie-breaker'
        if (tx.Bottom == ty.Bottom)
        {
            if (tx.Left == ty.Left) return 0;
            else if (tx.Left > ty.Left) return 1;
            else if (tx.Left < ty.Left) return -1;
        }
        else if (tx.Bottom > ty.Bottom) return 1;
        else if (tx.Bottom < ty.Bottom) return -1;

        return 0;
        
    }

}