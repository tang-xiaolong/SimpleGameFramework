using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public void BeginGame()
    {
        HudManager.Instance.BeginGame();
        TubeSpawn.Instance.BeginSpawnTube();
        FindObjectOfType<BirdController>().Init();
    }

    public void BirdDie()
    {
        HudManager.Instance.OnBirdDie();
        TubeSpawn.Instance.StopSpawnTube();
    }
}
