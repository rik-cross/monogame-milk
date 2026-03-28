//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using milk.Core;
using milk.UI;

namespace milk.Components;

/// <summary>
/// An CraftingComponent manages recipes.
/// </summary>
public class CraftingComponent : Component
{

    // Maps an Entity.Tag to an InventorySlot
    public List<CraftingSlot> inventory;

    private int _numberOfSlots;
    /// <summary>
    /// The number of spaces in the inventory.
    /// </summary>
    public int NumberOfSlots {
        get
        {
            return _numberOfSlots;
        }
        set
        {
            _numberOfSlots = value;
            while (inventory.Count < _numberOfSlots)
                inventory.Add(new CraftingSlot());
            CalculateSize();
        }
    }

    private int _selectedSlot;
    /// <summary>
    /// The index of the currently selected slot,
    /// from left-to-right, and top-to-bottom.
    /// The selected slot number is clamped to the
    /// number of available slots.
    /// </summary>
    public int SelectedSlot
    {
        get
        {
            return _selectedSlot;
        }
        set
        {
            _selectedSlot = Math.Clamp(value, 0, inventory.Count - 1);
        }
    }

    /// <summary>
    /// Allows selecting previous and next slots to wrap
    /// around to the other side of the component.
    /// </summary>
    public bool allowInputWrapping { get; set; }

    /// <summary>
    /// Get the size of the component. The size can't
    /// be set directly, as it depends on the number of
    /// inventory slots, and the margin between slots.
    /// </summary>
    public Vector2 Size { get; private set; }

    /// <summary>
    /// The (x, y) position of the component, relative
    /// to its Anchor, which default to the top-left.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// The part of the component to set the position against.
    /// </summary>
    public Anchor Anchor { get; set; }

    private Vector2 _slotSize;
    /// <summary>
    /// The (x, y) size of each inventory slot, in pixels.
    /// </summary>
    public Vector2 SlotSize
    {
        get
        {
            return _slotSize;
        }
        set
        {
            _slotSize = value;
            CalculateSize();
        }
    }

    private int _slotsPerRow;
    /// <summary>
    /// The number of inventory slots per row of the drawn inventory.
    /// </summary>
    public int SlotsPerRow
    {
        get
        {
            return _slotsPerRow;
        }
        set
        {
            _slotsPerRow = value;
            CalculateSize();
        }
    }

    private int _margin;
    /// <summary>
    /// The space (in pixels) between inventory slots.
    /// </summary>
    public int Margin
    {
        get
        {
            return _margin;
        }
        set
        {
            _margin = value;
            CalculateSize();
        }
    }

    /// <summary>
    /// The opcity of the component when active, between 0 and 1.
    /// </summary>
    public float Alpha { get; set; }

    /// <summary>
    /// The opcity of the component when inactive, between 0 and 1.
    /// </summary>
    public float AlphaInactive { get; set; }

    /// <summary>
    /// The main color used to draw the inventory.
    /// </summary>
    public Color PrimaryColor;

    /// <summary>
    /// The main color used to draw the inventory when inactive.
    /// </summary>
    public Color PrimaryColorInactive;

    /// <summary>
    /// The accent color used to draw the inventory.
    /// </summary>
    public Color SecondaryColor;
    
    /// <summary>
    /// The accent color used to draw the inventory when inactive.
    /// </summary>
    public Color SecondaryColorInactive;
    
    /// <summary>
    /// The font used to draw entity names and amounts.
    /// </summary>
    public SpriteFont Font;

    /// <summary>
    /// Specifies a method used to override the default Draw() method.
    /// </summary>
    public Action<Entity>? CustomDrawMethod;

    private bool _visible;
    /// <summary>
    /// Hides and shows the component.
    /// </summary>
    public bool Visible
    {
        get
        {
            return _visible;
        }
        set
        {
            // Set the selected slot if turning visible
            if (_visible == false && value == true)
                SelectedSlot = 0;
            _visible = value;
        }
    }

    private bool _active;
    internal bool HasActiveValueChanged = false;
    /// <summary>
    /// Enables or disables use of the component.
    /// </summary>
    public bool Active
    {
        get
        {
            return _active;
        }
        set
        {
            _active = value;
            HasActiveValueChanged = true;
        }
    }

    public Action<CraftingComponent, Scene>? OnActivate;
    public Action<CraftingComponent, Scene>? OnDeactivate;

