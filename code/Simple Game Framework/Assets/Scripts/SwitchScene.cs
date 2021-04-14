using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public string sceneName;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (sceneName != string.Empty)
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
