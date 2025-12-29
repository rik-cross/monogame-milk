//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using milk.Core;

namespace milk.UI;

/// <summary>
/// A menu holds UI elements.
/// </summary>
internal class UIMenu
{
    
    private Scene parentScene;
    private List<UIElement> menuItems;
    private UIElement? selectedElement;

    public List<Keys> Up;
    public List<Keys> Down;
    public List<Keys> Left;
    public List<Keys> Right;
    public List<Keys> Select;

    public bool Visible { get; set; }

    internal UIMenu(Scene parentScene)
    {
        this.parentScene = parentScene;
        menuItems = new List<UIElement>();
        selectedElement = null;
        Up = new List<Keys>() {Keys.W};
        Down = new List<Keys>() {Keys.S};
        Left = new List<Keys>() {Keys.A};
        Right = new List<Keys>() {Keys.D};
        Select = new List<Keys>() {Keys.Enter};
        Visible = true;
    }

    internal void AddElement(UIElement element)
    {
        menuItems.Add(element);
        if (menuItems.Count == 1)
            selectedElement = element;
    }

    internal void Update(GameTime gameTime, Scene scene)
    {
        // TODO: Is this required at the element level?
        //foreach (UIElement uiElement in menuItems)
        //{
        //    uiElement.Update(gameTime, selected: selectedElement == uiElement);
        //}
    }

    internal void Input(GameTime gameTime, Scene scene)
    {

        if (selectedElement == null)
            return;

        // To go Up
        UIElement? resultUp = GetNextElement(Up, e => e.ElementAbove);
        if (resultUp != null) selectedElement = resultUp;

        // To go Down
        UIElement? resultDown = GetNextElement(Down, e => e.ElementBelow);
        if (resultDown != null) selectedElement = resultDown;

        // To go Left
        UIElement? resultLeft = GetNextElement(Left, e => e.ElementLeft);
        if (resultLeft != null) selectedElement = resultLeft;

        // To go Right
        UIElement? resultRight = GetNextElement(Right, e => e.ElementRight);
        if (resultRight != null) selectedElement = resultRight;

        // Select
        foreach (Keys key in Select)
        {
            if (EngineGlobals.game.inputManager.IsKeyPressed(key))
            {
                if (selectedElement.OnSelected != null)
                {
                    selectedElement.OnSelected(selectedElement, scene);
                }
            }
        }
  
    }

    private UIElement? GetNextElement(List<Keys> keys, Func<UIElement, UIElement?> getDirection)
    {
        // 1. Check if any of the keys were pressed
        if (keys.Exists(key => EngineGlobals.game.inputManager.IsKeyPressed(key)))
        {
            // 2. Start from the neighbor in the specified direction
            UIElement? next = getDirection(selectedElement);

            // 3. Keep searching while we hit inactive elements
            while (next != null && !next.Active)
            {
                next = getDirection(next);
            }

            // 4. Return the result (could be an active element or null)
            return next;
        }
        
        return null;
    }

    internal void Draw()
    {

        if (Visible == false)
            return;

        foreach (UIElement uiElement in menuItems)
        {
            uiElement.Draw(selected: selectedElement == uiElement);
        }         
    }

}