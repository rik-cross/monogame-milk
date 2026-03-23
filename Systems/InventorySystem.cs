//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using milk.Core;
using milk.Components;

namespace milk.Systems;

/// <summary>
/// The inventory system processes and draws all inventory components.
/// </summary>
public class InventorySystem : milk.Core.System
{

    //
    // Inventory data, mapped against Entity.Type
    //
    
    // Maps an entity type to a stack size
    private Dictionary<string, int> _stackSizes = new Dictionary<string, int>();
    // Maps an entity type to a texture
    private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

    /// <summary>
    /// If true, subsequent systems will not process input (default = false).
    /// </summary>
    public bool ConsumeInput { get; set; } = false;

    /// <summary>
    /// Initialises the inventory system, which only processes entities
    /// with an inventory component.
    /// </summary>
    public override void Init()
    {
        AddRequiredComponentType<InventoryComponent>();
    }

    /// <summary>
    /// Iterates over all input actions, and runs the
    /// relevant mapped methods.
    /// </summary>
    /// <param name="gameTime">Elapsed game time.</param>
    /// <param name="scene">The scene containing the system.</param>
    /// <param name="entity">The entity performing the input.</param>
    /// <returns></returns>
    public override bool InputEntity(GameTime gameTime, Scene scene, Entity entity)
    {

        InventoryComponent inventoryComponent = entity.GetComponent<InventoryComponent>()!;

        // Process active value
        if (inventoryComponent.HasActiveValueChanged == true)
        {
            inventoryComponent.HasActiveValueChanged = false;
            if (inventoryComponent.Active == true)
                inventoryComponent.OnActivate?.Invoke(inventoryComponent, scene);
            else
                inventoryComponent.OnDeactivate?.Invoke(inventoryComponent, scene);
        }

        // Process the specified input
        foreach (var input in inventoryComponent.InputActions)
        {
            if (Milk.Controls.IsKeyPressed(input.Key))
            {
                input.Value.Invoke();
            }
        }

        // Defer to the ConsumeInput to decide whether input should be consumed.
        return ConsumeInput;

    }

