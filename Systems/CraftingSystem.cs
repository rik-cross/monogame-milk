//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using milk.Core;
using milk.Components;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Systems;

/// <summary>
/// Provides methods for dropping and picking up entities.
/// Note that only entities with a CollectableComponent and
/// a TriggerComponent can be collected.
/// </summary>
public class CraftingSystem : milk.Core.System
{

    // Maps an entity type to a texture
    private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

    /// <summary>
    /// If true, subsequent systems will not process input (default = false).
    /// </summary>
    public bool ConsumeInput { get; set; } = false;

    public override void Init()
    {
        AddRequiredComponentType<CraftingComponent>();
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

        CraftingComponent craftingComponent = entity.GetComponent<CraftingComponent>()!;

        if (craftingComponent.IsCrafting == true)
            return ConsumeInput;

        // Process the specified input
        foreach (var input in craftingComponent.InputActions)
        {
            if (Milk.Controls.IsKeyPressed(input.Key))
            {
                input.Value.Invoke();
            }
        }

        // Defer to the ConsumeInput to decide whether input should be consumed.
        return ConsumeInput;

    }

    public override void UpdateEntity(GameTime gameTime, Scene scene, Entity entity)
    {

        CraftingComponent craftingComponent = entity.GetComponent<CraftingComponent>()!;
        InventoryComponent inventoryComponent = entity.GetComponent<InventoryComponent>()!;
        
        // Process active value
        if (craftingComponent.HasActiveValueChanged == true)
        {
            craftingComponent.HasActiveValueChanged = false;
            if (craftingComponent.Active == true)
                craftingComponent.OnActivate?.Invoke(craftingComponent, scene);
            else
                craftingComponent.OnDeactivate?.Invoke(craftingComponent, scene);
        }

        // Start crafting

        if (
            craftingComponent.WaitingToCraft == true &&
            craftingComponent.IsCrafting == false &&
            craftingComponent.inventory[craftingComponent.SelectedSlot].Recipe != null
        )
        {

            craftingComponent.WaitingToCraft = false;

            bool canCraft = true;

            // Check if the selected component can be crafted
            CraftingRecipe recipe = craftingComponent.inventory[craftingComponent.SelectedSlot].Recipe!;

            // TODO
            // Store the position when the craft button was pressed,
            // not the current index 2 seconds later!

            foreach(var ingredient in recipe.Ingredients)
            {
                if (inventoryComponent.NumberOfEntityType(ingredient.Key) < ingredient.Value)
                {
                    canCraft = false;
                    Log.Add(
                        "Cannot craft " +
                        craftingComponent.inventory[craftingComponent.SlotNumberToCraft].Recipe!.EntityTypeCreated
                    );
                    break;
                }
            }

            // Start the crafting process
            // Allowing for possible delay
            if (canCraft == true)
            {
                craftingComponent.IsCrafting = true;
                craftingComponent.RecipeToCraft = recipe;
                craftingComponent.SlotNumberToCraft = craftingComponent.SelectedSlot;
                craftingComponent.OnCraftStart?.Invoke();
            }
        
        }

        // Process crafting
        if (craftingComponent.IsCrafting == true && craftingComponent.RecipeToCraft != null)
        {
            
            craftingComponent.CurrentCraftingDuration = Math.Min(
                craftingComponent.CurrentCraftingDuration + gameTime.ElapsedGameTime.TotalSeconds,
                craftingComponent.RecipeToCraft.Duration
            );

            // End crafting
            if (craftingComponent.CurrentCraftingDuration == craftingComponent.RecipeToCraft.Duration)
            {

                craftingComponent.CurrentCraftingDuration = 0;
                craftingComponent.IsCrafting = false;
                craftingComponent.OnCraftEnd?.Invoke();
                craftingComponent.inventory[craftingComponent.SlotNumberToCraft].LastUsed = gameTime.TotalGameTime.Seconds;
                Log.Add(
                    "Crafted " +
                    craftingComponent.inventory[craftingComponent.SlotNumberToCraft].Recipe!.EntityTypeCreated
                );

                // Create entity
                if (Milk.Entities.HasPrototype(craftingComponent.RecipeToCraft.EntityTypeCreated))
                {
                    Entity? e = Milk.Entities.CreateFromPrototype(
                        craftingComponent.RecipeToCraft.EntityTypeCreated,
                        new Vector2(0,0) // TODO...
                    );

                    if (e != null)
                    {
                        // try to add to the inventory
                        bool hasAdded = inventoryComponent.AddEntity(e);

                        // if no space, drop on ground
                        if (hasAdded == false)
                            Milk.Systems.GetSystem<CollectionSystem>().DropEntity(scene, entity, e);

                    }
                        
                }

                // Remove entities
                foreach (var s in craftingComponent.RecipeToCraft.Ingredients)
                {
                    for (int x=0; x<s.Value; x++)
                        inventoryComponent.RemoveEntityOfType(s.Key);
                }

                inventoryComponent.Consolidate();
                
                craftingComponent.RecipeToCraft = null;

            }

        }

    }

