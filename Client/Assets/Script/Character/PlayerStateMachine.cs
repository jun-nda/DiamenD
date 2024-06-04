using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable All

public class PlayerStateMachine : MonoBehaviour {
	[SerializeField] AnimancerComponent _animancer;
	[SerializeField] ClipTransition _idle;
	[SerializeField] ClipTransition _walk;
	[SerializeField] ClipTransition _run;
	[SerializeField] ClipTransition _jumpFirst;
	[SerializeField] ClipTransition _jumpSecond;
	[SerializeField] ClipTransition _jumpThird;
    [SerializeField] ClipTransition _jumpAttackFirst;
    [SerializeField] ClipTransition _jumpAttackSecond;
    [SerializeField] ClipTransition _jumpAttackThird;
    [SerializeField] ClipTransition _groundedAttackFirst;
    [SerializeField] ClipTransition _groundedAttackSecond;
    [SerializeField] ClipTransition _groundedAttackThird;
	
	public float _WalkSpeed = 1.8f;
	public float _RunSpeed = 3.8f;
    public float _AttackMoveSpeed = 0.2f;
    
	private bool _isMovementPressed;
	private bool _isRunPressed;
	
	Vector3 _curDirection;
	private float _appliedMovementX;
	private float _appliedMovementZ;
	private float _appliedMovementY;
	float _curSpeed;
	float _curAccelerated;
	bool _isWalking;
	float _deltaTime;
	float _rotSpeed = 10f;
	Vector3 _cameraRelativeMovement;
	
	// gravity variables
	float _gravity = -9.8f;
	float _groundedGravity = -0.5f;
	
	// jumping variables
	bool _isJumpPressed = false;
	float _initialJumpVelocity;
	float _maxJumpHeight = 6.0f;
	float _maxJumpTime = 0.9f;
	bool _isJumping = false;
	private int _jumpCount = -1;
	private List<float> _initialJumpVelocities = new List<float>();
	private List<float> _jumpGravities = new List<float>();
	
    // attack 
    private bool _isGroundedAttackPressed;
    private bool _isJumpAttackPressed;
    private int _curJumpAttackPhase;
    private int _curGroundedAttackPhase;
    private bool _isGroundedAttacking;
    private bool _isJumpAttacking;
    
	private CharacterController _characterController;

	PlayerBaseState _currentState;
	PlayerStateFactory _states;

	private Coroutine _currentJumpResetRoutine = null;
    private Coroutine _currentGroundedAttackRoutine = null;

	// getter and setters
	public bool IsMovementPressed => _isMovementPressed;
	public bool IsRunPressed => _isRunPressed;
	public PlayerBaseState CurrentState {
		get => _currentState;
		set => _currentState = value;
	}
	public bool IsJumpPressed => _isJumpPressed;

	public Coroutine CurrentJumpResetRoutine
	{
		get => _currentJumpResetRoutine;
		set => _currentJumpResetRoutine = value;
	}
    public Coroutine CurrentGroundedAttackRoutine
    {
        get => _currentGroundedAttackRoutine;
        set => _currentGroundedAttackRoutine = value;
    }

	public List<float> InitialJumpVelocities => _initialJumpVelocities;
	public List<float> JumpGravities => _jumpGravities;

	public int JumpCount
	{
		get => _jumpCount;
		set => _jumpCount = value;
	}

	public bool IsJumping
	{
		get => _isJumping;
		set => _isJumping = value;
	}

	public float CurSpeed
    {
        get => _curSpeed;
        set => _curSpeed = value;
    }

    public float RotSpeed
    {
        get => _rotSpeed;
        set => _rotSpeed = value;
    }
    
    
    public Vector3 CurDirection => _curDirection;

	public Vector3 CameraRelativeMovement
	{
		get => _cameraRelativeMovement;
		set => _cameraRelativeMovement = value;
	}
		
	public float AppliedMovementX
	{
		get => _appliedMovementX;
		set => _appliedMovementX = value;
	}
	public float AppliedMovementZ
	{
		get => _appliedMovementZ;
		set => _appliedMovementZ = value;
	}
	public float AppliedMovementY
	{
		get => _appliedMovementY;
		set => _appliedMovementY = value;
	}

	public float GroundedGravity
	{
		get => _groundedGravity;
		set => _groundedGravity = value;
	}

    public bool IsJumpAttackPressed
    {
        get => _isJumpAttackPressed;
        set => _isJumpAttackPressed = value;
    }
    
