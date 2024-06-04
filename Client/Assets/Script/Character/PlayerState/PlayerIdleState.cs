using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState (PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        _ctx.Animancer.Play(_ctx.Ilde);

        _ctx.AppliedMovementX = 0;
        _ctx.AppliedMovementZ = 0;
    }
    public override void UpdateState () {
        CheckSwitchStates();
        handleRotation();

        _ctx.AppliedMovementX = 0;
        _ctx.AppliedMovementZ = 0;
    }
    public override void ExitState (){}

    public override void CheckSwitchStates()
    {
        if (_ctx.IsGroundedAttackPressed)
        {
            SwitchState(_factory.GroundedAttack());
        }
        else
        {
            if (_ctx.IsMovementPressed && _ctx.IsRunPressed)
            {
                SwitchState(_factory.Run());
            }else if (_ctx.IsMovementPressed)
            {
                SwitchState(_factory.Walk());
            }
        }

    }
    public override void InitializeSubState () {
    }
}
