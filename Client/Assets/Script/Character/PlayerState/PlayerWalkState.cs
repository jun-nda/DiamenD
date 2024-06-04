using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState (PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        _ctx.Animancer.Play(_ctx.Walk);
        _ctx.CurSpeed = _ctx._WalkSpeed;
    }

    public override void UpdateState() {
        CheckSwitchStates();
        handleRotation();
        _ctx.CameraRelativeMovement = PlayerControlUtil.ConvertToCameraSpace(_ctx.CurDirection);
        _ctx.AppliedMovementX = _ctx.CurSpeed * _ctx.CameraRelativeMovement.x;
        _ctx.AppliedMovementZ = _ctx.CurSpeed  * _ctx.CameraRelativeMovement.z;
    }
    public override void ExitState (){}
    public override void CheckSwitchStates () {
        if (_ctx.IsGroundedAttackPressed)
        {
            SwitchState(_factory.GroundedAttack());
        }
        else
        {
            if (!_ctx.IsMovementPressed) {
                SwitchState(_factory.Idle());
            }else if (_ctx.IsMovementPressed && _ctx.IsRunPressed) {
                SwitchState(_factory.Run());
            }
        }
    }
    public override void InitializeSubState (){}
}
