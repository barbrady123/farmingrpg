using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void SceneLoaded()
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
