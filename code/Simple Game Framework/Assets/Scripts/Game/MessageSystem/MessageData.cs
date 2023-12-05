using PoolModule;
using UnityEngine.Events;

public interface IMessageData
{
}

public class MessageData<T> : IMessageData, IPoolObjectItem
{
    public bool HasDispatching = false;
    public UnityAction<T> MessageEvents;
    public MessageData()
    {
    }

    public MessageData(UnityAction<T> action)
    {
        MessageEvents += action;
    }

    public void OnGetHandle()
    {
        HasDispatching = false;
    }

    public void OnRecycleHandle()
    {
        
    }
}

public class MessageData : IMessageData, IPoolObjectItem
{
    public bool HasDispatching = false;
    public UnityAction MessageEvents;

    public MessageData()
    {
        
    }

    public MessageData(UnityAction action)
    {
        MessageEvents += action;
    }

    public void OnGetHandle()
    {
        HasDispatching = false;
    }

    public void OnRecycleHandle()
    {
        
    }
}