using System.Collections;
using UnityEngine;

public class Player : SingletonMonobehavior<Player>
{
    #region Movement Parameters
    private float _xInput;

    private float _yInput;

    private bool _isWalking;

    private bool _isRunning;

    private bool _isIdle;

    private bool _isCarrying = false;

    private bool _isUsingToolRight;

    private bool _isUsingToolLeft;

    private bool _isUsingToolUp;

    private bool _isUsingToolDown;

    private bool _isLiftingToolRight;

    private bool _isLiftingToolLeft;

    private bool _isLiftingToolUp;

    private bool _isLiftingToolDown;

    private bool _isPickingRight;

    private bool _isPickingLeft;

    private bool _isPickingUp;

    private bool _isPickingDown;

    private bool _isSwingingToolRight;

    private bool _isSwingingToolLeft;

    private bool _isSwingingToolUp;

    private bool _isSwingingToolDown;

    private bool _idleRight;

    private bool _idleLeft;

    private bool _idleUp;

    private bool _idleDown;

    private ToolEffect _toolEffect = ToolEffect.None;
    #endregion

    private WaitForSeconds _useToolAnimationPause = new WaitForSeconds(Settings.UseToolAnimationPause);

    private WaitForSeconds _afterUseToolAnimationPause = new WaitForSeconds(Settings.AfterUseToolAnimationPause);

    private WaitForSeconds _liftToolAnimationPause = new WaitForSeconds(Settings.LiftToolAnimationPause);

    private WaitForSeconds _afterLiftToolAnimationPause = new WaitForSeconds(Settings.AfterLiftToolAnimationPause);

    private bool _playerToolUseDisabled = false;

    private GridCursor _gridCursor;

    private Rigidbody2D _rigidBody;

    private Direction _playerDirection;

    private float _movementSpeed;

    private bool _playerInputIsDisabled = false;

    private Camera _camera;

    private AnimationOverrides _animationOverrides;

    [SerializeField]
    private SpriteRenderer _equippedItemSpriteRenderer = null;

    // Player attributes that can be swapped
    public bool PlayerInputIsDisabled { get => _playerInputIsDisabled; set => _playerInputIsDisabled = value; }

    public Vector3 GetViewportPosition() => _camera.WorldToViewportPoint(transform.position);

    public void SetPlayerInputDisabled(bool isDisabled = true, bool resetMovementOnDisabled = true)
    {
        this.PlayerInputIsDisabled = isDisabled;

        if (isDisabled && resetMovementOnDisabled)
        {
            ResetMovement();
        }
    }

    public void ClearCarriedItem()
    {
        _equippedItemSpriteRenderer.sprite = null;
        _equippedItemSpriteRenderer.SetImageTransparent();

        // Apply base character arms customization
        _animationOverrides.ApplyCharacterCustomizationParameters(
            new[] { new CharacterAttribute(CharacterPartAnimator.Arms) });

        _isCarrying = false;
    }

    public void ShowCarriedItem(int itemCode)
    {
        var itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        if (itemDetails == null)
            return;

        _equippedItemSpriteRenderer.sprite = itemDetails.ItemSprite;
        _equippedItemSpriteRenderer.SetImageOpaque();

        // Apply "Carry" character arms customization
        _animationOverrides.ApplyCharacterCustomizationParameters(
            new[] { new CharacterAttribute(CharacterPartAnimator.Arms, partVariantType: PartVariantType.Carry) });

        _isCarrying = true;
    }

    private void ResetMovement()
    {
        _xInput = 0f;
        _yInput = 0f;
        _isRunning = false;
        _isWalking = false;
        _isIdle = true;
        FireMovementEvent();
    }

    protected override void Awake()
    {
        base.Awake();

        _rigidBody = GetComponent<Rigidbody2D>();
        _camera = Camera.main;

        _animationOverrides = GetComponentInChildren<AnimationOverrides>();
    }

    private void Start()
    {
        _gridCursor = FindObjectOfType<GridCursor>();
    }

