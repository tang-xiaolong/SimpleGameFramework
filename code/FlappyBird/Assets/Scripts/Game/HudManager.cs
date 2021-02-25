using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager :MonoSingleton<HudManager>, IInitable
{
    public GameObject beginPanel;
    public GameObject diePanel;
    private int score = 0;
    public Text txtScore;

    private void Awake()
    {
        Init();
        beginPanel.SetActive(true);
        diePanel.SetActive(false);
    }

    public void BeginGame()
    {
        Init();
        beginPanel.SetActive(false);
        diePanel.SetActive(false);
    }

    public void OnBirdDie()
    {
        diePanel.SetActive(true);
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

    void ShowBeginPanel()
    {
        
    }
}
