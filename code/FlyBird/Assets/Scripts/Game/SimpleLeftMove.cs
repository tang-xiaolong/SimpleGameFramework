using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLeftMove : MonoBehaviour
{
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        _transform.position += Vector3.left * GlobalConfig.BG_MOVE_SPEED * Time.deltaTime;
    }
}
