using System;
using System.Collections.Generic;
using PoolModule;
using UnityEngine.Events;


public abstract class MessageDataBase : IPoolObjectItem
{
    public bool HasDispatching = false;
    public int DispatchLoopCount = 0;
    public void OnGetHandle()
    {
        HasDispatching = false;
        DispatchLoopCount = 0;
        BeGetHandle();
    }

    public abstract bool NotEmpty();

    public void OnRecycleHandle()
    {
        BeRecycleHandle();
    }

    protected virtual void BeGetHandle()
    {
    }
    
    protected virtual void BeRecycleHandle()
    {
    }
}

public class MessageData<T> : MessageDataBase
{
    private List<MessageDataBody<T>> _messageQueue;
    public MessageData()
    {
    }

    public void AddMessageAction(UnityAction<T> action, int priority = 0)
    {
        var messageDataBody = ObjectPoolFactory.Instance.GetItem<MessageDataBody<T>>();
        messageDataBody.MessageEvents = action;
        messageDataBody.Priority = priority;
        
        if(_messageQueue.Count == 0)
            _messageQueue.Add(messageDataBody);
        else
        {
            int index = -1;
            for (int i = 0; i < _messageQueue.Count; i++)
            {
                var body = _messageQueue[i];
                if(body.Priority < priority)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                _messageQueue.Add(messageDataBody);
            else
                _messageQueue.Insert(index, messageDataBody);
        }
    }
    
    public void RemoveMessageAction(UnityAction<T> action)
    {
        if (_messageQueue.Count > 0)
        {
            int index = -1;
            for (int i = 0; i < _messageQueue.Count; i++)
            {
                if (_messageQueue[i].MessageEvents == action)
                {
                    index = i;
                    break;
                }
            }
            if(index != -1)
            {
                ObjectPoolFactory.Instance.RecycleItem(_messageQueue[index]);
                _messageQueue.RemoveAt(index);
            }
        }
    }

    public override bool NotEmpty()
    {
        return _messageQueue != null && _messageQueue.Count > 0;
    }

    protected override void BeGetHandle()
    {
        _messageQueue = ObjectPoolFactory.Instance.GetItem<List<MessageDataBody<T>>>();
    }

    protected override void BeRecycleHandle()
    {
        if (_messageQueue != null)
        {
            int count = _messageQueue.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ObjectPoolFactory.Instance.RecycleItem(_messageQueue[i]);
                }
            }
            _messageQueue.Clear();
            ObjectPoolFactory.Instance.RecycleItem(_messageQueue);
            _messageQueue = null;
        }
    }

    public void Dispatch(T data)
    {
        for (int i = 0; i < _messageQueue.Count; i++)
        {
            var messageDataBody = _messageQueue[i];
            try
            {
                messageDataBody.MessageEvents.Invoke(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

public class MessageData : MessageDataBase
{
    public List<MessageDataBody> _messageQueue;

    public MessageData()
    {
    }

    public void AddMessageAction(UnityAction action, int priority = 0)
    {
        var messageDataBody = ObjectPoolFactory.Instance.GetItem<MessageDataBody>();
        messageDataBody.MessageEvents = action;
        messageDataBody.Priority = priority;
        
        if(_messageQueue.Count == 0)
            _messageQueue.Add(messageDataBody);
        else
        {
            int index = -1;
            for (int i = 0; i < _messageQueue.Count; i++)
            {
                var body = _messageQueue[i];
                if(body.Priority < priority)
                {
                    index = i;
                    break;
                }
            }
            
            if (index == -1)
                _messageQueue.Add(messageDataBody);
            else
                _messageQueue.Insert(index, messageDataBody);
        }
    }

    public void RemoveMessageAction(UnityAction action)
    {
        if (_messageQueue.Count > 0)
        {
            int index = -1;
            for (int i = 0; i < _messageQueue.Count; i++)
            {
                if (_messageQueue[i].MessageEvents == action)
                {
                    index = i;
                    break;
                }
            }
            if(index != -1)
            {
                ObjectPoolFactory.Instance.RecycleItem(_messageQueue[index]);
                _messageQueue.RemoveAt(index);
            }
        }
    }

    public override bool NotEmpty()
    {
        return _messageQueue != null && _messageQueue.Count > 0;
    }

    protected override void BeGetHandle()
    {
        _messageQueue = ObjectPoolFactory.Instance.GetItem<List<MessageDataBody>>();
    }

    protected override void BeRecycleHandle()
    {
        if (_messageQueue != null)
        {
            int count = _messageQueue.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ObjectPoolFactory.Instance.RecycleItem(_messageQueue[i]);
                }
            }
            _messageQueue.Clear();
            ObjectPoolFactory.Instance.RecycleItem(_messageQueue);
            _messageQueue = null;
        }
    }
    
    public void Dispatch()
    {
        for (int i = 0; i < _messageQueue.Count; i++)
        {
            var messageDataBody = _messageQueue[i];
            try
            {
                messageDataBody.MessageEvents.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}