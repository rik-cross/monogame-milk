using Microsoft.Xna.Framework;

namespace milk;

public class InputComponent : Component
{

    public Action<Scene, Entity> inputController;

    public InputComponent(Action<Scene, Entity> inputController = null)
    {
        this.inputController = inputController;
    }

}