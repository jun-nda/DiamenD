

using UnityEngine;

public class PlayerJumpDefaultState : PlayerBaseState
{
    public PlayerJumpDefaultState (PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        handleAnimation();
    }
    public override void UpdateState () {
        CheckSwitchStates();
        handleRotation();

        _ctx.CameraRelativeMovement = PlayerControlUtil.ConvertToCameraSpace(_ctx.CurDirection);
        _ctx.AppliedMovementX = _ctx.CurSpeed * _ctx.CameraRelativeMovement.x;
        _ctx.AppliedMovementZ = _ctx.CurSpeed  * _ctx.CameraRelativeMovement.z;
    }
    public override void ExitState (){}

    public override void CheckSwitchStates()
    {
        if (_ctx.IsJumpAttackPressed)
        {
            SwitchState(_factory.JumpAttack());
        }
    }
    public override void InitializeSubState () {
    }
    
    void handleAnimation()
    {
        // if (_ctx.JumpCount == 0)
        // {
            _ctx.Animancer.Play(_ctx.JumpFirst);
            // Debug.Log("PlayerAirDefaultState");
        // }else if (_ctx.JumpCount == 1)
        // {
        //     _ctx.Animancer.Play(_ctx.JumpSecond);
        // }else if (_ctx.JumpCount == 2)
        // {
        //     _ctx.Animancer.Play(_ctx.JumpThird);
        // }
    }
}