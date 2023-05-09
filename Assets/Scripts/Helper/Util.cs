using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static IEnumerable<T> GetComponentsAtBoxLocation<T>(Vector2 point, Vector2 size, float angle) where T : MonoBehaviour
    {
        foreach (var collider in Physics2D.OverlapBoxAll(point, size, angle))
        {
            var component = collider.gameObject.GetComponentInParent<T>() ?? collider.gameObject.GetComponentInChildren<T>();
            if (component != null)
            {
                yield return component;
            }
        }
    }
}
