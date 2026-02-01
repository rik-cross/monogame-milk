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

    /// <summary>
    /// The key(s) pressed to move 'up' the menu
    /// (i.e. to select the `ElementAbove` for the current `UIElement`,
    /// default = `keys.W`.)
    /// </summary>
    public List<Keys> Up {get; set; }

    /// <summary>
    /// The key(s) pressed to move 'down' the menu
    /// (i.e. to select the `ElementBelow` for the current `UIElement`,
    /// default = `keys.S`.)
    /// </summary>
    public List<Keys> Down {get; set; }

    /// <summary>
    /// The key(s) pressed to move 'left' in the menu
    /// (i.e. to select the `ElementLeft` for the current `UIElement`,
    /// default = `keys.A`.)
    /// </summary>
    public List<Keys> Left {get; set; }

    /// <summary>
    /// The key(s) pressed to move 'right' in the menu
    /// (i.e. to select the `ElementRight` for the current `UIElement`, 
    /// default = `keys.D`.)
    /// </summary>
    public List<Keys> Right {get; set; }

    /// <summary>
    /// The key(s) pressed to 'select' the current `UIElement`.
    /// (`keys.Enter` by default.)
    /// </summary>
    public List<Keys> Select {get; set; }

    /// <summary>
    /// Setting a menu visibility to 'false' hides the menu and all of its UIElements.
    /// </summary>
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

        // To select
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

        // Don't draw any of the menu's elements if visible = false.
        if (Visible == false)
            return;
        
        // Otherwise, draw each element.
        foreach (UIElement uiElement in menuItems)
        {
            uiElement.Draw(selected: selectedElement == uiElement);
        }         
    }

}