    private void Update()
    {
        if (this.PlayerInputIsDisabled)
            return;

        // TEMP:
        PlayerTestInput();

        #region Player Input
        ResetAnimationTriggers();
        PlayerMovementInput();
        PlayerWalkInput();
        PlayerClickInput();
        FireMovementEvent();
        #endregion
    }

    // TEMP:
    private void PlayerTestInput()
    {
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }

        if (Input.GetKey(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), transform.position);
        }
    }

    private void FixedUpdate()
    {
        var move = new Vector2(
            _xInput * _movementSpeed * Time.deltaTime,
            _yInput * _movementSpeed * Time.deltaTime);

        // Should use velocity here instead?  Usually plays better with physics engine...?
        _rigidBody.MovePosition(_rigidBody.position + move);
    }

    private void ResetAnimationTriggers()
    {
        _isPickingRight = false;
        _isPickingLeft = false;
        _isPickingUp = false;
        _isPickingDown = false;
        _isUsingToolRight = false;
        _isUsingToolLeft = false;
        _isUsingToolUp = false;
        _isUsingToolDown = false;
        _isLiftingToolRight = false;
        _isLiftingToolLeft = false;
        _isLiftingToolUp = false;
        _isLiftingToolDown = false;
        _isSwingingToolRight = false;
        _isSwingingToolLeft = false;
        _isSwingingToolUp = false;
        _isSwingingToolDown = false;
        _toolEffect = ToolEffect.None;
    }

    private void PlayerMovementInput()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");

        if (_xInput != 0 && _yInput != 0)
        {
            _xInput *= 0.71f;
            _yInput *= 0.71f;
        }

        if (_xInput != 0 || _yInput != 0)
        {
            _isRunning = true;
            _isWalking = false;
            _isIdle = false;
            _movementSpeed = Settings.RunningSpeed;

            // Capture player direction for save game
            _playerDirection = Conversions.DirectionFromAxisInput(_xInput, _yInput);
        }
        else
        {
            _isRunning = false;
            _isWalking = false;
            _isIdle = true;
        }
    }

    // Some of these settings don't make any sense for just checking Shift...
    private void PlayerWalkInput()
    {
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        _isRunning = !shiftPressed;
        _isWalking = shiftPressed;
        _isIdle = false;
        _movementSpeed = shiftPressed ? Settings.WalkingSpeed : Settings.RunningSpeed;
    }

    private void PlayerClickInput()
    {
        if (_playerToolUseDisabled)
            return;

        if (Input.GetMouseButtonDown(Global.Inputs.MouseButtons.Left))
        {
            if (_gridCursor.CursorIsEnabled)
            {
                ProcessPlayerClickInput();
            }
        }
    }

    private void ProcessPlayerClickInput()
    {
        ResetMovement();

        var itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.Player);
        if (itemDetails == null)
            return;

        switch (itemDetails.ItemType)
        {
            case ItemType.Seed:
                ProcessPlayerClickInputSeed(itemDetails);
                break;
            case ItemType.Commodity:
                ProcessPlayerClickInputCommondity(itemDetails);
                break;
            // case other tools also following same path?
            case ItemType.HoeingTool:
            case ItemType.WateringTool:
                ProcessPlayerClickInputTool(itemDetails);
                break;
        }
    }

    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
            return Vector3Int.right;

        if (cursorGridPosition.x < playerGridPosition.x)
            return Vector3Int.left;

        if (cursorGridPosition.y > playerGridPosition.y)
            return Vector3Int.up;

        return Vector3Int.down;
    }

    private void ProcessPlayerClickInputTool(ItemDetails itemDetails)
    {
        var cursorGridPosition = _gridCursor.GetGridPositionForCursor();
        var playerGridPosition = _gridCursor.GetGridPositionForPlayer();
        var playerClickDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);
        var gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (!_gridCursor.CursorPositionIsValid)
            return;

        switch (itemDetails.ItemType)
        {
            case ItemType.HoeingTool:
                HoeGroundAtCursor(gridPropertyDetails, playerClickDirection);
                break;
            case ItemType.WateringTool:
                WaterGroundAtCursor(gridPropertyDetails, playerClickDirection);
                break;
        }
    }

    private void ProcessPlayerClickInputSeed(ItemDetails itemDetails)
    {
        if (itemDetails.CanBeDropped && _gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputCommondity(ItemDetails itemDetails)
    {
        if (itemDetails.CanBeDropped && _gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerClickDirection)
    {
        // Trigger animation
        StartCoroutine(WaterGroundAtCursorRoutine(gridPropertyDetails, playerClickDirection));
    }

    private IEnumerator WaterGroundAtCursorRoutine(GridPropertyDetails gridPropertyDetails, Vector3Int playerClickDirection)
    {
        _playerInputIsDisabled = true;
        _playerToolUseDisabled = true;

        // Set tool animation to hoe in override animation
        _animationOverrides.ApplyCharacterCustomizationParameters(
            new[] { new CharacterAttribute(CharacterPartAnimator.Tool, partVariantType: PartVariantType.WateringCan) });

        // TODO: if there is water in the watering can
        _toolEffect = ToolEffect.Watering;

        if (playerClickDirection == Vector3Int.right)
        {
            _isLiftingToolRight = true;
        }
        else if (playerClickDirection == Vector3Int.left)
        {
            _isLiftingToolLeft = true;
        }
        else if (playerClickDirection == Vector3Int.up)
        {
            _isLiftingToolUp = true;
        }
        else
        {
            _isLiftingToolDown = true;
        }

        yield return _liftToolAnimationPause;

        // Set grid property details for ground watered
        if (gridPropertyDetails.DaysSinceWatered < 0)
        {
            gridPropertyDetails.DaysSinceWatered = 0;
        }

        // Set grid property to watered
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY, gridPropertyDetails);

        // Display watered grid tiles
        GridPropertiesManager.Instance.DisplayWateredGround(gridPropertyDetails);

        // Animation after pause
        yield return _afterLiftToolAnimationPause;

        _playerInputIsDisabled = false;
        _playerToolUseDisabled = false;
    }

    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerClickDirection)
    {
        // Trigger animation
        StartCoroutine(HoeGroundAtCursorRoutine(gridPropertyDetails, playerClickDirection));
    }

    private IEnumerator HoeGroundAtCursorRoutine(GridPropertyDetails gridPropertyDetails, Vector3Int playerClickDirection)
    {
        _playerInputIsDisabled = true;
        _playerToolUseDisabled = true;

        // Set tool animation to hoe in override animation
        _animationOverrides.ApplyCharacterCustomizationParameters(
            new[] { new CharacterAttribute(CharacterPartAnimator.Tool, partVariantType: PartVariantType.Hoe) });

        if (playerClickDirection == Vector3Int.right)
        {
            _isUsingToolRight = true;
        }
        else if (playerClickDirection == Vector3Int.left)
        {
            _isUsingToolLeft = true;
        }
        else if (playerClickDirection == Vector3Int.up)
        {
            _isUsingToolUp = true;
        }
        else
        {
            _isUsingToolDown = true;
        }

        yield return _useToolAnimationPause;

        // Set grid property details for dug ground
        if (gridPropertyDetails.DaysSinceDug < 0)
        {
            gridPropertyDetails.DaysSinceDug = 0;
        }

        // Set grid property to dug
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY, gridPropertyDetails);

        // Display dug grid tiles
        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);

        // Animation after pause
        yield return _afterUseToolAnimationPause;

        _playerInputIsDisabled = false;
        _playerToolUseDisabled = false;
    }

    private void FireMovementEvent()
    {
        EventHandler.CallMovementEvent(
            _xInput,
            _yInput,
            _isWalking,
            _isRunning,
            _isIdle,
            _isCarrying,
            _toolEffect,
            _isUsingToolRight,
            _isUsingToolLeft,
            _isUsingToolUp,
            _isUsingToolDown,
            _isLiftingToolRight,
            _isLiftingToolLeft,
            _isLiftingToolUp,
            _isLiftingToolDown,
            _isPickingRight,
            _isPickingLeft,
            _isPickingUp,
            _isPickingDown,
            _isSwingingToolRight,
            _isSwingingToolLeft,
            _isSwingingToolUp,
            _isSwingingToolDown,
            false,
            false,
            false,
            false);
    }
}
