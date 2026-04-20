//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Core;

/// <summary>
/// In-game logger
/// </summary>
public static class Log
{
    private static List<LogItem> _logItems;

    /// <summary>
    /// Display font.
    /// </summary>
    public static SpriteFont Font;

    /// <summary>
    /// Text colour.
    /// </summary>
    public static Color Color;

    /// <summary>
    /// The (x, y) screen position of the top log item.
    /// </summary>
    public static Vector2 Position;

    /// <summary>
    /// The time (in seconds) to display each log item.
    /// </summary>
    public static double Duration;

    /// <summary>
    /// The time (in seconds) for log items to fade out.
    /// </summary>
    public static double FadeOutTime;

    static Log()
    {
        _logItems = new List<LogItem>();
        Color = Color.White;
        Position = Vector2.Zero;
        Duration = 3;
        FadeOutTime = 0.5;
    }

    internal static void Init(
        SpriteFont? font = null,
        Color? color = null,
        Vector2? position = null,
        float duration = 3.0f,
        float fadeOutTime = 0.5f
    )
    {
        Font = font ?? EngineGlobals.game._engineResources.FontSmall;
        Color = color ?? Color.White;
        Position = position ?? new Vector2(20, Milk.Size.Y / 2);
        Duration = duration;
        FadeOutTime = fadeOutTime;
    }

    /// <summary>
    /// Adds a log to the logger.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public static void Add(string message)
    {
        _logItems.Add(new LogItem(message));
    }

    internal static void Update(GameTime gameTime)
    {
        for(int i=_logItems.Count - 1; i>=0; i--)
        {

            LogItem logItem = _logItems[i];

            // Remove completed log items
            if (logItem.Finished == true && logItem.Alpha <= 0)
                _logItems.RemoveAt(i);
            else
            {
                if (logItem.Finished == false)
                {
                    if (logItem.Lifetime >= Duration)
                        logItem.Finished = true;
                    else
                        logItem.Lifetime += gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                    logItem.Alpha -= (float)(gameTime.ElapsedGameTime.TotalSeconds / FadeOutTime);
            }
        }

    }

    internal static void Draw()
    {
        // Ensure we don't crash if Draw is called before Init
        if (Font == null)
            Font = EngineGlobals.game._engineResources.FontSmall;

        float y = 0;
        foreach(LogItem logItem in _logItems)
        {
            Milk.Graphics.DrawString(
                Font,
                logItem.Text,
                Position + new Vector2(0, y),
                Color * logItem.Alpha
            );
            y += Font.MeasureString(logItem.Text).Y;
        }
    }
    
}