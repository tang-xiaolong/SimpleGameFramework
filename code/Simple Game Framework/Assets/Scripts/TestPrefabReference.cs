using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class TestPrefabReference : MonoBehaviour
{
    public GameObject obj1;
    private GameObject objIns;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (obj1)
            {
                if (objIns != null)
                {
                    Destroy(objIns);
                }
                objIns = Instantiate(obj1);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (objIns != null)
            {
                Destroy(objIns);
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            objIns = null;
        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            obj1 = Resources.Load<GameObject>("objs");
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            obj1 = null;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Destroy(this);
        }
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            Destroy(gameObject);
        }
    }
}
