using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState (PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
        _isRootState = true;
    }

    public override void EnterState()
    {
        _ctx.Animancer.Play(_ctx.Ilde);
    
        _ctx.AppliedMovementY = _ctx.GroundedGravity * Time.deltaTime;

        resetState();
        
        InitializeSubState();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        _ctx.IsJumpAttackPressed = false;
    }
    public override void ExitState (){}
    public override void CheckSwitchStates () {
        if (_ctx.IsJumpPressed) {
            SwitchState(_factory.Jump());
        }
    }
    public override void InitializeSubState () {
        if (_ctx.IsGroundedAttackPressed)
        {
            setSubState(_factory.GroundedAttack());
        }
        else
        {
            if (!_ctx.IsMovementPressed && !_ctx.IsRunPressed) {
                setSubState(_factory.Idle());
            }else if (_ctx.IsMovementPressed && !_ctx.IsRunPressed) {
                setSubState(_factory.Walk());
            }else if (_ctx.IsRunPressed){
                setSubState(_factory.Run());
            }
        }

    }

    void resetState()
    {
        _ctx.CurJumpAttackPhase = 0;
    }
}
