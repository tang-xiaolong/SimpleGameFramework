using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager :MonoSingleton<HudManager>, IInitable
{
    public Button beginBtn;
    public Button beginBtn1;
    public GameObject beginPanel;
    public GameObject diePanel;
    private int score = 0;
    public Text txtScore;

    private void Awake()
    {
        Init();
        beginBtn.onClick.AddListener(OnBeginGameClick);
        beginBtn1.onClick.AddListener(OnBeginGameClick);
        ListenMessage();
        beginPanel.SetActive(true);
        diePanel.SetActive(false);
    }

    void ListenMessage()
    {
        MessageManager.Instance.Register(MessageDefine.BEGIN_GAME, BeginGame);
        MessageManager.Instance.Register(MessageDefine.BIRD_DIE, OnBirdDie);
        MessageManager.Instance.Register(MessageDefine.ADD_SCORE, AddScore);
    }

    public void OnBeginGameClick()
    {
        MessageManager.Instance.Send(MessageDefine.ON_BEGIN_GAME_CLICK);
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
