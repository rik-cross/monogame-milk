using Microsoft.Xna.Framework;
using MonoGame.Extended;
using milk.Core;
using milk.Components;
namespace milk.Systems;

public class PhysicsSystem : milk.Core.System
{
    public override void Init()
    {
        AddRequiredComponentType<TransformComponent>();
        AddRequiredComponentType<PhysicsComponent>();
        AddRequiredComponentType<ColliderComponent>();
    }

    public override void UpdateEntity(GameTime gameTime, Scene scene, Entity entity)
    {
        // A constant is often necessary when implementing acceleration to prevent infinite speed growth.
        // If your PhysicsComponent has a MaxSpeed property, use that instead (e.g., physicsComponent.MaxSpeed).
        const float MAX_INHERENT_SPEED = 5000f; 
        
        // Get the time elapsed since the last frame update for frame-rate independence (t).
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // 1. Retrieve components
        TransformComponent transformComponent = entity.GetComponent<TransformComponent>();
        PhysicsComponent physicsComponent = entity.GetComponent<PhysicsComponent>();
        
        // --- ACCELERATION: Update Persistent Velocity (v = v0 + a * t) ---

        // Calculate the total acceleration applied to the entity for this frame.
        // Assuming PhysicsComponent has an Acceleration property (Vector2).
        Vector2 totalAcceleration = physicsComponent.Acceleration + scene.Acceleration;

        // Apply acceleration to the entity's persistent velocity.
        physicsComponent.Velocity += totalAcceleration * deltaTime;

        // --- CLAMP 1: Persistent Velocity Cap (Required after acceleration) ---
        // Prevent the entity's base velocity from exceeding a practical maximum speed.
        if (physicsComponent.Velocity.Length() > MAX_INHERENT_SPEED)
        {
            physicsComponent.Velocity.Normalize();
            physicsComponent.Velocity *= MAX_INHERENT_SPEED;
        }

        // --- MOVEMENT: Calculate and Clamp Effective Velocity for this Frame ---

        // 2. Create the temporary effective velocity for this frame.
        // This combines the entity's UPDATED persistent velocity and the scene's velocity.
        Vector2 effectiveVelocity = physicsComponent.Velocity + scene.Velocity;

        // --- CLAMP 2: Normalization/Clamping by Max Component Magnitude (Your original logic) ---

        float currentLength = effectiveVelocity.Length();

        // Use float.Epsilon to check if there is any significant velocity
        if (currentLength > float.Epsilon) 
        {
            // 1. Define the "max speed" as the magnitude of the largest component of the effective velocity
            float maxComponentMagnitude = Math.Max(
                Math.Abs(effectiveVelocity.X),
                Math.Abs(effectiveVelocity.Y)
            );

            // 2. Clamp the effective velocity: Scale down the vector if its length is greater than the target magnitude.
            if (currentLength > maxComponentMagnitude) 
            {
                float scaleFactor = maxComponentMagnitude / currentLength;
                
                // Apply the scaling (clamping) to the effective velocity
                effectiveVelocity *= scaleFactor;
            }
        } 
        else 
        {
            // If the effective velocity is near zero, set it exactly to zero
            effectiveVelocity = Vector2.Zero;
        }

        // --- Apply Effective Velocity to Position (x = x0 + v * t) ---

        // CORRECTED: Apply the final clamped effective velocity to the position, scaled by deltaTime.
        transformComponent.Position.X += effectiveVelocity.X * deltaTime;
        transformComponent.Position.Y += effectiveVelocity.Y * deltaTime;
    }


    public override void Draw(Scene scene)
    {
    }
    public override void DrawEntity(Scene scene, Entity entity)
    {
        TransformComponent transformComponent = entity.GetComponent<TransformComponent>();
        ColliderComponent colliderComponent = entity.GetComponent<ColliderComponent>();

        if (scene.game.Debug == true)
        {

            spriteBatch.DrawRectangle(
                new Rectangle(
                    (int)transformComponent.X,
                    (int)transformComponent.Y,
                    (int)transformComponent.Width,
                    (int)transformComponent.Height
                ),
                Color.Green,
                1.0f
            );

            spriteBatch.DrawRectangle(
                new Rectangle(
                    (int)transformComponent.X + (int)colliderComponent.offset.X,
                    (int)transformComponent.Y + (int)colliderComponent.offset.Y,
                    (int)colliderComponent.size.X,
                    (int)colliderComponent.size.Y
                ),
                Color.Blue,
                1.0f
            );
        }

    }
}