    /// <summary>
    /// Draws all crafting components.
    /// </summary>
    /// <param name="scene">The scene containing the system.</param>
    public override void Draw(Scene scene)
    {

        // Iterate over all entities...
        foreach (Entity entity in scene.Entities)
        {
            // ... with an crafting component
            if (entity.HasComponent<CraftingComponent>() == true)
            {

                // Get the crafting component
                CraftingComponent craftingComponent = entity.GetComponent<CraftingComponent>()!;

                // Don't draw unless visible
                if (craftingComponent.Visible == false)
                    continue;

                // Defer to the custom draw method, if one has been specified
                if (craftingComponent.CustomDrawMethod != null)
                {
                    craftingComponent.CustomDrawMethod(entity);
                    continue;
                }
                
                // Get the top-left position of the component,
                // which may not be the same as the Position property
                // if an anchor other than Anchor.TopLeft has been set
                Vector2 adjustedPosition = craftingComponent.CalculateTopLeftPositionFromAnchor();

                // Use the value of 'Active' to set the color scheme
                Color primaryColor = craftingComponent.Active ? craftingComponent.PrimaryColor : craftingComponent.PrimaryColorInactive;
                Color secondaryColor = craftingComponent.Active ? craftingComponent.SecondaryColor : craftingComponent.SecondaryColorInactive;
                float alpha = craftingComponent.Active ? craftingComponent.Alpha : craftingComponent.AlphaInactive;

                // Calculate the background size
                int columns = Math.Min(craftingComponent.NumberOfSlots, craftingComponent.SlotsPerRow);
                int rows = (int)Math.Ceiling((double)craftingComponent.NumberOfSlots / craftingComponent.SlotsPerRow);
                int totalWidth = (int)(columns * craftingComponent.SlotSize.X + (columns + 1) * craftingComponent.Margin);
                int totalHeight = (int)(rows * craftingComponent.SlotSize.Y + (rows + 1) * craftingComponent.Margin);

                // Draw the background
                Milk.Graphics.FillRectangle(
                    new Rectangle((int)adjustedPosition.X, (int)adjustedPosition.Y, totalWidth, totalHeight),
                    primaryColor * alpha
                );

                // Draw each slot
                for (int i = 0; i<craftingComponent.NumberOfSlots; i++)
                {

                    // Calculate slot top-left position
                    Vector2 pos = new Vector2(
                        adjustedPosition.X + craftingComponent.Margin + ( (i % craftingComponent.SlotsPerRow) * (craftingComponent.SlotSize.X + craftingComponent.Margin)),
                        (int)(adjustedPosition.Y + craftingComponent.Margin + ( (craftingComponent.Margin + craftingComponent.SlotSize.Y) * (Math.Floor((double)(i / craftingComponent.SlotsPerRow)))))
                    );

                    // Calculate the size of the slot border, depending on
                    // whether it is the selected slot or not
                    Rectangle r;
                    if (i == craftingComponent.SelectedSlot)
                        r = new Rectangle(
                            (int)pos.X - craftingComponent.Margin / 2,
                            (int)pos.Y - craftingComponent.Margin / 2,
                            (int)craftingComponent.SlotSize.X + craftingComponent.Margin,
                            (int)craftingComponent.SlotSize.Y + craftingComponent.Margin
                        );
                    else
                        r = new Rectangle(
                            (int)pos.X - craftingComponent.Margin / 4,
                            (int)pos.Y - craftingComponent.Margin / 4,
                            (int)craftingComponent.SlotSize.X + craftingComponent.Margin / 2,
                            (int)craftingComponent.SlotSize.Y + craftingComponent.Margin / 2
                        );

                    // Draw the slot border
                    Milk.Graphics.DrawRectangle(
                        r,
                        secondaryColor * alpha,
                        i == craftingComponent.SelectedSlot ? craftingComponent.Margin / 2 : craftingComponent.Margin / 4
                    );

                    // Draw the texture of the entity in the slot
                    // (if the slot is being used)
                    if (
                        craftingComponent.inventory[i].Recipe != null &&
                        _textures.ContainsKey(craftingComponent.inventory[i].Recipe!.EntityTypeCreated)
                    )
                    {

                        bool canCraft = true;
                        InventoryComponent inventoryComponent = entity.GetComponent<InventoryComponent>()!;
                        foreach(var ingredient in craftingComponent.inventory[i].Recipe!.Ingredients)
                        {
                            if (inventoryComponent.NumberOfEntityType(ingredient.Key) < ingredient.Value)
                            {
                                canCraft = false;
                                break;
                            }
                        }

                        Milk.Graphics.FillRectangle(
                            (int)pos.X,
                            (int)pos.Y,
                            craftingComponent.SlotSize.X,
                            craftingComponent.SlotSize.Y,
                            canCraft == true ? Color.Green * alpha : Color.Red * alpha
                            //craftingComponent.CanBeCrafted(i) == true ? Color.Green * alpha : Color.Red * alpha
                        );

                        // Fit the texture to the available slot size
                        Utilities.DrawTextureToContainerSize(
                            _textures[craftingComponent.inventory[i].Recipe!.EntityTypeCreated],
                            new Rectangle(
                                (int)pos.X + craftingComponent.Margin, (int)pos.Y + craftingComponent.Margin,
                                (int)craftingComponent.SlotSize.X - + craftingComponent.Margin * 2,
                                (int)craftingComponent.SlotSize.Y - + craftingComponent.Margin * 2
                            ),
                            Color.White * alpha
                        );

                    }

                }

            }

        }

    }

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