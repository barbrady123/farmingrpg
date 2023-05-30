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

    public static (bool isRight, bool isLeft, bool isUp, bool IsDown) DirectionToFlag(Direction direction)
    {
        if (direction == Direction.Right)
            return (true, false, false, false);
        else if (direction == Direction.Left)
            return (false, true, false, false);
        else if (direction == Direction.Up)
            return (false, false, true, false);
        else if (direction == Direction.Down)
            return (false, false, false, true);
        return (false, false, false, false);
    }

    public static (bool isRight, bool isLeft, bool isUp, bool IsDown) Vector3IntDirectionToFlag(Vector3Int direction)
    {
        if (direction == Vector3Int.right)
            return (true, false, false, false);
        else if (direction == Vector3Int.left)
            return (false, true, false, false);
        else if (direction == Vector3Int.up)
            return (false, false, true, false);
        else if (direction == Vector3Int.down)
            return (false, false, false, true);
        return (false, false, false, false);
    }
}
