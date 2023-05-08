using Unity.VisualScripting;
using UnityEngine;

public static class Settings
{
    // Tilemap - Grid cell size in unity units
    public const float GridCellSize = 1f;

    // Time System
    public const float SecondsPerGameSecond = 0.012f;

    // Obscured Item Fading
    public const float fadeInSeconds = 0.25f;

    public const float fadeOutSeconds = 0.35f;

    public const float targetAlpha = 0.45f;

    // Player Movement
    public const float runningSpeed = 5.333f;

    public const float walkingSpeed = 2.666f;

    // Inventory
    public static int PlayerInitialInventoryCapacity = 24;

    public static int PlayerMaximumInventoryCapacity = 48;

    #region Animation Parameters
    public static int xInput = Animator.StringToHash(nameof(xInput));

    public static int yInput = Animator.StringToHash(nameof(yInput));

    public static int isWalking = Animator.StringToHash(nameof(isWalking));

    public static int isRunning = Animator.StringToHash(nameof(isRunning));

    public static int toolEffect = Animator.StringToHash(nameof(toolEffect));

    public static int isUsingToolRight = Animator.StringToHash(nameof(isUsingToolRight));

    public static int isUsingToolLeft = Animator.StringToHash(nameof(isUsingToolLeft));

    public static int isUsingToolUp = Animator.StringToHash(nameof(isUsingToolUp));

    public static int isUsingToolDown = Animator.StringToHash(nameof(isUsingToolDown));

    public static int isLiftingToolRight = Animator.StringToHash(nameof(isLiftingToolRight));

    public static int isLiftingToolLeft = Animator.StringToHash(nameof(isLiftingToolLeft));

    public static int isLiftingToolUp = Animator.StringToHash(nameof(isLiftingToolUp));

    public static int isLiftingToolDown = Animator.StringToHash(nameof(isLiftingToolDown));

    public static int isSwingingToolRight = Animator.StringToHash(nameof(isSwingingToolRight));

    public static int isSwingingToolLeft = Animator.StringToHash(nameof(isSwingingToolLeft));

    public static int isSwingingToolUp = Animator.StringToHash(nameof(isSwingingToolUp));

    public static int isSwingingToolDown = Animator.StringToHash(nameof(isSwingingToolDown));

    public static int isPickingRight = Animator.StringToHash(nameof(isPickingRight));

    public static int isPickingLeft = Animator.StringToHash(nameof(isPickingLeft));

    public static int isPickingUp = Animator.StringToHash(nameof(isPickingUp));

    public static int isPickingDown = Animator.StringToHash(nameof(isPickingDown));

    // Shared Animation Parameters
    public static int idleRight = Animator.StringToHash(nameof(idleRight));

    public static int idleLeft = Animator.StringToHash(nameof(idleLeft));

    public static int idleUp = Animator.StringToHash(nameof(idleUp));

    public static int idleDown = Animator.StringToHash(nameof(idleDown));
    #endregion

    static Settings()
    {

    }
}
