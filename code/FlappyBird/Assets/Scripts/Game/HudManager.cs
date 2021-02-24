using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour, IInitable
{
    private int score = 0;
    public Text txtScore;

    private void Awake()
    {
        Init();
    }

    public void AddScore()
    {
        score += 1;
        RefreshScore();
    }

    public void Init()
    {
        score = 0;
        RefreshScore();
    }

    void RefreshScore()
    {
        if (txtScore)
        {
            txtScore.text = score.ToString();
        }
    }
}