    public bool IsGroundedAttackPressed
    {
	    get => _isGroundedAttackPressed;
	    set => _isGroundedAttackPressed = value;
    }

    public int CurJumpAttackPhase
    {
	    get => _curJumpAttackPhase;
	    set => _curJumpAttackPhase = value;
    }

    public int CurGroundedAttackPhase
    {
        get => _curGroundedAttackPhase;
        set => _curGroundedAttackPhase = value;
    }

    public bool IsGroundedAttacking
    {
        get => _isGroundedAttacking;
        set => _isGroundedAttacking = value;
    }

    public bool IsJumpAttacking
    {
        get => _isJumpAttacking;
        set => _isJumpAttacking = value;
    }
    
    
	public CharacterController CharacterController
	{
		get => _characterController;
		set => _characterController = value;
	}

	public ClipTransition JumpFirst => _jumpFirst;
	public ClipTransition JumpSecond => _jumpSecond;
	public ClipTransition JumpThird => _jumpThird;
	public ClipTransition Ilde => _idle;
	public ClipTransition Walk => _walk;
	public ClipTransition Run => _run;
    public ClipTransition JumpAttackFirst => _jumpAttackFirst;
    public ClipTransition JumpAttackSecond => _jumpAttackSecond;
    public ClipTransition JumpAttackThird => _jumpAttackThird;
    public ClipTransition GroundedAttackFirst => _groundedAttackFirst;
    public ClipTransition GroundedAttackSecond => _groundedAttackSecond;
    public ClipTransition GroundedAttackThird => _groundedAttackThird;
	public AnimancerComponent Animancer => _animancer;
	
	void onMovementInput (InputAction.CallbackContext context ) {
		var data = context.ReadValue<Vector2>();
		var magnitude = data.magnitude;
		if (magnitude > 0) {
			_curDirection.x = data.x;
			_curDirection.z = data.y;
            if (!_isRunPressed)
                _curSpeed = _WalkSpeed;
            _isMovementPressed = true;
		}
		else
		{
			_isMovementPressed = false;
		}
	}

	void onRunInput (InputAction.CallbackContext context) {
		var data = context.ReadValueAsButton();
		if (data)
        {
            _curSpeed = _RunSpeed;
            _isRunPressed = true;
        }
		else
        {
            _curSpeed = _WalkSpeed;
            _isRunPressed = false;
        }
	}

	void onJump (InputAction.CallbackContext context) {
		_isJumpPressed = context.ReadValueAsButton();
	}

    void onAttack(InputAction.CallbackContext context)
    {
        var data = context.ReadValueAsButton();
        if (data) {
	        _isJumpAttackPressed = true;
	        _isGroundedAttackPressed = true;
        }
        // else {
	       //  _isJumpAttackPressed = false;
	       //  _isGroundedAttackPressed = false
        // }
    }
    
	public void Awake () {
		_characterController = transform.GetComponent<CharacterController>();
		_states = new PlayerStateFactory(this);
		_currentState = _states.Grouded();
		_currentState.EnterState();
		
		InputManager.Input.Character.Movement.started += onMovementInput;
		InputManager.Input.Character.Movement.performed += onMovementInput;
		InputManager.Input.Character.Movement.canceled += onMovementInput;

		InputManager.Input.Character.Run.started += onRunInput;
		InputManager.Input.Character.Run.performed += onRunInput;
		InputManager.Input.Character.Run.canceled += onRunInput;

		InputManager.Input.Character.Jump.started += onJump;
		// InputManager.Input.Character.Jump.performed += onJump;
		InputManager.Input.Character.Jump.canceled += onJump;

        InputManager.Input.Character.Attack.started += onAttack;
        // InputManager.Input.Character.Attack.performed += onAttack;
        // InputManager.Input.Character.Attack.canceled += onAttack;
        
		setupJumpVariables();
		_appliedMovementY = _groundedGravity;

        _jumpFirst.Events.OnEnd += OnJumpEnd;
        
        // jump Attack
		_jumpAttackFirst.Events.Add(0.2f, OnJumpAttackCancelInput);
		_jumpAttackSecond.Events.Add(0.2f, OnJumpAttackCancelInput);
        
		_jumpAttackFirst.Events.OnEnd += OnJumpFirstAttackEnd;
		_jumpAttackSecond.Events.OnEnd += OnJumpSecondAttackEnd;
        _jumpAttackThird.Events.OnEnd += OnJumpThirdAttackEnd;

        // grounded Attack
        _groundedAttackFirst.Events.Add(0.2f, OnGroundedAttackCancelInput);
        _groundedAttackSecond.Events.Add(0.2f, OnGroundedAttackCancelInput);
        
        
        _groundedAttackFirst.Events.OnEnd += OnGroundedFirstAttackEnd;
        _groundedAttackSecond.Events.OnEnd += OnGroundedSecondAttackEnd;
        _groundedAttackThird.Events.OnEnd += OnGroundedThirdAttackEnd;
    }
	
