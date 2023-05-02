using UnityEngine;

public class MovementAnimationParameterControl : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.MovementEvent += SetAnimationParameters;
    }

    private void OnDisable()
    {
        EventHandler.MovementEvent -= SetAnimationParameters;
    }

    private void SetAnimationParameters(
        float xInput,
        float yInput,
        bool isWalking,
        bool isRunning,
        bool isIdle,
        bool isCarrying,
        ToolEffect toolEffect,
        bool isUsingToolRight,
        bool isUsingToolLeft,
        bool isUsingToolUp,
        bool isUsingToolDown,
        bool isLiftingToolRight,
        bool isLiftingToolLeft,
        bool isLiftingToolUp,
        bool isLiftingToolDown,
        bool isPickingRight,
        bool isPickingLeft,
        bool isPickingUp,
        bool isPickingDown,
        bool isSwingingToolRight,
        bool isSwingingToolLeft,
        bool isSwingingToolUp,
        bool isSwingingToolDown,
        bool idleRight,
        bool idleLeft,
        bool idleUp,
        bool idleDown)
    {
        _animator.SetFloat(Settings.xInput, xInput);
        _animator.SetFloat(Settings.yInput, yInput);
        _animator.SetBool(Settings.isWalking, isWalking);
        _animator.SetBool(Settings.isRunning, isRunning);

        _animator.SetInteger(Settings.toolEffect, (int)toolEffect);

        if (isUsingToolRight)       _animator.SetTrigger(Settings.isUsingToolRight);
        if (isUsingToolLeft)        _animator.SetTrigger(Settings.isUsingToolLeft);
        if (isUsingToolUp)          _animator.SetTrigger(Settings.isUsingToolUp);
        if (isUsingToolDown)        _animator.SetTrigger(Settings.isUsingToolDown);

        if (isLiftingToolRight)     _animator.SetTrigger(Settings.isLiftingToolRight);
        if (isLiftingToolLeft)      _animator.SetTrigger(Settings.isLiftingToolLeft);
        if (isLiftingToolUp)        _animator.SetTrigger(Settings.isLiftingToolUp);
        if (isLiftingToolDown)      _animator.SetTrigger(Settings.isLiftingToolDown);

        if (isPickingRight)         _animator.SetTrigger(Settings.isPickingRight);
        if (isPickingLeft)          _animator.SetTrigger(Settings.isPickingLeft);
        if (isPickingUp)            _animator.SetTrigger(Settings.isPickingUp);
        if (isPickingDown)          _animator.SetTrigger(Settings.isPickingDown);

        if (isSwingingToolRight)    _animator.SetTrigger(Settings.isSwingingToolRight);
        if (isSwingingToolLeft)     _animator.SetTrigger(Settings.isSwingingToolLeft);
        if (isSwingingToolUp)       _animator.SetTrigger(Settings.isSwingingToolUp);
        if (isSwingingToolDown)     _animator.SetTrigger(Settings.isSwingingToolDown);

        if (idleRight)              _animator.SetTrigger(Settings.idleRight);
        if (idleLeft)               _animator.SetTrigger(Settings.idleLeft);
        if (idleUp)                 _animator.SetTrigger(Settings.idleUp);
        if (idleDown)               _animator.SetTrigger(Settings.idleDown);
    }

    private void AnimationEventPlayFootstepSound()
    {

    }
}
