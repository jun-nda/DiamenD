using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    static CharacterInput _input;

    public static CharacterInput Input {
        get {
            InitInput();
            return _input;
        }
    }

    static void InitInput () {
        if (_input == null) {
            _input = new CharacterInput();
        }
    }
    
    void Awake () {
        InitInput();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {

    }

    void OnEnable () {
        _input.Character.Enable();
    }

    void OnDisable () {
        _input.Character.Disable();
    }
}