    /// <summary>
    /// Draws all inventory components.
    /// </summary>
    /// <param name="scene">The scene containing the system.</param>
    public override void Draw(Scene scene)
    {

        // Iterate over all entities...
        foreach (Entity entity in scene.entities)
        {
            // ... with an inventory component
            if (entity.HasComponent<InventoryComponent>() == true)
            {
                
                // Get the inventory component
                InventoryComponent inventoryComponent = entity.GetComponent<InventoryComponent>()!;

                // Don't draw unless visible
                if (inventoryComponent.Visible == false)
                    continue;

                // Defer to the custom draw method, if one has been specified
                if (inventoryComponent.CustomDrawMethod != null)
                {
                    inventoryComponent.CustomDrawMethod(inventoryComponent);
                    continue;
                }
                
                // Get the top-left position of the component,
                // which may not be the same as the Position property
                // if an anchor other than Anchor.TopLeft has been set
                Vector2 adjustedPosition = inventoryComponent.CalculateTopLeftPositionFromAnchor();

                // Use the value of 'Active' to set the color scheme
                Color primaryColor = inventoryComponent.Active ? inventoryComponent.PrimaryColor : inventoryComponent.PrimaryColorInactive;
                Color secondaryColor = inventoryComponent.Active ? inventoryComponent.SecondaryColor : inventoryComponent.SecondaryColorInactive;
                float alpha = inventoryComponent.Active ? inventoryComponent.Alpha : inventoryComponent.AlphaInactive;

                // Calculate the background size
                int columns = Math.Min(inventoryComponent.NumberOfSlots, inventoryComponent.SlotsPerRow);
                int rows = (int)Math.Ceiling((double)inventoryComponent.NumberOfSlots / inventoryComponent.SlotsPerRow);
                int totalWidth = (int)(columns * inventoryComponent.SlotSize.X + (columns + 1) * inventoryComponent.Margin);
                int totalHeight = (int)(rows * inventoryComponent.SlotSize.Y + (rows + 1) * inventoryComponent.Margin);

                // Draw the background
                Milk.Graphics.FillRectangle(
                    new Rectangle((int)adjustedPosition.X, (int)adjustedPosition.Y, totalWidth, totalHeight),
                    primaryColor * alpha
                );

                // Draw each slot
                for (int i = 0; i<inventoryComponent.NumberOfSlots; i++)
                {

                    // Calculate slot top-left position
                    Vector2 pos = new Vector2(
                        adjustedPosition.X + inventoryComponent.Margin + ( (i % inventoryComponent.SlotsPerRow) * (inventoryComponent.SlotSize.X + inventoryComponent.Margin)),
                        (int)(adjustedPosition.Y + inventoryComponent.Margin + ( (inventoryComponent.Margin + inventoryComponent.SlotSize.Y) * (Math.Floor((double)(i / inventoryComponent.SlotsPerRow)))))
                    );

                    // Calculate the size of the slot border, depending on
                    // whether it is the selected slot or not
                    Rectangle r;
                    if (i == inventoryComponent.SelectedSlot)
                        r = new Rectangle(
                            (int)pos.X - inventoryComponent.Margin / 2,
                            (int)pos.Y - inventoryComponent.Margin / 2,
                            (int)inventoryComponent.SlotSize.X + inventoryComponent.Margin,
                            (int)inventoryComponent.SlotSize.Y + inventoryComponent.Margin
                        );
                    else
                        r = new Rectangle(
                            (int)pos.X - inventoryComponent.Margin / 4,
                            (int)pos.Y - inventoryComponent.Margin / 4,
                            (int)inventoryComponent.SlotSize.X + inventoryComponent.Margin / 2,
                            (int)inventoryComponent.SlotSize.Y + inventoryComponent.Margin / 2
                        );

                    // Draw the slot border
                    Milk.Graphics.DrawRectangle(
                        r,
                        secondaryColor * alpha,
                        i == inventoryComponent.SelectedSlot ? inventoryComponent.Margin / 2 : inventoryComponent.Margin / 4
                    );

                    // Draw the texture of the entity in the slot
                    // (if the slot is being used)
                    if (
                        inventoryComponent.inventory[i].Items.Count > 0 &&
                        _textures.ContainsKey(inventoryComponent.inventory[i].Type)    
                    )
                    {
                        // Fit the texture to the available slot size
                        Utilities.DrawTextureToContainerSize(
                            _textures[inventoryComponent.inventory[i].Type],
                            new Rectangle(
                                (int)pos.X + inventoryComponent.Margin, (int)pos.Y + inventoryComponent.Margin,
                                (int)inventoryComponent.SlotSize.X - + inventoryComponent.Margin * 2,
                                (int)inventoryComponent.SlotSize.Y - + inventoryComponent.Margin * 2
                            ),
                            Color.White * alpha
                        );
                    }

                    // Draw the number of entities stored in the slot
                    // (if the slot is being used)
                    if (inventoryComponent.inventory[i].Items.Count > 0)
                    {
                        string numberOfEntities = inventoryComponent.inventory[i].Items.Count.ToString();
                        Milk.Graphics.DrawString(
                            inventoryComponent.Font,
                            numberOfEntities,
                            new Vector2(
                                pos.X + inventoryComponent.SlotSize.X - inventoryComponent.Font.MeasureString(numberOfEntities).X - inventoryComponent.Margin,
                                pos.Y + inventoryComponent.SlotSize.Y - inventoryComponent.Font.MeasureString(numberOfEntities).Y 
                            ),
                            secondaryColor * alpha
                        );
                    }

                }

                // Draw the type of the entity in the selected slot
                // (only if the inventory is active and there's an entity
                // stored in the selected slot)
                if (
                    inventoryComponent.Active == true &&
                    inventoryComponent.inventory[inventoryComponent.SelectedSlot].Type != null
                )
                {
                    string typeString = inventoryComponent.inventory[inventoryComponent.SelectedSlot].Type;
                    typeString = char.ToUpper(typeString[0]) + typeString.Substring(1);
                    Milk.Graphics.DrawString(
                        inventoryComponent.Font,
                        typeString,
                        new Vector2(
                            adjustedPosition.X + totalWidth - inventoryComponent.Font.MeasureString(typeString).X,
                            adjustedPosition.Y - inventoryComponent.Font.MeasureString(typeString).Y
                        ),
                        primaryColor
                    );
                }

            }

        }

    }

    //
    // Entity stack sizes
    //

    /// <summary>
    /// Sets the stack size for an entity type.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="stackSize">The stack size.</param>
    public void SetStackSize(string entityType, int stackSize)
    {
        if (_stackSizes.ContainsKey(entityType))
            _stackSizes[entityType] = stackSize;
        else
            _stackSizes.Add(entityType, stackSize);
    }

    /// <summary>
    /// Gets the stack size for an entity type.
    /// </summary>
    /// <param name="entityType">The entity type to get the stack size for.</param>
    /// <returns>The stack size stored, or the default value of 1 otherwise.</returns>
    public int GetStackSize(string entityType)
    {
        if (_stackSizes.ContainsKey(entityType))
            return _stackSizes[entityType];
        return 1;
    }

    //
    // Entity textures
    //

    /// <summary>
    /// Sets the texture displayed for an entity type.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="texture">The texture to associate with the entity type.</param>
    public void SetTexture(string entityType, Texture2D texture)
    {
        if (_textures.ContainsKey(entityType))
            _textures[entityType] = texture;
        else
            _textures.Add(entityType, texture);
    }

    /// <summary>
    /// Gets the texture for the specified entity type.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>The texture stored for the entity type, or null.</returns>
    public Texture2D? GetTexture(string entityType)
    {
        if (_textures.ContainsKey(entityType))
            return _textures[entityType];
        return null;
    }

}