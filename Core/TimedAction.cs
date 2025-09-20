using System;
using Microsoft.Xna.Framework;

namespace MonoGameECS;

public class TimedAction
{
    public readonly float ElapsedTime = 0f;
    public readonly Action<GameTime> Action;
    public readonly string Name;
    public TimedAction(float elapsedTime, Action<GameTime> action, string? name = null)
    {
        this.ElapsedTime = elapsedTime;
        this.Action = action;
        this.Name = name ?? "";
    }
    public void Execute(GameTime gameTime)
    {
        Action(gameTime);
    }
}