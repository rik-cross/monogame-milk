//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace milk.Core;

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

/// <summary>
/// In-game logger
/// </summary>
public static class Log
{
    private static List<LogItem> _logItems;
    private static SpriteFont _font;
    private static Color _color;
    private static Vector2 _position;
    private static double _duration;
    private static double _fadeOutTime;

    static Log()
    {
        _logItems = new List<LogItem>();
        _color = Color.White;
        _position = Vector2.Zero;
        _duration = 3;
        _fadeOutTime = 0.5;
    }

    internal static void Init(SpriteFont? font = null, Color? color = null, Vector2? position = null, float duration = 3.0f)
    {
        _font = font ?? EngineGlobals.game._engineResources.FontSmall;
        _color = color ?? Color.White;
        _position = position ?? new Vector2(20, Milk.Size.Y / 2);
        _duration = duration;
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
                    if (logItem.Lifetime >= _duration)
                        logItem.Finished = true;
                    else
                        logItem.Lifetime += gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                    logItem.Alpha -= (float)(gameTime.ElapsedGameTime.TotalSeconds / _fadeOutTime);
            }
        }

    }

    internal static void Draw()
    {
        // Ensure we don't crash if Draw is called before Init
        if (_font == null) _font = EngineGlobals.game._engineResources.FontSmall;

        float y = 0;
        foreach(LogItem logItem in _logItems)
        {
            Milk.Graphics.DrawString(
                _font,
                logItem.Text,
                _position + new Vector2(0, y),
                _color * logItem.Alpha
            );
            y += _font.MeasureString(logItem.Text).Y;
        }
    }
    
}