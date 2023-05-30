using UnityEngine;

[RequireComponent(typeof(AStar))]
public class AStarTest : MonoBehaviour
{
    [SerializeField]
    private NPCPath _npcPath = null;

    [SerializeField]
    private bool _moveNPC = false;

    [SerializeField]
    private Vector2Int _finishPosition;

    [SerializeField]
    private AnimationClip _idleDownAnimationClip = null;

    [SerializeField]
    private AnimationClip _eventAnimationClip = null;

    private NPCMovement _npcMovement;

    private void Start()
    {
        _npcMovement = _npcPath.GetComponent<NPCMovement>();
        _npcMovement.NPCFacingDirectionAtDestination = Direction.Down;
        _npcMovement.NPCTargetAnimationClip = _idleDownAnimationClip;
    }

    private void Update()
    {
        if (!_moveNPC)
            return;

        _moveNPC = false;

        var npcScheduleEvent = new NPCScheduleEvent(0, 0, 0, 0, Weather.None, Season.Spring, SceneName.Scene1_Farm, new GridCoordinate(_finishPosition.x, _finishPosition.y), _eventAnimationClip);

        _npcPath.BuildPath(npcScheduleEvent);
    }
}
