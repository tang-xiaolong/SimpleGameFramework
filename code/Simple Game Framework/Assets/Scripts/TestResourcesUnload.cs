using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestResourcesUnload : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Resources.UnloadUnusedAssets();
        }
    }
}
