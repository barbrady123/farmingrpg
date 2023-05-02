using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    void Start()
    {
        SwitchBoundingShape();
    }

    /// <summary>
    /// Switch the collider that cinemachine uses to define the edges of the screen
    /// </summary>
    private void SwitchBoundingShape()
    {
        var collider = GameObject.FindGameObjectWithTag(Global.Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();
        var confiner = GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = collider;

        // Have to clear cache if the confiner bounds change...
        confiner.InvalidatePathCache();
    }
}