    /// <summary>
    /// Maps keys to inventory methods.
    /// </summary>
    public Dictionary<Keys, Action> InputActions { get; set; } = new Dictionary<Keys, Action>();
    
    internal bool WaitingToCraft;
    public bool IsCrafting;
    public CraftingRecipe? RecipeToCraft;
    public int SlotNumberToCraft;
    public double CurrentCraftingDuration;
    public Action? OnCraftStart;
    public Action? OnCraftEnd;

    /// <summary>
    /// Creates a new inventory.
    /// </summary>
    /// <param name="numberOfSlots">The number of inventory slots (default = 8).</param>
    /// <param name="allowInputWrapping">Allows cyclical movement between inventory slots (default = false).</param>
    /// <param name="position">The (x, y) position of the inventory (default = null - (0, 0)).</param>
    /// <param name="anchor">The anchor to set the position against (default = Anchor.TopLeft).</param>
    /// <param name="slotSize">The (w, h) slot size in pixels (default = null - (64, 64)).</param>
    /// <param name="slotsPerRow">The number of slots in a row (default = null - all on a single row).</param>
    /// <param name="margin">The gap between slots (default = 8).</param>
    /// <param name="alpha">The opacity of the inventory, between 0 anf 1 (default = 1 - fully opaque).</param>
    /// <param name="alphaInactive">The opacity of the inventory when inactive, between 0 anf 1 (default = 1 - fully opaque).</param>
    /// <param name="primaryColor">The main color used to draw the inventory (default = null - Color.White).</param>
    /// <param name="primaryColorInactive">The main color used to draw the inventory when inactive (default = null - Color.LightGray).</param>
    /// <param name="secondaryColor">The accent color used to draw the inventory (default = null - Color.Black).</param>
    /// <param name="secondaryColorInactive">The accent color used to draw the inventory when inactive (default = null - Color.DarkGray).</param>
    /// <param name="font">The font used to draw slot names and numbers (default = null - FontSmall).</param>
    /// <param name="customDrawMethod">Specifies a method to override the drawing of the inventory (default = null).</param>
    /// <param name="visible">Sets the visibility of the inventory (default = true).</param>
    /// <param name="active">Sets the ability to interact with the component (default = true).</param>
    /// <param name="inputActions">Maps Keys to Actions (default = null - no actions mapped).</param>
    public CraftingComponent(
        int numberOfSlots = 8,
        bool allowInputWrapping = false,
        Vector2? position = null,
        Anchor anchor = Anchor.TopLeft,
        Vector2? slotSize = null,
        int? slotsPerRow = null,
        int margin = 8,
        float alpha = 1.0f,
        float alphaInactive = 1.0f,
        Color? primaryColor = null,
        Color? primaryColorInactive = null,
        Color? secondaryColor = null,
        Color? secondaryColorInactive = null,
        SpriteFont? font = null,
        Action<Entity>? customDrawMethod = null,
        bool visible = true,
        bool active = true,
        Dictionary<Keys, Action>? inputActions = null
    )
    {

        // Create a new list of empty inventory slots
        inventory = new List<CraftingSlot>();
        for (int i=0; i<numberOfSlots; i++)
            inventory.Add(new CraftingSlot());

        // Slots
        NumberOfSlots = numberOfSlots;
        SelectedSlot = 0;
        this.allowInputWrapping = allowInputWrapping;

        // Position and sizing
        Position = position ?? Vector2.Zero;
        Anchor = anchor;
        SlotSize = slotSize ?? new Vector2(64, 64);
        SlotsPerRow = slotsPerRow ?? numberOfSlots;
        Margin = margin;

        // Look and feel
        Alpha = alpha;
        AlphaInactive = alphaInactive;
        PrimaryColor = primaryColor ?? Color.White;
        PrimaryColorInactive = primaryColorInactive ?? Color.LightGray;
        SecondaryColor = secondaryColor ?? Color.Black;
        SecondaryColorInactive = secondaryColorInactive ?? Color.DarkGray;
        Font = font ?? EngineGlobals.game._engineResources.FontSmall;
        CustomDrawMethod = customDrawMethod;
        
        // Usage
        Visible = visible;
        Active = active;
        
        // Input action mappings
        InputActions = inputActions ?? new Dictionary<Keys, Action>();

        CalculateSize();

        // Todo: add to constructor
        WaitingToCraft = false;
        IsCrafting = false;
        CurrentCraftingDuration = 0;
        OnCraftStart = null;
        OnCraftEnd = null;
        SlotNumberToCraft = -1;

    }

