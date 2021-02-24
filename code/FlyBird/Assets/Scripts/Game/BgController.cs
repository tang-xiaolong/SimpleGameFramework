using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgController : MonoBehaviour
{
    public SpriteRenderer leftSprite;
    public SpriteRenderer rightSprite;
    private Transform leftTransform;
    private Transform rightTransform;

    private float bgWidth = 0;
    private float bgHalfWidth = 0;
    private float cameraLeftBorderValue;
    
    private void Start()
    {
        if (leftSprite == null || rightSprite == null)
        {
            Debug.LogWarning("Bg is miss!");
            enabled = false;
        }

        leftTransform = leftSprite.transform;
        rightTransform = rightSprite.transform;
        bgWidth = leftSprite.size.x;
        bgHalfWidth = bgWidth / 2;
        cameraLeftBorderValue = Camera.main.orthographicSize * GlobalConfig.SCREEN_ASPECT;
    }

    private void Update()
    {
        // leftTransform.position += Vector3.left * GlobalConfig.BG_MOVE_SPEED * Time.deltaTime;
        // rightTransform.position += Vector3.left * GlobalConfig.BG_MOVE_SPEED * Time.deltaTime;
        if (rightTransform.position.x - bgHalfWidth < -cameraLeftBorderValue)
        {
            leftTransform.position = rightTransform.position + new Vector3(bgHalfWidth * 2, 0, 0);
            var tempTransform = leftTransform;
            leftTransform = rightTransform;
            rightTransform = tempTransform;
        }
    }
}
