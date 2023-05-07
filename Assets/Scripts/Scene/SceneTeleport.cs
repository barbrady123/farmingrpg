using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField]
    private SceneName _sceneNameGoto = SceneName.Scene1_Farm;

    [SerializeField]
    private Vector3 _scenePositionGoto = new Vector3();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player == null)
            return;

        float xPosition = Mathf.Approximately(_scenePositionGoto.x, 0f) ? player.transform.position.x : _scenePositionGoto.x;
        float yPosition = Mathf.Approximately(_scenePositionGoto.y, 0f) ? player.transform.position.y : _scenePositionGoto.y;

        SceneControllerManager.Instance.FadeAndLoadScene(_sceneNameGoto.ToString(), new Vector3(xPosition, yPosition, 0f));
    }
}
