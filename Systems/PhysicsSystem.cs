//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

// NOTE: This PhysicsSystem is mostly AI-generated

using Microsoft.Xna.Framework;
using milk.Components;

namespace milk.Core;

/// <summary>
/// The PhysicsSystem handles collision detection.
/// </summary>
public class PhysicsSystem : System
{

    /// <summary>
    /// Checks and resolves collisions.
    /// </summary>
    /// <param name="gameTime">Elapsed game time.</param>
    /// <param name="scene">The scene containing the system.</param>
    public override void Update(GameTime gameTime, Scene scene)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Follows the Milk pattern of filtering entities with required components
        var entities = scene.entities.Where(e => e.HasComponent<TransformComponent>() && e.HasComponent<PhysicsComponent>());

        foreach (var entity in entities)
        {
            var transform = entity.GetComponent<TransformComponent>();
            var physics = entity.GetComponent<PhysicsComponent>();

            // 1. Incorporate existing Milk features: Scene-wide forces
            // Note: Milk's Scene properties are added to local physics
            physics.Velocity += (physics.Acceleration + scene.Acceleration) * deltaTime;
            Vector2 currentVelocity = physics.Velocity + scene.Velocity;

            // 2. Collision Resolution Logic
            if (entity.HasComponent<ColliderComponent>())
            {
                ResolveCollisions(scene, entity, currentVelocity, deltaTime);
            }
            else
            {
                // Default behavior from the original Milk shell
                transform.Position += currentVelocity * deltaTime;
            }
        }
    }


    private void ResolveCollisions(Scene scene, Entity entity, Vector2 velocity, float deltaTime)
    {
        var transform = entity.GetComponent<TransformComponent>();
        var physics = entity.GetComponent<PhysicsComponent>();
        var collider = entity.GetComponent<ColliderComponent>();

        Vector2 moveAmount = velocity * deltaTime;
        float remainingTime = 1.0f;
        int maxIterations = 4; 

        while (remainingTime > 0 && maxIterations > 0)
        {
            float hitTime = 1.0f;
            Vector2 hitNormal = Vector2.Zero;
            bool hitAnything = false;

            MilkRectangleF playerBox = GetAbsoluteCollider(transform, collider);

            // 1. Check Scene Colliders (Static list in Scene.cs)
            // Note: Using 'sceneColliders' as defined in your uploaded Scene.cs
            if (scene.sceneColliders != null)
            {
                foreach (var sc in scene.sceneColliders)
                {
                    MilkRectangleF obsBox = new MilkRectangleF(sc.X, sc.Y, sc.Width, sc.Height);
                    float t = SweptAABB(playerBox, obsBox, moveAmount, out Vector2 normal);

                    if (t < hitTime)
                    {
                        hitTime = t;
                        hitNormal = normal;
                        hitAnything = true;
                    }
                }
            }

            // 2. Check Entity Colliders (Dynamic entities in scene.entities)
            foreach (var obstacle in scene.entities.Where(e => e != entity && e.HasComponent<ColliderComponent>()))
            {
                var obsTransform = obstacle.GetComponent<TransformComponent>();
                var obsCollider = obstacle.GetComponent<ColliderComponent>();
                MilkRectangleF obsBox = GetAbsoluteCollider(obsTransform, obsCollider);

                float t = SweptAABB(playerBox, obsBox, moveAmount, out Vector2 normal);

                if (t < hitTime)
                {
                    hitTime = t;
                    hitNormal = normal;
                    hitAnything = true;
                }
            }

            // 3. Resolution
            float epsilon = 0.001f;
            transform.Position += moveAmount * Math.Max(0, hitTime - epsilon);
            remainingTime -= hitTime;

            if (hitAnything)
            {
                // Apply sliding: project velocity onto the plane of the normal
                float dot = (physics.Velocity.X * hitNormal.X + physics.Velocity.Y * hitNormal.Y);
                physics.Velocity -= hitNormal * dot;
                
                // Re-calculate remaining move amount based on new sliding velocity
                moveAmount = physics.Velocity * deltaTime * remainingTime;
            }
            else
            {
                // No more collisions this frame
                break;
            }
            
            maxIterations--;
        }
    }

    private float SweptAABB(MilkRectangleF b1, MilkRectangleF b2, Vector2 v, out Vector2 normal)
    {
        normal = Vector2.Zero;

        // 1. Static check: If we aren't moving on an axis, 
        // we MUST be overlapping on that axis for a collision to be possible.
        bool xOverlap = (b1.X < b2.X + b2.Width) && (b1.X + b1.Width > b2.X);
        bool yOverlap = (b1.Y < b2.Y + b2.Height) && (b1.Y + b1.Height > b2.Y);

        if (v.X == 0 && !xOverlap) return 1.0f;
        if (v.Y == 0 && !yOverlap) return 1.0f;

        // 2. Calculate entry/exit times
        float xInvEntry = v.X > 0 ? b2.X - (b1.X + b1.Width) : (b2.X + b2.Width) - b1.X;
        float xInvExit = v.X > 0 ? (b2.X + b2.Width) - b1.X : b2.X - (b1.X + b1.Width);
        float yInvEntry = v.Y > 0 ? b2.Y - (b1.Y + b1.Height) : (b2.Y + b2.Height) - b1.Y;
        float yInvExit = v.Y > 0 ? (b2.Y + b2.Height) - b1.Y : b2.Y - (b1.Y + b1.Height);

        float xEntry = v.X == 0 ? float.NegativeInfinity : xInvEntry / v.X;
        float xExit = v.X == 0 ? float.PositiveInfinity : xInvExit / v.X;
        float yEntry = v.Y == 0 ? float.NegativeInfinity : yInvEntry / v.Y;
        float yExit = v.Y == 0 ? float.PositiveInfinity : yInvExit / v.Y;

        // 3. The "Minkowski" Entry/Exit
        float entryTime = Math.Max(xEntry, yEntry);
        float exitTime = Math.Min(xExit, yExit);

        // 4. Collision Conditions
        // - entryTime > exitTime: The rectangles miss each other
        // - xExit < 0 or yExit < 0: The rectangles are moving away from each other
        // - xEntry > 1 or yEntry > 1: No collision in this frame
        if (entryTime > exitTime || xExit < 0 || yExit < 0 || xEntry > 1 || yEntry > 1)
        {
            return 1.0f;
        }

        // 5. Determine Normal
        if (xEntry > yEntry)
        {
            normal = v.X < 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
        }
        else
        {
            normal = v.Y < 0 ? new Vector2(0, 1) : new Vector2(0, -1);
        }

        return entryTime;
    }


    private MilkRectangleF GetAbsoluteCollider(TransformComponent t, ColliderComponent c)
        => new MilkRectangleF(t.Position.X + c.Offset.X, t.Position.Y + c.Offset.Y, c.Size.X, c.Size.Y);
}

// Aligned with Milk's simple structure
internal struct MilkRectangleF 
{
    public float X, Y, Width, Height;
    public MilkRectangleF(float x, float y, float w, float h) { X = x; Y = y; Width = w; Height = h; }
}