using milk.Components;
namespace milk.Core;

public static class EntitySortMethods
{

    public static int CompareBottom(Entity x, Entity y)
    {

        // Deal with null components
        if (x.HasComponent<TransformComponent>() == false &&
            y.HasComponent<TransformComponent>() == false)
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