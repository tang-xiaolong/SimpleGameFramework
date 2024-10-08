﻿using System;
using System.Collections.Generic;
using PoolModule;
using UnityEngine;
using UnityEngine.Events;

public class MessageManager : Singleton<MessageManager>, IDisposable
{
    class MessageTagData
    {
        public string MessageTag;
        public Type DataType;
    }
    private Dictionary<string, MessageDataBase> dictionaryMessage;
    private Dictionary<string, List<Action>> _delayExecuteActions = new Dictionary<string, List<Action>>();
    public static bool NeedRecheckMsgTag = false;
    private Dictionary<string, string> _messageTagDic = new Dictionary<string, string>();
    private const int MaxLoopCount = 10;
    public MessageManager()
    {
        InitData();
    }

    private void InitData()
    {
        dictionaryMessage = new Dictionary<string, MessageDataBase>();
    }

    public void Register<T>(string key, UnityAction<T> action, int priority)
    {
        Register(key, action, priority, string.Empty);
    }
    
    void TryAddMessageTag(string key, string messageTag)
    {
        if (string.IsNullOrEmpty(messageTag))
            return;
        if (_messageTagDic.TryGetValue(key, out var oldTag))
        {
            if(NeedRecheckMsgTag && oldTag != messageTag)
                Debug.LogError(
                    $"MessageManager TryAddMessageTag oldTag != messageTag, oldTag:{oldTag}, messageTag:{messageTag}");
        }
        else
        {
            _messageTagDic.Add(key, messageTag);
        }
    }
    
