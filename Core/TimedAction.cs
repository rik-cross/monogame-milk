using System;
using Microsoft.Xna.Framework;

namespace milk;

public class TimedAction
{
    public readonly float ElapsedTime = 0f;
    public readonly Action Action;
    public readonly string Name;
    public TimedAction(float elapsedTime, Action action, string? name = null)
    {
        this.ElapsedTime = elapsedTime;
        this.Action = action;
        this.Name = name ?? "";
    }
    public void Execute()
    {
        Action();
    }
}