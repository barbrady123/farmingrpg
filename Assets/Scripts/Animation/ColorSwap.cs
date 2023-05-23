using System;
using UnityEngine;

[Serializable]
public class ColorSwap
{
    public Color FromColor;

    public Color ToColor;

    public ColorSwap(Color fromColor, Color toColor)
    {
        this.FromColor = fromColor;
        this.ToColor = toColor;
    }
}
