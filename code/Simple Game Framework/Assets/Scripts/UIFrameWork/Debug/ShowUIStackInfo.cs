using System;
using System.Collections.Generic;
using PoolModule;
using UnityEngine;

public class ShowUIStackInfo : MonoBehaviour
{
    public GameObject stackInfoPrefab;
    List<UIStackCeilDebugInfo> _uiStackCeilDebugInfos = new List<UIStackCeilDebugInfo>();
    private void Awake()
    {
        UIManager.Instance.OnStackInfoRefresh += OnStackInfoRefresh;
    }

    private void OnStackInfoRefresh()
    {
        //回收所有的对象
        foreach (var uiStackCeilDebugInfo in _uiStackCeilDebugInfos)
        {
            UnityObjectPoolFactory.Instance.RecycleItem(stackInfoPrefab.name, uiStackCeilDebugInfo.gameObject);
        }
        _uiStackCeilDebugInfos.Clear();
        //拿到StackCeil列表
        var uiStack = UIManager.Instance.UIStack;
        for (int i = 0; i < uiStack.Count; i++)
        {
            var uiStackCeil = uiStack[i];
            GameObject debugCeilObj = UnityObjectPoolFactory.Instance.GetItem<GameObject>(stackInfoPrefab.name);
            UIStackCeilDebugInfo uiStackCeilDebugInfo = debugCeilObj.GetComponent<UIStackCeilDebugInfo>();
            uiStackCeilDebugInfo.transform.SetParent(transform);
            debugCeilObj.transform.localScale = Vector3.one;
            debugCeilObj.transform.localPosition = Vector3.zero;
            uiStackCeilDebugInfo.SetInfo(uiStackCeil);
            _uiStackCeilDebugInfos.Add(uiStackCeilDebugInfo);
        }
    }
}