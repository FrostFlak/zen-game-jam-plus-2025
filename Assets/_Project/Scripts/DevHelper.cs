using System;
using UnityEngine;

public class Debug : MonoBehaviour {
    
    #if UNITY_EDITOR
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            Time.timeScale *= 3;
        else if (Input.GetKeyUp(KeyCode.Space))
            Time.timeScale /= 3;
    }
#endif
    
}
