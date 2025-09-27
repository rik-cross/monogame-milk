using System;

namespace milk;

public static class EasingFunctions
{
    /// <summary>
    /// Represents a delegate for an easing function.
    /// An easing function takes a single float parameter (typically a normalized time value from 0 to 1)
    /// and returns a float value, which is the eased output.
    /// </summary>
    public delegate float EasingDelegate(float t);

    /// <summary>
    /// No easing, no acceleration.
    /// </summary>
    public static float Linear(float t)
    {
        return t;
    }

    /// <summary>
    /// Easing in (accelerating from zero velocity).
    /// </summary>
    public static float SineEaseIn(float t)
    {
        return 1f - (float)Math.Cos(t * Math.PI / 2f);
    }

    /// <summary>
    /// Easing out (decelerating to zero velocity).
    /// </summary>
    public static float SineEaseOut(float t)
    {
        return (float)Math.Sin(t * Math.PI / 2f);
    }

    /// <summary>
    /// Easing in and out (accelerating until halfway, then decelerating).
    /// </summary>
    public static float SineEaseInOut(float t)
    {
        return -0.5f * ((float)Math.Cos(Math.PI * t) - 1f);
    }

    /// <summary>
    /// Easing in (accelerating from zero velocity).
    /// </summary>
    public static float QuadEaseIn(float t)
    {
        return t * t;
    }

    /// <summary>
    /// Easing out (decelerating to zero velocity).
    /// </summary>
    public static float QuadEaseOut(float t)
    {
        return t * (2f - t);
    }

    /// <summary>
    /// Easing in and out (accelerating until halfway, then decelerating).
    /// </summary>
    public static float QuadEaseInOut(float t)
    {
        t *= 2f;
        if (t < 1f) return 0.5f * t * t;
        t--;
        return -0.5f * (t * (t - 2f) - 1f);
    }

    /// <summary>
    /// Easing in (accelerating from zero velocity).
    /// </summary>
    public static float CubicEaseIn(float t)
    {
        return t * t * t;
    }

    /// <summary>
    /// Easing out (decelerating to zero velocity).
    /// </summary>
    public static float CubicEaseOut(float t)
    {
        t--;
        return t * t * t + 1f;
    }

    /// <summary>
    /// Easing in and out (accelerating until halfway, then decelerating).
    /// </summary>
    public static float CubicEaseInOut(float t)
    {
        t *= 2f;
        if (t < 1f) return 0.5f * t * t * t;
        t -= 2f;
        return 0.5f * (t * t * t + 2f);
    }

    /// <summary>
    /// Easing in (accelerating from zero velocity).
    /// </summary>
    public static float QuartEaseIn(float t)
    {
        return t * t * t * t;
    }

    /// <summary>
    /// Easing out (decelerating to zero velocity).
    /// </summary>
    public static float QuartEaseOut(float t)
    {
        t--;
        return -(t * t * t * t - 1f);
    }

    /// <summary>
    /// Easing in and out (accelerating until halfway, then decelerating).
    /// </summary>
    public static float QuartEaseInOut(float t)
    {
        t *= 2f;
        if (t < 1f) return 0.5f * t * t * t * t;
        t -= 2f;
        return -0.5f * (t * t * t * t - 2f);
    }

    /// <summary>
    /// Easing in (accelerating from zero velocity).
    /// </summary>
    public static float QuintEaseIn(float t)
    {
        return t * t * t * t * t;
    }

    /// <summary>
    /// Easing out (decelerating to zero velocity).
    /// </summary>
    public static float QuintEaseOut(float t)
    {
        t--;
        return (t * t * t * t * t + 1f);
    }

    /// <summary>
    /// Easing in and out (accelerating until halfway, then decelerating).
    /// </summary>
    public static float QuintEaseInOut(float t)
    {
        t *= 2f;
        if (t < 1f) return 0.5f * t * t * t * t * t;
        t -= 2f;
        return 0.5f * (t * t * t * t * t + 2f);
    }

    /// <summary>
    /// Easing in (accelerating from zero velocity).
    /// </summary>
    public static float ExpoEaseIn(float t)
    {
        return (t == 0f) ? 0f : (float)Math.Pow(2f, 10f * (t - 1f));
    }

    /// <summary>
    /// Easing out (decelerating to zero velocity).
    /// </summary>
    public static float ExpoEaseOut(float t)
    {
        return (t == 1f) ? 1f : -(float)Math.Pow(2f, -10f * t) + 1f;
    }

    /// <summary>
    /// Easing in and out (accelerating until halfway, then decelerating).
    /// </summary>
    public static float ExpoEaseInOut(float t)
    {
        if (t == 0f) return 0f;
        if (t == 1f) return 1f;
        t *= 2f;
        if (t < 1f) return 0.5f * (float)Math.Pow(2f, 10f * (t - 1f));
        t--;
        return 0.5f * (-(float)Math.Pow(2f, -10f * t) + 2f);
    }

