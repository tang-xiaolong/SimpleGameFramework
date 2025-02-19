using System;
using UnityEngine;

// public enum Direction
// {
//     Up,
//     Down,
//     Left,
//     Right
// }

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public enum CharacterState
{
    [InspectorName("寝室/睡觉")] Sleep,
    [InspectorName("寝室/打游戏")] Gaming,
    [InspectorName("寝室/吃东西")] Eat,
    [InspectorName("教室/学习")] Study,
    [InspectorName("教室/聊天")] Talk,
}

public enum NodeDirection
{
    Up,
    Down,
    Left,
    Right,
    Count
}

[System.Flags]
public enum CharacterBeBanAbility
{
    [InspectorName("移动")] Move = 1 << 0,
    [InspectorName("攻击")] Attack = 1 << 1,
    [InspectorName("使用技能")] UseSkill = 1 << 2,
    [InspectorName("选取目标")] SelectTarget = 1 << 3,
    [InspectorName("回复行动力")] ReplyActionPoint = 1 << 4,
    [InspectorName("行动")] DoAction = Move | Attack | UseSkill | SelectTarget | ReplyActionPoint,
    [InspectorName("被选取")] BeSelect = 1 << 5,
    [InspectorName("被伤害")] BeHurt = 1 << 6
}

public class EnumTest : MonoBehaviour
{
    public Direction dir;
    public CharacterState characterState;
    public CharacterBeBanAbility characterBeBanAbility;


    public long characterAbilityMask;

    [ContextMenu("眩晕")]
    void SetStun()
    {
        characterAbilityMask |= (long)CharacterBeBanAbility.DoAction;
    }
    
    [ContextMenu("TryMove")]
    void TryMove()
    {
        if (((CharacterBeBanAbility)characterAbilityMask).HasFlag(CharacterBeBanAbility.Move))
            Debug.Log("不能移动");
        else
            Debug.Log("可以移动");
        
        if((characterAbilityMask & (long)CharacterBeBanAbility.Move) == 0)
            Debug.Log("可以移动");
        else
            Debug.Log("不能移动");
    }


    private void Awake()
    {
        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            Debug.Log(direction);
        }
        Direction[] directions = Enum.GetValues(typeof(Direction)) as Direction[];
        foreach (Direction direction in directions)
        {
            Debug.Log(direction);
        }
        for (int i = 0; i < (int)NodeDirection.Count; i++)
        {
            Debug.Log((NodeDirection)i);
        }
    }
}