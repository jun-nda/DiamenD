using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory {
	PlayerStateMachine _context;
	public PlayerStateFactory (PlayerStateMachine currentContext) {
		_context = currentContext;
	}

	public PlayerBaseState Idle () {
		return new PlayerIdleState(_context, this);
	}
	
	public PlayerBaseState Walk() {
		return new PlayerWalkState(_context, this);
	}
	
	public PlayerBaseState Run () {
		return new PlayerRunState(_context, this);
	}
	
	public PlayerBaseState Jump () {
		return new PlayerJumpState(_context, this);
	}

	public PlayerBaseState Grouded () {
		return new PlayerGroundedState(_context, this);
	}
    
    // jump operation
    public PlayerBaseState JumpDefault()
    {
        return new PlayerJumpDefaultState(_context, this);
    }
    public PlayerBaseState JumpAttack()
    {
        return new PlayerJumpAttackState(_context, this);
    }
    
    // grounded operation
    public PlayerBaseState GroundedAttack()
    {
        return new PlayerGroundedAttackState(_context, this);
    }
}
