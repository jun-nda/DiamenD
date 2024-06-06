using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class GameManager : MonoBehaviour
    {
        private bool _hasStart;
        private float _remainTime;

        public bool IsReplay = false;
        
        public List<FrameInput> frames = new List<FrameInput>();
        
        private void Update()
        {
            if (!_hasStart) return;
            _remainTime += Time.deltaTime;
            while (_remainTime >= 0.03f) {
                _remainTime -= 0.03f;
                //send input
                if (!IsReplay) {
                    SendInput();
                }


                if (GetFrame(curFrameIdx) == null) {
                    return;
                }

                Step();
            }
        }

        private 
        
        private void SendInput()
        {
            
        }


        private void Step()
        {
            
        }
        
    }
}