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

    private Rigidbody2D _rigidBody;

    private Direction _playerDirection;

    private float _movementSpeed;

    private bool _playerInputIsDisabled = false;

    private Camera _camera;

    public bool PlayerInputIsDisabled { get => _playerInputIsDisabled; set => _playerInputIsDisabled = value; }

    public Vector3 GetViewportPosition() => _camera.WorldToViewportPoint(transform.position);

    protected override void Awake()
    {
        base.Awake();

        _rigidBody = GetComponent<Rigidbody2D>();

        _camera = Camera.main;
    }

    private void Update()
    {
        #region Player Input
        ResetAnimationTriggers();
        PlayerMovementInput();
        PlayerWalkInput();
        FireMovementEvent();
        #endregion
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
            _movementSpeed = Settings.runningSpeed;

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
        _movementSpeed = shiftPressed ? Settings.walkingSpeed : Settings.runningSpeed;
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
