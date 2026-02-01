//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using milk.UI;

namespace milk.Core;

/// <summary>
/// Each scene has an animator, which is responsible for processing tweens.
/// </summary>
internal class Animator
{
    
    // A list containing all active tweens
    private List<Tween> _tweenList;

    internal Animator()
    {
        _tweenList = new List<Tween>();        
    }

    /// <summary>
    /// Adds a tween to the animator.
    /// </summary>
    /// <param name="tween">The tween object to add.</param>
    public void AddTween(Tween tween)
    {
        _tweenList.Add(tween);
    }

    internal void Update(GameTime gameTime)
    {

        // Process each tween in the list
        for (int i = _tweenList.Count - 1; i >= 0; i--)
        {
            Tween tween = _tweenList[i];
            // Update the tween
            tween.Update(gameTime);
            // Remove the tween if finished
            if (tween.Finished == true)
                _tweenList.RemoveAt(i);
        }

    }

}