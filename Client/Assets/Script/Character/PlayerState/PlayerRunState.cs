using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState (PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        _ctx.Animancer.Play(_ctx.Run);
        _ctx.CurSpeed = _ctx._RunSpeed;
    }
    public override void UpdateState () {
        CheckSwitchStates();
        handleRotation();
        _ctx.CameraRelativeMovement = PlayerControlUtil.ConvertToCameraSpace(_ctx.CurDirection);
        // Debug.Log("CurDirection: " + _ctx.CurDirection);
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
            if (!_ctx.IsRunPressed && _ctx.IsMovementPressed) {
                SwitchState(_factory.Walk());
            }else if (!_ctx.IsRunPressed && !_ctx.IsMovementPressed) {
                SwitchState(_factory.Idle());
            }
        }
    }
    public override void InitializeSubState (){}
    
}
