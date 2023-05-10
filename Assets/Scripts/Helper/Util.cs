using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static IEnumerable<T> GetComponentsAtCursorLocation<T>(Vector3 positionToCheck) where T : MonoBehaviour
    {
        var collider2DArray = Physics2D.OverlapPointAll(positionToCheck);

        foreach (var collider in collider2DArray)
        {
            var component = collider.gameObject.GetComponentInParent<T>() ?? collider.gameObject.GetComponentInChildren<T>();
            if (component != null)
            {
                yield return component;
            }
        }
    }

    public static IEnumerable<T> GetComponentsAtBoxLocationNonAlloc<T>(int numberOfCollidersToTest, Vector2 point, Vector2 size, float angle) where T : MonoBehaviour
    {
        var colliders = new Collider2D[numberOfCollidersToTest];

        Physics2D.OverlapBoxNonAlloc(point, size, angle, colliders);

        foreach (var collider in colliders)
        {
            var component = collider?.gameObject.GetComponent<T>();
            if (component != null)
            {
                yield return component;
            }
        }
    }

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
