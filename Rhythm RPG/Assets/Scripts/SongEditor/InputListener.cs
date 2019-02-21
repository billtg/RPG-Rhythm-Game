using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputListener : MonoBehaviour {

    public KeyCode escapeKey;

    private void Update()
    {
        if (Input.GetKeyDown(escapeKey))
        {
            SceneManager.LoadScene(0);
        }
    }
}
