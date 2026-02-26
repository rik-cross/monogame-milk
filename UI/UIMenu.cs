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
    public List<Keys> Up { get; set; }

    /// <summary>
    /// The key(s) pressed to move 'down' the menu
    /// (i.e. to select the `ElementBelow` for the current `UIElement`,
    /// default = `keys.S`.)
    /// </summary>
    public List<Keys> Down { get; set; }

    /// <summary>
    /// The key(s) pressed to move 'left' in the menu
    /// (i.e. to select the `ElementLeft` for the current `UIElement`,
    /// default = `keys.A`.)
    /// </summary>
    public List<Keys> Left { get; set; }

    /// <summary>
    /// The key(s) pressed to move 'right' in the menu
    /// (i.e. to select the `ElementRight` for the current `UIElement`, 
    /// default = `keys.D`.)
    /// </summary>
    public List<Keys> Right { get; set; }

    /// <summary>
    /// The key(s) pressed to 'select' the current `UIElement`.
    /// (`keys.Enter` by default.)
    /// </summary>
    public List<Keys> Select { get; set; }

    /// <summary>
    /// Setting a menu visibility to 'false' hides the menu and all of its UIElements.
    /// </summary>
    public bool Visible { get; set; }

    public bool AllowInputWrapping { get; set; }

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
        AllowInputWrapping = true;
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

        GetNextElement(Up, e => e.ElementAbove, e => e.ElementBelow);
        GetNextElement(Down, e => e.ElementBelow, e => e.ElementAbove);
        GetNextElement(Left, e => e.ElementLeft, e => e.ElementRight);
        GetNextElement(Right, e => e.ElementRight, e => e.ElementLeft);

        // Call selected element
        foreach (Keys key in Select)
            if (EngineGlobals.game.inputManager.IsKeyPressed(key))
                if (selectedElement.Active && selectedElement.OnSelected != null)
                    selectedElement.OnSelected(selectedElement, scene);
    }

    private void GetNextElement(List<Keys> keys, Func<UIElement, UIElement?> getForward, Func<UIElement, UIElement?> getBackward)
    {
        if (!keys.Exists(key => EngineGlobals.game.inputManager.IsKeyPressed(key)))
            return;

        UIElement initialElement = selectedElement!;
        
        // 1. Try to find the next active element forward
        UIElement? next = getForward(selectedElement);
        while (next != null && !next.Active)
            next = getForward(next);

        if (next != null && next.Active)
        {
            selectedElement = next;
            return;
        }

        // 2. Wrap if allowed
        if (AllowInputWrapping)
        {
            UIElement? boundary = selectedElement;
            while (getBackward(boundary!) != null)
                boundary = getBackward(boundary!);

            UIElement? wrapped = boundary;
            while (wrapped != null && !wrapped.Active && wrapped != initialElement)
                wrapped = getForward(wrapped);

            if (wrapped != null && wrapped.Active && wrapped != initialElement)
            {
                selectedElement = wrapped;
                return;
            }
        }

        return;
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