    /// <summary>
    /// Easing in (accelerating from zero velocity).
    /// </summary>
    public static float CircEaseIn(float t)
    {
        return -(float)Math.Sqrt(1f - t * t) + 1f;
    }

    /// <summary>
    /// Easing out (decelerating to zero velocity).
    /// </summary>
    public static float CircEaseOut(float t)
    {
        t--;
        return (float)Math.Sqrt(1f - t * t);
    }

    /// <summary>
    /// Easing in and out (accelerating until halfway, then decelerating).
    /// </summary>
    public static float CircEaseInOut(float t)
    {
        t *= 2f;
        if (t < 1f) return -0.5f * ((float)Math.Sqrt(1f - t * t) - 1f);
        t -= 2f;
        return 0.5f * ((float)Math.Sqrt(1f - t * t) + 1f);
    }

    /// <summary>
    /// Easing in (pulls back slightly before accelerating).
    /// </summary>
    public static float BackEaseIn(float t)
    {
        const float s = 1.70158f;
        return t * t * ((s + 1f) * t - s);
    }

    /// <summary>
    /// Easing out (overshoots the target slightly before settling).
    /// </summary>
    public static float BackEaseOut(float t)
    {
        const float s = 1.70158f;
        t--;
        return t * t * ((s + 1f) * t + s) + 1f;
    }

    /// <summary>
    /// Easing in and out (pulls back at start, overshoots at end).
    /// </summary>
    public static float BackEaseInOut(float t)
    {
        const float s = 1.70158f * 1.525f;
        t *= 2f;
        if (t < 1f) return 0.5f * (t * t * ((s + 1f) * t - s));
        t -= 2f;
        return 0.5f * (t * t * ((s + 1f) * t + s) + 2f);
    }

    /// <summary>
    /// Easing in (bounces in towards the target).
    /// </summary>
    public static float ElasticEaseIn(float t)
    {
        if (t == 0f) return 0f;
        if (t == 1f) return 1f;
        t--;
        const float p = 0.3f;
        return -(float)Math.Pow(2f, 10f * t) * (float)Math.Sin((t - p / 4f) * (2f * Math.PI) / p);
    }

    /// <summary>
    /// Easing out (bounces out from the target).
    /// </summary>
    public static float ElasticEaseOut(float t)
    {
        if (t == 0f) return 0f;
        if (t == 1f) return 1f;
        const float p = 0.3f;
        return (float)Math.Pow(2f, -10f * t) * (float)Math.Sin((t - p / 4f) * (2f * Math.PI) / p) + 1f;
    }

    /// <summary>
    /// Easing in and out (bounces in then out).
    /// </summary>
    public static float ElasticEaseInOut(float t)
    {
        if (t == 0f) return 0f;
        if (t == 1f) return 1f;
        t *= 2f;
        const float p = 0.45f; // This value is different from in/out for smoother transition
        float s = p / (2f * (float)Math.PI) * (float)Math.Asin(1f);

        if (t < 1f)
        {
            t -= 1f;
            return -0.5f * ((float)Math.Pow(2f, 10f * t) * (float)Math.Sin((t - s) * (2f * Math.PI) / p));
        }
        else
        {
            t -= 1f;
            return 0.5f * ((float)Math.Pow(2f, -10f * t) * (float)Math.Sin((t - s) * (2f * Math.PI) / p)) + 1f;
        }
    }

    /// <summary>
    /// Easing in (bounces as it approaches the start).
    /// </summary>
    public static float BounceEaseIn(float t)
    {
        return 1f - BounceEaseOut(1f - t);
    }

    /// <summary>
    /// Easing out (bounces as it reaches the end).
    /// </summary>
    public static float BounceEaseOut(float t)
    {
        if (t < (1f / 2.75f))
        {
            return (7.5625f * t * t);
        }
        else if (t < (2f / 2.75f))
        {
            t -= (1.5f / 2.75f);
            return (7.5625f * t * t + 0.75f);
        }
        else if (t < (2.5f / 2.75f))
        {
            t -= (2.25f / 2.75f);
            return (7.5625f * t * t + 0.9375f);
        }
        else
        {
            t -= (2.625f / 2.75f);
            return (7.5625f * t * t + 0.984375f);
        }
    }

    /// <summary>
    /// Easing in and out (bounces at start and end).
    /// </summary>
    public static float BounceEaseInOut(float t)
    {
        if (t < 0.5f)
        {
            return BounceEaseIn(t * 2f) * 0.5f;
        }
        else
        {
            return BounceEaseOut(t * 2f - 1f) * 0.5f + 0.5f;
        }
    }
}