    public void Register<T>(string key, UnityAction<T> action, int priority, string messageTag)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if (previousAction is MessageData<T> messageData)
            {
                if (!messageData.HasDispatching)
                {
                    messageData.AddMessageAction(action, priority);
                    TryAddMessageTag(key, messageTag);
                }
                else
                {
                    RegisterActionData<T> registerActionData = ObjectPoolFactory.Instance.GetItem<RegisterActionData<T>>();
                    registerActionData.Key = key;
                    registerActionData.Action = action;
                    registerActionData.Priority = priority;
                    registerActionData.MessageTag = messageTag;
                    void WrappedAction()
                    {
                        RegisterWithActionData(registerActionData);
                        ObjectPoolFactory.Instance.RecycleItem(registerActionData);
                    }
                    AddDelayExecuteAction(key, WrappedAction);
                }
            }
        }
        else
        {
            var messageData = ObjectPoolFactory.Instance.GetItem<MessageData<T>>();
            messageData.AddMessageAction(action, priority);
            TryAddMessageTag(key, messageTag);
            dictionaryMessage.Add(key, messageData);
        }
    }
    public void Register<T>(string key, UnityAction<T> action)
    {
        Register(key, action, MessageDispatchPriority.UI);
    }
    public void Register<T>(string key, UnityAction<T> action, string messageTag)
    {
        Register(key, action, MessageDispatchPriority.UI, messageTag);
    }

    public void Remove<T>(string key, UnityAction<T> action)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if (previousAction is MessageData<T> messageData)
            {
                if(!messageData.HasDispatching)
                {
                    if(messageData.NotEmpty())
                        messageData.RemoveMessageAction(action);
                    if (!messageData.NotEmpty())
                    {
                        dictionaryMessage.Remove(key);
                        if(_messageTagDic.ContainsKey(key))
                            _messageTagDic.Remove(key);
                        ObjectPoolFactory.Instance.RecycleItem(messageData);
                    }
                }
                else
                {
                    RemoveActionData<T> removeActionData = ObjectPoolFactory.Instance.GetItem<RemoveActionData<T>>();
                    removeActionData.Key = key;
                    removeActionData.Action = action;
                    void RemoveAction()
                    {
                        RemoveWithActionData(removeActionData);
                        ObjectPoolFactory.Instance.RecycleItem(removeActionData);
                    }
                    AddDelayExecuteAction(key, RemoveAction);
                }
            }
        }
    }
    
    void AddDelayExecuteAction(string key, Action action)
    {
        if (_delayExecuteActions.TryGetValue(key, out var actions))
        {
            actions.Add(action);
        }
        else
        {
            actions = ObjectPoolFactory.Instance.GetItem<List<Action>>();
            actions.Add(action);
            _delayExecuteActions.Add(key, actions);
        }
    }

    public void Send<T>(string key, T data)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if(previousAction is MessageData<T> messageData)
            {
                if (messageData.HasDispatching)
                {
                    var actionData = ObjectPoolFactory.Instance.GetItem<SendActionData<T>>();// new SendActionData<T> { Key = key, Data = data };
                    actionData.Key = key;
                    actionData.Data = data;

                    void WrappedAction()
                    {
                        SendWithActionData(actionData);
                        ObjectPoolFactory.Instance.RecycleItem(actionData);
                    }

                    AddDelayExecuteAction(key,  WrappedAction);
                    return;
                }
                
                messageData.HasDispatching = true;
                messageData.DispatchLoopCount++;
                if(messageData.NotEmpty())
                {
                    if (messageData.DispatchLoopCount < MaxLoopCount)
                        messageData.Dispatch(data);
                    else
                        Debug.LogError("MessageManager Send LoopCount > MaxLoopCount");
                }
                messageData.HasDispatching = false;
                //如果key有对应未处理的延时执行的action，则执行
                TryExecuteDelayAction(key);
                messageData.DispatchLoopCount = 0;
            }
        }
    }

    public void Register(string key, UnityAction action, int priority)
    {
        Register(key, action, priority, string.Empty);
    }
    
    public void Register(string key, UnityAction action, string messageTag)
    {
        Register(key, action, MessageDispatchPriority.UI, messageTag);
    }
    
    public void Register(string key, UnityAction action, int priority, string messageTag)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if (previousAction is MessageData messageData)
            {
                if(!messageData.HasDispatching)
                {
                    messageData.AddMessageAction(action, priority);
                    TryAddMessageTag(key, messageTag);
                }
                else
                {
                    RegisterActionData registerActionData = ObjectPoolFactory.Instance.GetItem<RegisterActionData>();
                    registerActionData.Key = key;
                    registerActionData.Action = action;
                    registerActionData.Priority = priority;
                    registerActionData.MessageTag = messageTag;
                    void RegisterAction()
                    {
                        RegisterWithActionData(registerActionData);
                        ObjectPoolFactory.Instance.RecycleItem(registerActionData);
                    }

                    AddDelayExecuteAction(key, RegisterAction);
                }
            }
        }
        else
        {
            // dictionaryMessage.Add(key, new MessageData(action));
            //池化
            var messageData = ObjectPoolFactory.Instance.GetItem<MessageData>();
            messageData.AddMessageAction(action, priority);
            TryAddMessageTag(key, messageTag);
            dictionaryMessage.Add(key, messageData);
        }
    }
    
    public void Register(string key, UnityAction action)
    {
        Register(key, action, MessageDispatchPriority.UI);
    }

    public void Remove(string key, UnityAction action)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if (previousAction is MessageData messageData)
            {
                if(!messageData.HasDispatching)
                {
                    if(messageData.NotEmpty())
                        messageData.RemoveMessageAction(action);
                    if (!messageData.NotEmpty())
                    {
                        dictionaryMessage.Remove(key);
                        if(_messageTagDic.ContainsKey(key))
                            _messageTagDic.Remove(key);
                        ObjectPoolFactory.Instance.RecycleItem(messageData);
                    }
                }
                else
                {
                    RemoveActionData removeActionData = ObjectPoolFactory.Instance.GetItem<RemoveActionData>();
                    removeActionData.Key = key;
                    removeActionData.Action = action;
                    void RemoveAction()
                    {
                        RemoveWithActionData(removeActionData);
                        ObjectPoolFactory.Instance.RecycleItem(removeActionData);
                    }

                    AddDelayExecuteAction(key, RemoveAction);
                }
            }
        }
    }
    
    public void RemoveMessageByTag(string messageTag)
    {
        List<string> keys = ObjectPoolFactory.Instance.GetItem<List<string>>();
        foreach (var item in _messageTagDic)
        {
            if (item.Value == messageTag)
            {
                keys.Add(item.Key);
            }
        }

        foreach (var key in keys)
        {
            if (dictionaryMessage.TryGetValue(key, out var previousAction))
            {
                previousAction.InjectRemove(this, key);
            }
        }
        ObjectPoolFactory.Instance.RecycleItem(keys);
    }

    //尝试将某个key对应的延时执行的action执行掉
    void TryExecuteDelayAction(string key)
    {
        if(_delayExecuteActions.TryGetValue(key, out var actions))
        {
            var newActionList = ObjectPoolFactory.Instance.GetItem<List<Action>>();
            newActionList.AddRange(actions);
            _delayExecuteActions.Remove(key);
            ObjectPoolFactory.Instance.RecycleItem(actions);
            foreach (var item in newActionList)
            {
                item.Invoke();
            }
            ObjectPoolFactory.Instance.RecycleItem(newActionList);
        }
    }

    public void Send(string key)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if (previousAction is MessageData messageData)
            {
                if (messageData.HasDispatching)
                {
                    var actionData = ObjectPoolFactory.Instance.GetItem<SendActionData>();
                    actionData.Key = key;
                    void DispatchAction()
                    {
                        SendWithActionData(actionData);
                        ObjectPoolFactory.Instance.RecycleItem(actionData);
                    }
                    
                    AddDelayExecuteAction(key, DispatchAction);
                    return;
                }
                //设置派发标记位
                messageData.HasDispatching = true;
                messageData.DispatchLoopCount++;
                if(messageData.NotEmpty())
                {
                    //小于等于最大派发次数，执行派发
                    if(messageData.DispatchLoopCount <= MaxLoopCount)
                        messageData.Dispatch();
                    else
                        Debug.LogError("MessageManager Send LoopCount > MaxLoopCount");
                }
                //派发完毕，清除标记位
                messageData.HasDispatching = false;
                //如果key有对应未处理的延时执行的action，则执行
                TryExecuteDelayAction(key);
                messageData.DispatchLoopCount = 0;
            }
        }
    }

    #region DelayEvent相关处理
    private struct SendActionData<T>
    {
        public string Key;
        public T Data;
    }
    private struct SendActionData
    {
        public string Key;
    }
    struct RegisterActionData<T>
    {
        public string Key;
        public UnityAction<T> Action;
        public int Priority;
        public string MessageTag;
    }

    struct RegisterActionData
    {
        public string Key;
        public UnityAction Action;
        public int Priority;
        public string MessageTag;
    }

    struct RemoveActionData<T>
    {
        public string Key;
        public UnityAction<T> Action;
    }
    struct RemoveActionData
    {
        public string Key;
        public UnityAction Action;
    }
    
    private void SendWithActionData<T>(SendActionData<T> actionData)
    {
        Send(actionData.Key, actionData.Data);
    }
    private void SendWithActionData(SendActionData actionData)
    {
        Send(actionData.Key);
    }
    void RegisterWithActionData<T>(RegisterActionData<T> actionData)
    {
        Register(actionData.Key, actionData.Action, actionData.Priority, actionData.MessageTag);
    }
    void RegisterWithActionData(RegisterActionData actionData)
    {
        Register(actionData.Key, actionData.Action, actionData.Priority, actionData.MessageTag);
    }
    void RemoveWithActionData<T>(RemoveActionData<T> actionData)
    {
        Remove(actionData.Key, actionData.Action);
    }
    
    void RemoveWithActionData(RemoveActionData actionData)
    {
        Remove(actionData.Key, actionData.Action);
    }
    #endregion

    public void Clear()
    {
        dictionaryMessage.Clear();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    void Dispose(bool disposing)
    {
        if (disposing)
        {
            Clear();
        }
    }
    ~MessageManager()
    {
        Dispose(false);
    }
}