using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MessageManager : Singleton<MessageManager>
{
    private Dictionary<string, IMessageData> dictionaryMessage;

    public MessageManager()
    {
        InitData();
    }

    private void InitData()
    {
        dictionaryMessage = new Dictionary<string, IMessageData>();
    }

    public void Register<T>(string key, UnityAction<T> action)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if (previousAction is MessageData<T> messageData)
            {
                messageData.MessageEvents += action;
            }
        }
        else
        {
            dictionaryMessage.Add(key, new MessageData<T>(action));
        }
    }

    public void Remove<T>(string key, UnityAction<T> action)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if (previousAction is MessageData<T> messageData)
            {
                messageData.MessageEvents -= action;
            }
        }
    }

    public void Send<T>(string key, T data)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            (previousAction as MessageData<T>)?.MessageEvents.Invoke(data);
        }
    }

    public void Clear()
    {
        dictionaryMessage.Clear();
    }
}