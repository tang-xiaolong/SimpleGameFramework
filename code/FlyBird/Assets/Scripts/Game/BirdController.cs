using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour,IInitable
{
    public Rigidbody2D birdRig;
    // [SerializeField]
    private Vector2 upForce;

    private bool willDie = false;
    private bool birdIsDie = false;
    private bool willAddScore = false;

    private HudManager _hudManager;
    private GameManager _gameManager;

    private void Awake()
    {
        upForce = new Vector2(0, GlobalConfig.BIRD_UP_FORCE);
        _hudManager = FindObjectOfType<HudManager>();
        _gameManager = FindObjectOfType<GameManager>();
        Init();
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
            _hudManager.AddScore();
        }
        if (willDie == true && birdIsDie == false)
        {
            birdIsDie = true;
            Debug.Log("Die");
            _gameManager.BirdDie();
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
        birdRig.transform.position = Vector3.zero;
        birdIsDie = false;
        willDie = false;
        willAddScore = false;
    }
}
