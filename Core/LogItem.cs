//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Core;

/// <summary>
/// A LogItem is one item displayed in the Log.
/// </summary>
internal class LogItem
{
    internal string Text { get; private set; }
    internal double Lifetime { get; set; }
    internal bool Finished { get; set; }
    internal float Alpha { get; set; }
    internal LogItem(string message)
    {
        Text = message;
        Lifetime = 0;
        Finished = false;
        Alpha = 1;
    }
}