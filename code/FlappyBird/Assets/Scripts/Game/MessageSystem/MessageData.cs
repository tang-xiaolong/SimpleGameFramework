using UnityEngine.Events;

public interface IMessageData
{
}

public class MessageData<T> : IMessageData
{
    public UnityAction<T> MessageEvents;

    public MessageData(UnityAction<T> action)
    {
        MessageEvents += action;
    }
}

public class MessageData : IMessageData
{
    public UnityAction MessageEvents;

    public MessageData(UnityAction action)
    {
        MessageEvents += action;
    }
}