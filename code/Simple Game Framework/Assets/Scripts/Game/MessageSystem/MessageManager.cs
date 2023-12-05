using System;
using System.Collections.Generic;
using PoolModule;
using UnityEngine;
using UnityEngine.Events;

public class MessageManager : Singleton<MessageManager>, IDisposable
{
    private Dictionary<string, IMessageData> dictionaryMessage;
    private Dictionary<string, List<Action>> _delayExecuteActions = new Dictionary<string, List<Action>>();
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
                if (!messageData.HasDispatching)
                    messageData.MessageEvents += action;
                else
                {
                    void RegisterAction()
                    {
                        Register(key, action);
                    }

                    AddDelayExecuteAction(key, RegisterAction);
                }
            }
        }
        else
        {
            // dictionaryMessage.Add(key, new MessageData<T>(action));
            var messageData = ObjectPoolFactory.Instance.GetItem<MessageData<T>>();
            messageData.MessageEvents += action;
            dictionaryMessage.Add(key, messageData);
        }
    }

    public void Remove<T>(string key, UnityAction<T> action)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if (previousAction is MessageData<T> messageData)
            {
                if(!messageData.HasDispatching)
                {
                    if(messageData.MessageEvents != null)
                        messageData.MessageEvents -= action;
                    if (messageData.MessageEvents == null)
                    {
                        dictionaryMessage.Remove(key);
                        ObjectPoolFactory.Instance.RecycleItem(messageData);
                    }
                }
                else
                {
                    void RemoveAction()
                    {
                        Remove(key, action);
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
            // (previousAction as MessageData<T>)?.MessageEvents.Invoke(data);
            
            if(previousAction is MessageData<T> messageData)
            {
                if (messageData.HasDispatching)
                {
                    void DispatchAction()
                    {
                        Send(key, data);
                    }

                    AddDelayExecuteAction(key, DispatchAction);

                    return;
                }
                
                messageData.HasDispatching = true;
                var invocationList = messageData.MessageEvents.GetInvocationList();
                foreach (var item in invocationList)
                {
                    if (item is UnityAction<T> action)
                    {
                        try
                        {
                            action.Invoke(data);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        
                    }
                }
                messageData.HasDispatching = false;
                //如果key有对应未处理的延时执行的action，则执行
                TryExecuteDelayAction(key);
            }
        }
    }

    public void Register(string key, UnityAction action)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if (previousAction is MessageData messageData)
            {
                if(!messageData.HasDispatching)
                    messageData.MessageEvents += action;
                else
                {
                    void RegisterAction()
                    {
                        Register(key, action);
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
            messageData.MessageEvents += action;
            dictionaryMessage.Add(key, messageData);
        }
    }

    public void Remove(string key, UnityAction action)
    {
        if (dictionaryMessage.TryGetValue(key, out var previousAction))
        {
            if (previousAction is MessageData messageData)
            {
                if(!messageData.HasDispatching)
                {
                    if(messageData.MessageEvents != null)
                        messageData.MessageEvents -= action;
                    if(messageData.MessageEvents == null)
                    {
                        dictionaryMessage.Remove(key);
                        ObjectPoolFactory.Instance.RecycleItem(messageData);
                    }
                }
                else
                {
                    void RemoveAction()
                    {
                        Remove(key, action);
                    }

                    AddDelayExecuteAction(key, RemoveAction);
                }
            }
        }
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
            // (previousAction as MessageData)?.MessageEvents.Invoke();
            if (previousAction is MessageData messageData)
            {
                if (messageData.HasDispatching)
                {
                    
                    return;
                }
                //设置派发标记位
                messageData.HasDispatching = true;
                if(messageData.MessageEvents != null)
                {
                    var invocationList = messageData.MessageEvents.GetInvocationList();
                    foreach (var item in invocationList)
                    {
                        if (item is UnityAction action)
                        {
                            try
                            {
                                action.Invoke();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }

                        }
                    }
                }
                //派发完毕，清除标记位
                messageData.HasDispatching = false;
                //如果key有对应未处理的延时执行的action，则执行
                TryExecuteDelayAction(key);
            }
        }
    }

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