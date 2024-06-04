using Unity.VisualScripting;
using UnityEngine;

public class PlayerJumpAttackState : PlayerBaseState
{
    public PlayerJumpAttackState (PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        // _ctx.Animancer.Play(_ctx.JumpAttack);
        handleAnimation();
        _ctx.IsJumpAttacking = true;
        Debug.Log("PlayerJumpAttackState Enter");
    }

    private float temp = 0.0f;
    public override void UpdateState()
    {
        CheckSwitchStates();

        if (_ctx.IsJumpAttacking)
        {   
            temp += 1f;
            _ctx.AppliedMovementY = 0.1f + Time.deltaTime * temp;
        }
    }

    public override void ExitState (){
    }
    public override void CheckSwitchStates(){
        if (!_ctx.IsJumpAttacking)
        {
            SwitchState(_factory.JumpDefault());
        }
    }
    public override void InitializeSubState(){
    }

    private void handleAnimation()
    {
        if (_ctx.CurJumpAttackPhase == 0)
        {
            _ctx.Animancer.Play(_ctx.JumpAttackFirst);
        }else if (_ctx.CurJumpAttackPhase == 1)
        {
            _ctx.Animancer.Play(_ctx.JumpAttackSecond);
        }
        else if (_ctx.CurJumpAttackPhase == 2)
        {
            _ctx.Animancer.Play(_ctx.JumpAttackThird);
        }
    }
}
