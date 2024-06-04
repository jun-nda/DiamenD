using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable All

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState (PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
        _isRootState = true;
    }

    public override void EnterState()
    {
        handleJump();
        handleAnimation();
        InitializeSubState();
    }

    public override void UpdateState()
    {
        _ctx.IsGroundedAttackPressed = false;
        CheckSwitchStates();
        handleGravity();

        // Debug.Log(_ctx.Animancer.States.Current);

    }

    public override void ExitState()
    {
        if (_ctx.CurrentJumpResetRoutine == null)
        {
            _ctx.CurrentJumpResetRoutine = _ctx.StartCoroutine(IJumpResetRoutine());
        }
    }

    public override void CheckSwitchStates()
    {
        if (_ctx.CharacterController.isGrounded)
        {
            SwitchState(_factory.Grouded());
        }
    }

    public override void InitializeSubState()
    {
        if (_ctx.IsJumpAttackPressed)
        {
            setSubState(_factory.JumpAttack());
        }
        else
        {
            setSubState(_factory.JumpDefault());
        }
    }

    void handleJump()
    {
        if (_ctx.JumpCount < 3 && _ctx.CurrentJumpResetRoutine != null)
        {
            _ctx.StopCoroutine(_ctx.CurrentJumpResetRoutine);
            _ctx.CurrentJumpResetRoutine = null;
        }
        if (_ctx.JumpCount == 2)
        {
            _ctx.JumpCount = -1;
        }
        _ctx.IsJumping = true;
        _ctx.JumpCount += 1;
        _ctx.AppliedMovementY = _ctx.InitialJumpVelocities[2] * .5f;
    }
    
    void handleGravity () {
        bool isFalling = _ctx.AppliedMovementY <= 0.0f || !_ctx.IsJumpPressed;
        float fallMultiplier =  isFalling ? 2.0f : 1.0f;

        // 印象中这也不是verlet呀，先这么写着吧
        if (_ctx.JumpCount < 0) return;
        float previouseV = _ctx.AppliedMovementY;
        float newV = _ctx.AppliedMovementY + _ctx.JumpGravities[_ctx.JumpCount] * fallMultiplier * Time.deltaTime;
        float nextV = (previouseV + newV) * .5f;
        _ctx.AppliedMovementY = nextV;
		
    }

    void handleAnimation()
    {
        // if (_ctx.JumpCount == 0)
        // {
            _ctx.Animancer.Play(_ctx.JumpFirst);
        // }else if (_ctx.JumpCount == 1)
        // {
        //     _ctx.Animancer.Play(_ctx.JumpSecond);
        // }else if (_ctx.JumpCount == 2)
        // {
        //     _ctx.Animancer.Play(_ctx.JumpThird);
        // }
    }
    IEnumerator IJumpResetRoutine()
    {
        yield return new WaitForSeconds(1f);
        _ctx.JumpCount = -1;
    }
}
