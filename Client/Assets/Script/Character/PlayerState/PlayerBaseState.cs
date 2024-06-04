using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState {
	protected PlayerStateMachine _ctx;
	protected PlayerStateFactory _factory;
	protected PlayerBaseState _currentSubState;
	protected PlayerBaseState _currentSuperState;
	protected bool _isRootState = false;
	public PlayerBaseState (PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) {
		_ctx = currentContext;
		_factory = playerStateFactory;
	}
	
	public abstract void EnterState ();
	public abstract void UpdateState ();
	public abstract void ExitState ();
	public abstract void CheckSwitchStates();
	public abstract void InitializeSubState();

	public void UpdateStates()
	{
		UpdateState();
		if (_currentSubState != null)
		{
			_currentSubState.UpdateStates();
		}
	}

	protected void SwitchState (PlayerBaseState newState) {
		// Current state exits state
		ExitState();
		
		// new state enters state
		newState.EnterState();

		if (_isRootState) {
			_ctx.CurrentState = newState;
		}else if (_currentSuperState != null) {
			_currentSuperState.setSubState(newState);
		}
		
	}

	protected void setSuperState (PlayerBaseState newSuperState) {
		_currentSuperState = newSuperState;
	}
	protected void setSubState (PlayerBaseState newSubState) {
		_currentSubState = newSubState;
		newSubState.EnterState();
		newSubState.setSuperState(this);
	}

    protected void handleRotation () {
        if (_ctx.CurDirection != Vector3.zero) {
            // Debug.Log("_curDirection" + _curDirection);
            // Debug.Log("_cameraRelativeMovement" + _cameraRelativeMovement);
            Quaternion curQ = Quaternion.LookRotation(_ctx.CameraRelativeMovement);
            _ctx.transform.rotation = Quaternion.Slerp(_ctx.transform.rotation, curQ,_ctx.RotSpeed * Time.deltaTime);
        }
    }
    
}
