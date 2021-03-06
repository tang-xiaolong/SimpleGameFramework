using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour,IInitable
{
    public Rigidbody2D birdRig;

    private Transform _transform;
    // [SerializeField]
    private Vector2 upForce;

    private bool willDie = false;
    private bool birdIsDie = false;
    private bool willAddScore = false;

    void ListenMessage()
    {
        MessageManager.Instance.Register(MessageDefine.BEGIN_GAME, Init);
    }
    
    private void Awake()
    {
        _transform = transform;
        upForce = new Vector2(0, GlobalConfig.BIRD_UP_FORCE);
        Init();
        ListenMessage();
        birdRig.simulated = false;
    }

    private void Update()
    {
        if (birdIsDie == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                birdRig.velocity = Vector2.zero;
                birdRig.AddForce(upForce);
            }
        }
    }

    private void LateUpdate()
    {
        if (birdIsDie == false && willAddScore == true)
        {
            willAddScore = false;
            MessageManager.Instance.Send(MessageDefine.ADD_SCORE);
        }
        if (willDie == true && birdIsDie == false)
        {
            birdIsDie = true;
            Debug.Log("Die");
            MessageManager.Instance.Send(MessageDefine.BIRD_DIE);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Land") || other.gameObject.CompareTag("Tube"))
        {
            if (birdIsDie == false)
            {
                willDie = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ScorePanel"))
        {
            willAddScore = true;
            other.gameObject.SetActive(false);
        }
    }

    public void Init()
    {
        _transform.position = Vector3.zero;
        birdIsDie = false;
        willDie = false;
        willAddScore = false;
        _transform.localRotation = Quaternion.identity;
        birdRig.simulated = true;
    }
}