    public CraftingSlot GetSlot(int? slotNumber = null)
    {
        if (slotNumber == null)
            return inventory[SelectedSlot];
        else
            return inventory[slotNumber.Value];
    }

    public void Craft()
    {
        WaitingToCraft = true;
    }

    public bool CanBeCrafted(int slotNumber)
    {
        // todo
        return true;
    }

    public CraftingRecipe? GetRecipe(int? slotNumber = null)
    {
        int slot = slotNumber ?? SelectedSlot;
        return inventory[slot].Recipe;
    }

    public string? GetRecipeInfo(int? slotNumber = null)
    {
        int slot = slotNumber ?? SelectedSlot;
        if (inventory[slot] != null && inventory[slot].Recipe != null)
            return "1 x wood\n3 x something";
        else
            return null;
    }

    // overwrites
    public bool AddRecipe(CraftingRecipe recipe, int? slotNumber = null)
    {

        bool added = false;
        int slot = -1;
        
        // Find the slot to add to
        if (slotNumber == null)
        {
            // Find first available slot
            for (int i=0; i<inventory.Count; i++)
            {
                if (inventory[i].Recipe == null)
                {
                    slot = i;
                    break;
                }
            }
        }
        else
        {
            slot = slotNumber.Value;
        }

        if (slot != -1 && inventory[slot] != null)
        {
            inventory[slot].Recipe = recipe;
            added = true;
        }

        return added;
    }

    public void ClearRecipeType(string type)
    {
        foreach(CraftingSlot slot in inventory)
        {
            if (slot.Recipe != null && slot.Recipe.EntityTypeCreated == type)
                slot.Clear();
        }
    }

    public void Clear(int? slotNumber = null)
    {
        if (slotNumber == null)
        {
            foreach (CraftingSlot slot in inventory)
                slot.Clear();  
        }
        else
        {
            inventory[slotNumber.Value].Clear();
        }
    }

    //
    // Slot selection methods
    //

    /// <summary>
    /// Selects the inventory slot above the currently selected slot, if one exists.
    /// If allowInputWrapping == true, the selection wraps around to the bottom-most slot.
    /// </summary>
    /// <returns>Returns true if the selected slot was already the top-most.</returns>
    public bool SelectAbove()
    {
        bool limit = false;
        // Check if we are in the top row
        if (SelectedSlot < SlotsPerRow)
        {
            if (allowInputWrapping)
            {
                int rows = (int)Math.Ceiling((double)NumberOfSlots / SlotsPerRow);
                SelectedSlot = (SelectedSlot - SlotsPerRow + (rows * SlotsPerRow)) % (rows * SlotsPerRow);
                
                // If the wrap lands on a non-existent slot in a jagged bottom row, 
                // clamp to the absolute last item.
                if (SelectedSlot >= NumberOfSlots) SelectedSlot = NumberOfSlots - 1;
            }
            else
                limit = true;
        }
        else
            SelectedSlot -= SlotsPerRow;
        return limit;
    }

    /// <summary>
    /// Selects the inventory slot below the currently selected slot, if one exists.
    /// If allowInputWrapping == true, the selection wraps around to the top-most slot.
    /// </summary>
    /// <returns>Returns true if the selected slot was already the bottom-most.</returns>
    public bool SelectBelow()
    {
        bool limit = false;
        // Check if moving down would exceed the number of slots
        if (SelectedSlot + SlotsPerRow >= NumberOfSlots)
        {
            if (allowInputWrapping)
                // Wrap to the top row (same column)
                SelectedSlot %= SlotsPerRow;
            else
                limit = true;
        }
        else
            SelectedSlot += SlotsPerRow;
        return limit;
    }

    /// <summary>
    /// Selects the inventory slot to the left of the currently selected slot, if one exists.
    /// If allowInputWrapping == true, the selection wraps around to the right-most slot.
    /// </summary>
    /// <returns>Returns true if the selected slot was already the left-most.</returns>
    public bool SelectLeft()
    {
        bool limit = false;
        int currentRow = SelectedSlot / SlotsPerRow;
        int firstInRow = currentRow * SlotsPerRow;
        int lastInRow = Math.Min(firstInRow + SlotsPerRow, NumberOfSlots) - 1;

        if (SelectedSlot == firstInRow)
        {
            if (allowInputWrapping) SelectedSlot = lastInRow;
            else limit = true;
        }
        else
            SelectedSlot--;
        return limit;
    }

