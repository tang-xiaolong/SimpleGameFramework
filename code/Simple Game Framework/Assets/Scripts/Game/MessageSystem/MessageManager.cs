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

    public void Register(string key, UnityAction action)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if (previousAction is MessageData messageData)
            {
                messageData.MessageEvents += action;
            }
        }
        else
        {
            dictionaryMessage.Add(key, new MessageData(action));
        }
    }

    public void Remove(string key, UnityAction action)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if (previousAction is MessageData messageData)
            {
                messageData.MessageEvents -= action;
            }
        }
    }

    public void Send(string key)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            (previousAction as MessageData)?.MessageEvents.Invoke();
        }
    }

    public void Clear()
    {
        dictionaryMessage.Clear();
    }
}