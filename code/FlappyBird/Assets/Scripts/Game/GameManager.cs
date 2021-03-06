using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>, IInitable
{
    public void BeginGame()
    {
        MessageManager.Instance.Send(MessageDefine.BEGIN_GAME);
    }

    public void Init()
    {
        ListenMessage();
    }

    void ListenMessage()
    {
        MessageManager.Instance.Register(MessageDefine.ON_BEGIN_GAME_CLICK, BeginGame);
    }
}