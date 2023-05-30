using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class NPCPath : MonoBehaviour
{
    public Stack<NPCMovementStep> NPCMovementStepStack;

    private NPCMovement _npcMovement;

    private void Awake()
    {
        _npcMovement = GetComponent<NPCMovement>();
        this.NPCMovementStepStack = new Stack<NPCMovementStep>();
    }

    public void ClearPath()
    {
        this.NPCMovementStepStack.Clear();
    }

    public void BuildPath(NPCScheduleEvent npcScheduleEvent)
    {
        ClearPath();

        if (npcScheduleEvent.ToSceneName != _npcMovement.NPCCurrentScene)
            return;

        NPCManager.Instance.BuildPath(npcScheduleEvent.ToSceneName, (Vector2Int)_npcMovement.NPCCurrentGridPosition, (Vector2Int)npcScheduleEvent.ToGridCoordinate, this.NPCMovementStepStack);

        // If stack count > 1, update times and then pop off 1st item which is the starting position
        if (this.NPCMovementStepStack.Count > 1)
        {
            UpdateTimesOnPath();
            this.NPCMovementStepStack.Pop(); // skip starting step

            // Set schedule event details in NPC movement
            _npcMovement.SetScheduleEventDetails(npcScheduleEvent);
        }
    }

    private void UpdateTimesOnPath()
    {
        var currentGameTime = TimeManager.Instance.GetGameTime();

        NPCMovementStep previousStep = null;

        foreach (var step in this.NPCMovementStepStack)
        {
            if (previousStep == null)
                previousStep = step;

            step.Hour = currentGameTime.Hours;
            step.Minute = currentGameTime.Minutes;
            step.Second = currentGameTime.Seconds;

            float stepSize = MovementIsDiagonal(step, previousStep) ? Settings.GridCellSizeDiagonal : Settings.GridCellSize;

            var movementTimeStep = new TimeSpan(0,0, (int)(stepSize / Settings.SecondsPerGameSecond / _npcMovement.NPCNormalSpeed));

            currentGameTime = currentGameTime.Add(movementTimeStep);

            previousStep = step;
        }
    }

    private bool MovementIsDiagonal(NPCMovementStep step, NPCMovementStep previousStep)
    {
        return (step.GridCoordinate.x != previousStep.GridCoordinate.x) && (step.GridCoordinate.y != previousStep.GridCoordinate.y);
    }
}
