using Microsoft.Xna.Framework;
using milk.UI;

namespace mill.Core;

/// <summary>
/// Each scene has an animator, which is responsible for processing tweens.
/// </summary>
public class Animator
{
    
    private List<Tween> tweenList;

    internal Animator()
    {
        tweenList = new List<Tween>();        
    }

    /// <summary>
    /// Adds a tween to the animator.
    /// </summary>
    /// <param name="tween"></param>
    public void AddTween(Tween tween)
    {
        tweenList.Add(tween);
    }

    internal void Update(GameTime gameTime)
    {
        //Console.WriteLine("updating");
        for (int i = tweenList.Count - 1; i >= 0; i--)
        {
            //Console.WriteLine("found a tween");
            Tween tween = tweenList[i];
            tween.Update(gameTime);
            if (tween.Finished == true)
            {
                tweenList.RemoveAt(i);
            }
        }
    }

}