using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(NPCPath))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class NPCMovement : MonoBehaviour
{
    [HideInInspector]
    public SceneName NPCCurrentScene;

    [HideInInspector]
    public SceneName NPCTargetScene;

    [HideInInspector]
    public Vector3Int NPCCurrentGridPosition;

    [HideInInspector]
    public Vector3Int NPCTargetGridPosition;

    [HideInInspector]
    public Vector3 NPCTargetWorldPosition;

    [HideInInspector]
    public Direction NPCFacingDirectionAtDestination;

    private SceneName _npcPreviousMovementStepScene;

    private Vector3Int _npcNextGridPosition;

    private Vector3 _npcNextWorldPosition;

    [Header("NPC Movement")]
    public float NPCNormalSpeed = 2f;

    [SerializeField]
    private float _npcMinSpeed = 1f;

    [SerializeField]
    private float _npcMaxSpeed = 3f;

    private bool _npcIsMoving = false;

    [HideInInspector]
    public AnimationClip NPCTargetAnimationClip;

    [Header("NPC Animation")]
    [SerializeField]
    private AnimationClip _blankAnimation = null;

    private Grid _grid;

    private Rigidbody2D _rigidBody2D;

    private BoxCollider2D _boxCollider2D;

    private WaitForFixedUpdate _waitForFixedUpdate;

    private Animator _animator;

    private AnimatorOverrideController _animatorOverrideController;

    private int _lastMoveAnimationParameter;

    private NPCPath _npcPath;

    private bool _npcInitialized = false;

    private SpriteRenderer _spriteRenderer;

    [HideInInspector]
    public bool NPCActiveInScene = false;

    private bool _sceneLoaded = false;

    private Coroutine _moveToGridPositionRoutine;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloaded;
    }

    private void AfterSceneLoad()
    {
        _grid = GameObject.FindObjectOfType<Grid>();

        if (!_npcInitialized)
        {
            InitializeNPC();
            _npcInitialized = true;
        }

        _sceneLoaded = true;
    }

    private void BeforeSceneUnloaded()
    {
        _sceneLoaded = false;
    }

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _npcPath = GetComponent<NPCPath>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _animatorOverrideController;

        // Initialize target world position, target grid position, target scene to current
        this.NPCTargetScene = this.NPCCurrentScene;
        this.NPCTargetGridPosition = this.NPCCurrentGridPosition;
        this.NPCTargetWorldPosition = transform.position;
    }

    private void Start()
    {
        _waitForFixedUpdate = new WaitForFixedUpdate();
        SetIdleAnimation();
    }

    private void FixedUpdate()
    {
        if (!_sceneLoaded)
            return;

        if (!_npcIsMoving)
            return;

        // set npc current and next grid position - to take into account the npc might be animating
        this.NPCCurrentGridPosition = GetGridPosition(transform.position);
        _npcNextGridPosition = this.NPCCurrentGridPosition;

        if (_npcPath.NPCMovementStepStack.Any())
        {
            var npcMovementStep = _npcPath.NPCMovementStepStack.Peek();
            this.NPCCurrentScene = npcMovementStep.SceneName;

            // If NPC is in current scene then set NPC to active to make visible, pop the movement step off the stack and then call method to move NPC
            if (this.NPCCurrentScene.ToString() == SceneManager.GetActiveScene().name)
            {
                SetNPCActiveInScene(true);

                npcMovementStep = _npcPath.NPCMovementStepStack.Pop();

                _npcNextGridPosition = (Vector3Int)npcMovementStep.GridCoordinate;

                var npcMovementStepTime = new TimeSpan(npcMovementStep.Hour, npcMovementStep.Minute, npcMovementStep.Second);

                MoveToGridPosition(_npcNextGridPosition, npcMovementStepTime, TimeManager.Instance.GetGameTime());
            }
        }
        // else if no more NPC movement steps
        else
        {
            ResetMoveAnimation();
            SetNPCFacingDirection();
            SetNPCEventAnimation();
        }
    }

    public void SetScheduleEventDetails(NPCScheduleEvent npcScheduleEvent)
    {
        this.NPCTargetScene = npcScheduleEvent.ToSceneName;
        this.NPCTargetGridPosition = (Vector3Int)npcScheduleEvent.ToGridCoordinate;
        this.NPCTargetWorldPosition = GetWorldPosition(this.NPCTargetGridPosition);
        this.NPCFacingDirectionAtDestination = npcScheduleEvent.NPCFacingDirectionAtDestination;
        this.NPCTargetAnimationClip = npcScheduleEvent.AnimationAtDestination;
        ClearNPCEventAnimation();
    }

    private void SetNPCEventAnimation()
    {
        if (this.NPCTargetAnimationClip != null)
        {
            ResetIdleAnimation();
            _animatorOverrideController[_blankAnimation] = this.NPCTargetAnimationClip;
            _animator.SetBool(Settings.eventAnimation, true);
        }
        else
        {
            _animatorOverrideController[_blankAnimation] = _blankAnimation;
            _animator.SetBool(Settings.eventAnimation, false);
        }
    }

    private void ClearNPCEventAnimation()
    {
        _animatorOverrideController[_blankAnimation] = _blankAnimation;
        _animator.SetBool(Settings.eventAnimation, false);

        // Clear any rotation on npc
        transform.rotation = Quaternion.identity;
    }

    private void SetNPCFacingDirection()
    {
        ResetIdleAnimation();

        (bool isRight, bool isLeft, bool isUp, bool isDown) = Util.DirectionToFlag(this.NPCFacingDirectionAtDestination);

        _animator.SetBool(Settings.idleRight, isRight);
        _animator.SetBool(Settings.idleLeft, isLeft);
        _animator.SetBool(Settings.idleUp, isUp);
        _animator.SetBool(Settings.idleDown, isDown);
    }

    public void SetNPCActiveInScene(bool isActive)
    {
        _spriteRenderer.enabled = isActive;
        _boxCollider2D.enabled = isActive;
        this.NPCActiveInScene = isActive;
    }

    private void SetMoveAnimation(Vector3Int gridPosition)
    {
        ResetIdleAnimation();
        ResetMoveAnimation();

        var toWorldPosition = GetWorldPosition(gridPosition);

        var directionVector = toWorldPosition - transform.position;

        if (Mathf.Abs(directionVector.x) >= Mathf.Abs(directionVector.y))
        {
            // Use left/right animation
            if (directionVector.x > 0)
            {
                _animator.SetBool(Settings.walkRight, true);
            }
            else
            {
                _animator.SetBool(Settings.walkLeft, true);
            }
        }
        else
        {
            // Use left/right animation
            if (directionVector.y > 0)
            {
                _animator.SetBool(Settings.walkUp, true);
            }
            else
            {
                _animator.SetBool(Settings.walkDown, true);
            }
        }
    }

    private void SetIdleAnimation()
    {
        _animator.SetBool(Settings.idleDown, true);
    }

    private void ResetMoveAnimation()
    {
        _animator.SetBool(Settings.walkRight, false);
        _animator.SetBool(Settings.walkLeft, false);
        _animator.SetBool(Settings.walkUp, false);
        _animator.SetBool(Settings.walkDown, false);
    }

    private void ResetIdleAnimation()
    {
        _animator.SetBool(Settings.idleRight, false);
        _animator.SetBool(Settings.idleLeft, false);
        _animator.SetBool(Settings.idleUp, false);
        _animator.SetBool(Settings.idleDown, false);
    }

    private void InitializeNPC()
    {
        SetNPCActiveInScene(this.NPCCurrentScene.ToString() == SceneManager.GetActiveScene().name);

        this.NPCCurrentGridPosition = GetGridPosition(transform.position);

        _npcNextGridPosition = this.NPCCurrentGridPosition;
        this.NPCTargetGridPosition = this.NPCCurrentGridPosition;
        this.NPCTargetWorldPosition = GetWorldPosition(this.NPCTargetGridPosition);

        _npcNextWorldPosition = GetWorldPosition(this.NPCCurrentGridPosition);
    }

    private Vector3Int GetGridPosition(Vector3 worldPosition) => _grid?.WorldToCell(worldPosition) ?? Vector3Int.zero;

    private Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        var worldPosition = _grid.CellToWorld(gridPosition);

        // Get center of the grid square
        return new Vector3(
            worldPosition.x + (Settings.GridCellSize / 2f),
            worldPosition.y + (Settings.GridCellSize / 2f),
            worldPosition.z);
    }

    private void MoveToGridPosition(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        _moveToGridPositionRoutine = StartCoroutine(MoveToGridPositionRoutine(gridPosition, npcMovementStepTime, gameTime));
    }

    private IEnumerator MoveToGridPositionRoutine(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        _npcIsMoving = true;

        SetMoveAnimation(gridPosition);

        _npcNextWorldPosition = GetWorldPosition(gridPosition);

        // if movement step time is in the future, otherwise skip and move NPC immediately to position
        if (npcMovementStepTime > gameTime)
        {
            // calculate time difference in seconds
            float timeToMove = (float)(npcMovementStepTime.TotalSeconds - gameTime.TotalSeconds);

            // calculate speed
            float npcCalculatedSpeed = Mathf.Max(_npcMinSpeed, Vector3.Distance(transform.position, _npcNextWorldPosition) / timeToMove / Settings.SecondsPerGameSecond);

            // if speed is at least npc max speed then process, otherwise skip and move npc immediately to posiion
            if (npcCalculatedSpeed <= _npcMaxSpeed)
            {
                while (Vector3.Distance(transform.position, _npcNextWorldPosition) > Settings.PixelSize)
                {
                    var unitVector = Vector3.Normalize(_npcNextWorldPosition - transform.position);
                    var move = new Vector2(unitVector.x * npcCalculatedSpeed * Time.fixedDeltaTime, unitVector.y * npcCalculatedSpeed * Time.fixedDeltaTime);

                    _rigidBody2D.MovePosition(_rigidBody2D.position + move);

                    yield return _waitForFixedUpdate;
                }
            }
        }

        _rigidBody2D.position = _npcNextWorldPosition;
        this.NPCCurrentGridPosition = gridPosition;
        _npcNextGridPosition = this.NPCCurrentGridPosition;
        _npcIsMoving = false;
    }
}
