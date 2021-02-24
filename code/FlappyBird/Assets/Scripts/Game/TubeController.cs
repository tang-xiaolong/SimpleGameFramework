using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeController : MonoBehaviour
{
    private float minX = 0;
    private Transform _transform;

    private void Awake()
    {
        minX= - Camera.main.orthographicSize * GlobalConfig.SCREEN_ASPECT - 2;
        _transform = transform;
    }

    private void Update()
    {
        if (_transform.position.x < minX)
        {
            Destroy(gameObject);
        }
    }
}