	void OnJumpEnd()
	{
		_isJumpPressed = false;
		_animancer.States.Current.Events = null;
	}
    
	void OnJumpAttackCancelInput()
	{
		_isJumpAttackPressed = false;
    }

	void OnJumpFirstAttackEnd()
    {
	    _curJumpAttackPhase = 1;
	    _animancer.States.Current.Events = null;
        _isJumpAttacking = false;

    }

    void OnJumpSecondAttackEnd()
    {
	    _curJumpAttackPhase = 2;
	    _animancer.States.Current.Events = null;
        _isJumpAttacking = false;

    }
    void OnJumpThirdAttackEnd()
    {
	    // Debug.Log("OnJumpAttackEnd");
	    _isJumpAttackPressed = false;
        _animancer.States.Current.Events = null;
        _curJumpAttackPhase = 0;
        _isJumpAttacking = false;
    }

    void OnGroundedAttackCancelInput()
    {
        _isGroundedAttackPressed = false;
    }
    
    void OnGroundedFirstAttackEnd()
    {
        _curGroundedAttackPhase = 1;
        _animancer.States.Current.Events = null;
        _isGroundedAttacking = false;
        if (_currentGroundedAttackRoutine == null)
        {
            _currentGroundedAttackRoutine = StartCoroutine(IGroundedAttackResetRoutine());
        }
    }
    
    void OnGroundedSecondAttackEnd()
    {
        _curGroundedAttackPhase = 2;
        _animancer.States.Current.Events = null;
        _isGroundedAttacking = false;
        if (_currentGroundedAttackRoutine == null)
        {
            _currentGroundedAttackRoutine = StartCoroutine(IGroundedAttackResetRoutine());
        }
    }
    
    void OnGroundedThirdAttackEnd()
    {
        _curGroundedAttackPhase = 0;
        _animancer.States.Current.Events = null;
        _isGroundedAttackPressed = false;
        _isGroundedAttacking = false;
    }
    
	void setupJumpVariables () {
		// x = 1/2*a*t^2
		// v = at
		float timeToApex = _maxJumpTime / 2;
		_gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
		_initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
		
		float secondJumpGravity = (-2 * (_maxJumpHeight + 2)) / Mathf.Pow(timeToApex * 1.25f, 2);
		float secondInitialJumpVelocity = (2 * (_maxJumpHeight + 2)) / timeToApex;
		
		float thirdJumpGravity = (-2 * (_maxJumpHeight + 4)) / Mathf.Pow(timeToApex * 1.5f, 2);
		float thirdInitialJumpVelocity = (2 * (_maxJumpHeight + 4)) / timeToApex;
		
		_initialJumpVelocities.Add(_initialJumpVelocity);
		_initialJumpVelocities.Add(secondInitialJumpVelocity);
		_initialJumpVelocities.Add(thirdInitialJumpVelocity);
		
		_jumpGravities.Add(_gravity);
		_jumpGravities.Add(secondJumpGravity);
		_jumpGravities.Add(thirdJumpGravity);
	}
	
	void Start()
	{
    
	}
	
	void Update()
    {
        // Debug.Log("curSpeed" + _curSpeed);
        // Debug.Log("Update: " + Time.frameCount + ", " + _isJumpPressed);
		_currentState.UpdateStates();
		handleMove();
	}

	void handleMove()
	{
		_characterController.Move(new Vector3(_appliedMovementX,_appliedMovementY,_appliedMovementZ) * Time.deltaTime);
	}
    
    IEnumerator IGroundedAttackResetRoutine()
    {
        yield return new WaitForSeconds(0.005f);
        _curGroundedAttackPhase = 0;
        // Debug.Log("IGroundedAttackResetRoutine");
        _currentGroundedAttackRoutine = null;
    }
}


