using UnityEngine.Events;

public class MessageDataBody<T>
{
    public UnityAction<T> MessageEvents;
    public int Priority;
    public MessageDataBody()
    {
    }
}

public class MessageDataBody
{
    public UnityAction MessageEvents;
    public int Priority;
    public MessageDataBody()
    {
    }
}