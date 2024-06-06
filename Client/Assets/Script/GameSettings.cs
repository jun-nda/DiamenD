using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSettings : MonoBehaviour {
    public TMP_Dropdown solution;
    // Start is called before the first frame update
    void Start()
    {
        solution.onValueChanged.AddListener((value) =>
        {
            switch (value) {
                case 0:
                    Screen.SetResolution(1280, 720, false);
                    break;
                case 1:
                    Screen.SetResolution(800, 360, false);
                    break;
                case 2:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
