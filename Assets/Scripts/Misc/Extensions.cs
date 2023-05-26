using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> items) =>  items.Select((item, index) => (item, index));

    public static void SetAllToValue<T>(this IList<T> items, T value)
    {
        for (int x = 0; x < items.Count; x++)
        {
            items[x] = value;
        }
    }

    public static Color WithAlpha(this Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);

    public static Color AsTransparent(this Color color) => WithAlpha(color, 0f);

    public static Color AsOpaque(this Color color) => WithAlpha(color, 1f);

    public static void SetImageTransparent(this Image image) => image.color = image.color.AsTransparent();

    public static void SetImageOpaque(this Image image) => image.color = image.color.AsOpaque();

    public static void SetImageTransparent(this SpriteRenderer renderer) => renderer.color = renderer.color.AsTransparent();

    public static void SetImageOpaque(this SpriteRenderer renderer) => renderer.color = renderer.color.AsOpaque();

    public static Vector3Int ToVector3Int(this Vector2Int vector2) => new Vector3Int(vector2.x, vector2.y, 0);
}
