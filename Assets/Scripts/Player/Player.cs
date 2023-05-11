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

    #region WaitForSeconds caching
    private WaitForSeconds _useToolAnimationPause = new WaitForSeconds(Settings.UseToolAnimationPause);

    private WaitForSeconds _afterUseToolAnimationPause = new WaitForSeconds(Settings.AfterUseToolAnimationPause);

    private WaitForSeconds _liftToolAnimationPause = new WaitForSeconds(Settings.LiftToolAnimationPause);

    private WaitForSeconds _afterLiftToolAnimationPause = new WaitForSeconds(Settings.AfterLiftToolAnimationPause);

    private WaitForSeconds _collectAnimationPause = new WaitForSeconds(Settings.CollectAnimationPause);

    private WaitForSeconds _afterCollectAnimationPause = new WaitForSeconds(Settings.AfterCollectAnimationPause);
    #endregion

    private bool _playerToolUseDisabled = false;

    private GridCursor _gridCursor;

    private Cursor _cursor;

    private Rigidbody2D _rigidBody;

    private Direction _playerDirection;

    private float _movementSpeed;

    private bool _playerInputIsDisabled = false;

    private Camera _camera;

    private AnimationOverrides _animationOverrides;

    private CharacterAttribute _armsNoneCharacterAttribute;
    private CharacterAttribute _armsCarryCharacterAttribute;

    private CharacterAttribute _toolHoeCharacterAttribute;
    private CharacterAttribute _toolWateringCanCharacterAttribute;
    private CharacterAttribute _toolScytheCharacterAttribute;

    [SerializeField]
    private SpriteRenderer _equippedItemSpriteRenderer = null;

    // Player attributes that can be swapped
    public bool PlayerInputIsDisabled { get => _playerInputIsDisabled; set => _playerInputIsDisabled = value; }

    public Vector3 GetViewportPosition() => _camera.WorldToViewportPoint(transform.position);

    public Vector3 GetPlayerCenterPosition() =>
        new Vector3(
            transform.position.x,
            transform.position.y + Settings.PlayerCenterYOffset,
            transform.position.z);

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
        _animationOverrides.ApplyCharacterCustomizationParameters(new[] { _armsNoneCharacterAttribute });

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
        _animationOverrides.ApplyCharacterCustomizationParameters(new[] { _armsCarryCharacterAttribute });

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

        _armsNoneCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.Arms, PartVariantColor.None, PartVariantType.None);
        _armsCarryCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.Arms, PartVariantColor.None, PartVariantType.Carry);
        _toolHoeCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.Tool, PartVariantColor.None, PartVariantType.Hoe);
        _toolWateringCanCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.Tool, PartVariantColor.None, PartVariantType.WateringCan);
        _toolScytheCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.Tool, PartVariantColor.None, PartVariantType.Scythe);
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadFadeOutEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadFadeInEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadFadeOutEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadFadeInEvent -= AfterSceneLoadFadeIn;
    }

    private void BeforeSceneUnloadFadeOut()
    {
        SetPlayerInputDisabled();
    }

    private void AfterSceneLoadFadeIn()
    {
        SetPlayerInputDisabled(false);
    }

    private void Start()
    {
        _gridCursor = FindObjectOfType<GridCursor>();
        _cursor = FindObjectOfType<Cursor>();
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
            if (_gridCursor.CursorIsEnabled || _cursor.CursorIsEnabled)
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

        var cursorGridPosition = _gridCursor.GetGridPositionForCursor();
        var playerGridPosition = _gridCursor.GetGridPositionForPlayer();
        var playerClickDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);
        var gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        switch (itemDetails.ItemType)
        {
            case ItemType.Seed:
                ProcessPlayerClickInputSeed(itemDetails, gridPropertyDetails);
                break;
            case ItemType.Commodity:
                ProcessPlayerClickInputCommondity(itemDetails);
                break;
            // case other tools also following same path?
            case ItemType.HoeingTool:
            case ItemType.WateringTool:
            case ItemType.ReapingTool:
            case ItemType.CollectingTool:
                ProcessPlayerClickInputTool(itemDetails, gridPropertyDetails, playerClickDirection);
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

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
        if ((cursorPosition.x > playerPosition.x) &&
            (cursorPosition.y < (playerPosition.y + _cursor.ItemUseRadius / 2f)) &&
            (cursorPosition.y > (playerPosition.y - _cursor.ItemUseRadius / 2f)))
        {
            return Vector3Int.right;
        }

        if ((cursorPosition.x < playerPosition.x) &&
            (cursorPosition.y < (playerPosition.y + _cursor.ItemUseRadius / 2f)) &&
            (cursorPosition.y > (playerPosition.y - _cursor.ItemUseRadius / 2f)))
        {
            return Vector3Int.left;
        }

        if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }

        return Vector3Int.down;
    }

    private void ProcessPlayerClickInputTool(ItemDetails itemDetails, GridPropertyDetails gridPropertyDetails, Vector3Int playerClickDirection)
    {
        switch (itemDetails.ItemType)
        {
            case ItemType.HoeingTool:
                if (_gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerClickDirection);
                }
                break;
            case ItemType.WateringTool:
                if (_gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCursor(gridPropertyDetails, playerClickDirection);
                }
                break;
            case ItemType.ReapingTool:
                if (_cursor.CursorPositionIsValid)
                {
                    playerClickDirection = GetPlayerDirection(_cursor.GetWorldPositionForCursor(), GetPlayerCenterPosition());
                    ReapInPlayerDirectionAtCursor(itemDetails, playerClickDirection);
                }
                break;
            case ItemType.CollectingTool:
                if (_gridCursor.CursorPositionIsValid)
                {
                    CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerClickDirection);
                }
                break;
        }
    }

    private void ProcessPlayerClickInputSeed(ItemDetails itemDetails, GridPropertyDetails gridPropertyDetails)
    {
        if (itemDetails.CanBeDropped && _gridCursor.CursorPositionIsValid)
        {
            if ((gridPropertyDetails.DaysSinceDug >= 0) && (itemDetails.ItemCode > 0))
            {
                PlantSeedAtCursor(itemDetails, gridPropertyDetails);
                return;
            }

            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void PlantSeedAtCursor(ItemDetails itemDetails, GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.SeedItemCode = itemDetails.ItemCode;
        gridPropertyDetails.GrowthDays = 0;

        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

        EventHandler.CallRemoveSelectedItemFromInventoryEvent();
    }

    private void ProcessPlayerClickInputCommondity(ItemDetails itemDetails)
    {
        if (itemDetails.CanBeDropped && _gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerClickDirection)
    {
        StartCoroutine(CollectInPlayerDirectionRoutine(gridPropertyDetails, itemDetails, playerClickDirection));
    }

    private IEnumerator CollectInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerClickDirection)
    {
        _playerInputIsDisabled = true;
        _playerToolUseDisabled = true;

        ProcessCropWithEquippedItemInPlayerDirection(gridPropertyDetails, itemDetails, playerClickDirection);

        yield return _collectAnimationPause;

        yield return _afterCollectAnimationPause;

        _playerInputIsDisabled = false;
        _playerToolUseDisabled = false;
    }

    private void ProcessCropWithEquippedItemInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerClickDirection)
    {
        (_isPickingRight, _isPickingLeft, _isPickingUp, _isPickingDown) = Util.Vector3IntDirectionToFlag(playerClickDirection);

        var crop = GridPropertiesManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails.GridX, gridPropertyDetails.GridY);
        if (crop == null)
            return;

        switch (itemDetails.ItemType)
        {
            case ItemType.CollectingTool:
                crop.ProcessToolAction(itemDetails);
                break;
        }
    }

    private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
    }

    private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        _playerInputIsDisabled = true;
        _playerToolUseDisabled = true;

        _animationOverrides.ApplyCharacterCustomizationParameters(new[] { _toolScytheCharacterAttribute });

        // Reap in player direction
        UseToolInPlayerDirection(itemDetails, playerDirection);

        yield return _useToolAnimationPause;

        _playerInputIsDisabled = false;
        _playerToolUseDisabled = false;
    }

    private void UseToolInPlayerDirection(ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        (_isSwingingToolRight, _isSwingingToolLeft, _isSwingingToolUp, _isSwingingToolDown) = Util.Vector3IntDirectionToFlag(playerDirection);

        // Define center point of square which will be used for collision testing
        var playerCenterPosition = GetPlayerCenterPosition();
        var point = new Vector2(
            playerCenterPosition.x + (playerDirection.x * (equippedItemDetails.ItemUseRadius / 2f)),
            playerCenterPosition.y + (playerDirection.y * (equippedItemDetails.ItemUseRadius / 2f)));

        // Define size of the square which will be used for collision testing
        var size = new Vector2(equippedItemDetails.ItemUseRadius, equippedItemDetails.ItemUseRadius);

        // get item components with 2D collider located in the square at the center point defined (2d colliders tested limited to MaxCollidersToTestPerReapSwing)
        foreach (var component in Util.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.MaxCollidersToTestPerReapSwing, point, size, 0f))
        {
            if (InventoryManager.Instance.GetItemDetails(component.ItemCode).ItemType != ItemType.ReapableScenery)
                continue;

            // Effect position
            var effectPosition =
                new Vector3(
                    component.transform.position.x,
                    component.transform.position.y + Settings.GridCellSize / 2f,
                    component.transform.position.z);

            // Trigger Reaping Effect
            EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.Reaping);

            Destroy(component.gameObject);
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

        // Set tool animation to watering can in override animation
        _animationOverrides.ApplyCharacterCustomizationParameters(new[] { _toolWateringCanCharacterAttribute });

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
            new[] { _toolHoeCharacterAttribute });

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
