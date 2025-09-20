/*
   [x] Finished
   
   Monogame ECS Engine
   By Rik Cross
   -- github.com/rik-cross/monogame-ecs
   Shared under the MIT licence

   ------------------------------------

   MonogameECS.Component
   =====================
  
   A component is a collection of data, with some additional useful callbacks.
*/

namespace MonoGameECS;

public abstract class Component
{

    // Optional callback methods
    public virtual void OnAddedToEntity(Entity entity) { }
    public virtual void OnRemovedFromEntity(Entity entity) { }

    public override string ToString()
    {
        string output = "";
        output += Theme.CreateConsoleTitle("Component");
        output += Theme.PrintConsoleVar("ID", EngineGlobals.game.componentManager.GetComponentTypeID(this.GetType()).ToString());
        output = output.Remove(output.Length - 1);
        return output;
    }

}