    /// <summary>
    /// Selects the inventory slot to the right of the currently selected slot, if one exists.
    /// If allowInputWrapping == true, the selection wraps around to the left-most slot.
    /// </summary>
    /// <returns>Returns true if the selected slot was already the right-most.</returns>
    public bool SelectRight()
    {
        bool limit = false;
        int currentRow = SelectedSlot / SlotsPerRow;
        int firstInRow = currentRow * SlotsPerRow;
        int lastInRow = Math.Min(firstInRow + SlotsPerRow, NumberOfSlots) - 1;

        if (SelectedSlot == lastInRow)
        {
            if (allowInputWrapping) SelectedSlot = firstInRow;
            else limit = true;
        }
        else
            SelectedSlot++;
        return limit;
    }

    /// <summary>
    /// Calculate the size of the inventory, using the number of slots
    /// / slot size, the number of slots per row and the margin between slots.
    /// </summary>
    private void CalculateSize()
    {
        // Calculate the background size
        int columns = Math.Min(NumberOfSlots, SlotsPerRow);
        int rows = (int)Math.Ceiling((double)NumberOfSlots / SlotsPerRow);
        int totalWidth = (int)(columns * SlotSize.X + (columns + 1) * Margin);
        int totalHeight = (int)(rows * SlotSize.Y + (rows + 1) * Margin);
        // Set the size vector
        Size = new Vector2(
            totalWidth, totalHeight
        );
    }

    /// <summary>
    /// Uses the anchor to find the top-left position.
    /// </summary>
    /// <returns></returns>
    public Vector2 CalculateTopLeftPositionFromAnchor()
    {
        float newX = Position.X;
        float newY = Position.Y;

        if (Anchor == Anchor.TopCenter || Anchor == Anchor.MiddleCenter || Anchor == Anchor.BottomCenter)
            newX -= Size.X / 2;
        if (Anchor == Anchor.TopRight || Anchor == Anchor.MiddleRight || Anchor == Anchor.BottomRight)
            newX -= Size.X;
        if (Anchor == Anchor.MiddleLeft || Anchor == Anchor.MiddleCenter || Anchor == Anchor.MiddleRight)
            newY -= Size.Y / 2;
        if (Anchor == Anchor.BottomLeft || Anchor == Anchor.BottomCenter || Anchor == Anchor.BottomRight)
            newY -= Size.Y;

        return new Vector2(newX, newY);
    }

    //
    // Properties
    //

    /// <summary>
    /// The width.
    /// </summary>
    public float Width
    {
        get { return Size.X; }
        set { Size = new Vector2(value, Size.Y); }
    }

    /// <summary>
    /// The height.
    /// </summary>
    public float Height
    {
        get { return Size.Y; }
        set { Size = new Vector2(Size.Y, value); }
    }

    /// <summary>
    /// The x position.
    /// </summary>
    public float X
    {
        get { return Position.X; }
        set { Position = new Vector2(value, Position.Y); }
    }

    /// <summary>
    /// The y position.
    /// </summary>
    public float Y
    {
        get { return Position.Y; }
        set { Position = new Vector2(Position.X, value); }
    }

    /// <summary>
    /// The top (y) position.
    /// </summary>
    public float Top
    {
        get { return Position.Y; }
        set { Position = new Vector2(Position.X, value); }
    }

    /// <summary>
    /// The middle (y + 1/2 height).
    /// </summary>
    public float Middle
    {
        get { return Position.Y + (Size.Y / 2); }
        set { Position = new Vector2(Position.X, value - (Size.Y / 2)); }
    }

    /// <summary>
    /// The bottom (y + height).
    /// </summary>
    public float Bottom
    {
        get { return Position.Y + Size.Y; }
        set { Position = new Vector2(Position.X, value - Size.Y); }
    }

    /// <summary>
    /// The left (x) position.
    /// </summary>
    public float Left
    {
        get { return Position.X; }
        set { Position = new Vector2(value, Position.Y); }
    }

    /// <summary>
    /// The center (x + 1/2 width).
    /// </summary>
    public float Center
    {
        get { return Position.X + (Size.X / 2); }
        set { Position = new Vector2(value - (Size.X / 2), Position.Y); }
    }

    /// <summary>
    /// The right (x + width).
    /// </summary>
    public float Right
    {
        get { return Position.X + Size.X; }
        set { Position = new Vector2(value - Size.X, Position.Y); }
    }

}