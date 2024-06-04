using Unity.VisualScripting;
using UnityEngine;

public class PlayerGroundedAttackState : PlayerBaseState
{
    public PlayerGroundedAttackState (PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    private Vector2 _groundedAttackDirection = new Vector2();
    public override void EnterState()
    {
        // _ctx.Animancer.Play(_ctx.JumpAttack);
        handleAnimation();
        _ctx.IsGroundedAttacking = true;
        _ctx.CurSpeed = _ctx._AttackMoveSpeed;
        _groundedAttackDirection = _ctx.CurDirection;
    }

    private float temp;
    public override void UpdateState()
    {
        CheckSwitchStates();

        _ctx.CameraRelativeMovement = PlayerControlUtil.ConvertToCameraSpace(_groundedAttackDirection);
        // Debug.Log("CurDirection" + _ctx.CurDirection);
        _ctx.AppliedMovementX = _ctx.CurSpeed * _ctx.CameraRelativeMovement.x;
        _ctx.AppliedMovementZ = _ctx.CurSpeed  * _ctx.CameraRelativeMovement.z;
        
        // if (_ctx.CurrentGroundedAttackRoutine != null && _ctx.CurGroundedAttackPhase < 2)
        // {
        //     _ctx.StopCoroutine(_ctx.CurrentGroundedAttackRoutine);
        //     _ctx.CurrentGroundedAttackRoutine = null;
        // }
    }

    public override void ExitState ()
    {
    }
    public override void CheckSwitchStates(){
        if (!_ctx.IsGroundedAttacking)
        {
            if (_ctx.IsMovementPressed && _ctx.IsRunPressed)
            {
                SwitchState(_factory.Run());
            }else if (_ctx.IsMovementPressed)
            {
                SwitchState(_factory.Walk());
            }
            else
            {
                SwitchState(_factory.Idle());
            }
        }
    }
    public override void InitializeSubState(){
    }

    void handleAnimation()
    {
        if (_ctx.IsGroundedAttackPressed)
        {
            if (_ctx.CurGroundedAttackPhase == 0)
            {
                _ctx.Animancer.Play(_ctx.GroundedAttackFirst);
            }else if (_ctx.CurGroundedAttackPhase == 1)
            {
                _ctx.Animancer.Play(_ctx.GroundedAttackSecond);
            }
            else if (_ctx.CurGroundedAttackPhase == 2)
            {
                _ctx.Animancer.Play(_ctx.GroundedAttackThird);
            }
        }
    }
}