using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AStar))]
public class NPCManager : SingletonMonobehavior<NPCManager>
{
    [HideInInspector]
    public NPC[] _npcArray;

    private AStar _aStar;

    protected override void Awake()
    {
        base.Awake();

        _aStar = GetComponent<AStar>();

        // Get NPC gameobjects in scene
        _npcArray = FindObjectsOfType<NPC>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    private void AfterSceneLoad()
    {
        SetNPCsActiveStatus();
    }

    private void SetNPCsActiveStatus()
    {
        foreach (var npc in _npcArray)
        {
            var npcMovement = npc.GetComponent<NPCMovement>();
            npcMovement.SetNPCActiveInScene(npcMovement.NPCCurrentScene.ToString() == SceneManager.GetActiveScene().name);
        }
    }

    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        return _aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStepStack);
    }
}
