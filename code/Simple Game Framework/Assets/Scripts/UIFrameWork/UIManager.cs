using System;
using System.Collections.Generic;
using PoolModule;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public static string DefaultUIName = UIDefine.FullPanel1;
    private List<UIStackCeil> _uiStack = new List<UIStackCeil>();
    private Dictionary<string, int> _panelRefDic = new Dictionary<string, int>(); 
    private Dictionary<string, UIBasePanel> _panelDic = new Dictionary<string, UIBasePanel>();
    public Action OnStackInfoRefresh;
    private Transform _canvasTransform;
    private Transform GetCanvasTransform()
    {
        if (_canvasTransform == null)
        {
            _canvasTransform = GameObject.Find("UICanvas").transform;
        }

        return _canvasTransform;
    }

    public List<UIStackCeil> UIStack
    {
        get { return _uiStack; }
    }

    public void OpenPanel(string uiName, ScreenParam param = null)
    {
        // if (_panelRefDic.ContainsKey(uiName))
        UIBasePanel panel = GetPanel(uiName);
        if (panel != null)
        {
            //添加引用
            int refCount = 1;
            if (_panelRefDic.TryGetValue(uiName, out refCount))
            {
                refCount++;
                _panelRefDic[uiName] = refCount;
            }
            else
            {
                _panelRefDic.Add(uiName, 1);
                refCount = 1;
            }
            UIStackCeil uiStackCeil = ObjectPoolFactory.Instance.GetItem<UIStackCeil>();
            uiStackCeil.Panel = panel;
            
            //找到上一个全屏界面
            if (_uiStack.Count > 0)
            {
                var previousStackCeil = _uiStack[_uiStack.Count - 1];
                uiStackCeil.PreviousFullPanelCeil = previousStackCeil.Panel.IsFullPanel()
                    ? previousStackCeil
                    : previousStackCeil.PreviousFullPanelCeil;
            }
            else
                uiStackCeil.PreviousFullPanelCeil = null;
            
            //如果这个界面是全屏界面，则需要把前面的全屏界面隐藏。
            if (panel.IsFullPanel())
            {
                if(uiStackCeil.PreviousFullPanelCeil != null)
                {
                    //隐藏
                    uiStackCeil.PreviousFullPanelCeil.Hide();
                }
            }
            //如果引用数量大于1，则说明把同名的界面关了，后面如果把这个界面关了，需要再把对应同名的界面打开
            if (refCount > 1)
            {
                //把前面同名界面隐藏
                var previousSameNameCeil = FindPreviousSameNameCeil(uiName, _uiStack.Count - 1);
                uiStackCeil.IsCloseSameNamePanel = previousSameNameCeil!= null && previousSameNameCeil.IsShowing;
                if(uiStackCeil.IsCloseSameNamePanel)
                    previousSameNameCeil.Hide();
            }
            else
                uiStackCeil.IsCloseSameNamePanel = false;
            _uiStack.Add(uiStackCeil);
            uiStackCeil.Show(param);
            ResortPanel();
        }
        else
        {
            Debug.LogError("OpenPanel Error: " + uiName);
        }
        OnStackInfoRefresh?.Invoke();
    }

    //找到前一个同名的UIStackCeil
    UIStackCeil FindPreviousSameNameCeil(string uiName, int maxIndex)
    {
        for (int i = maxIndex; i >= 0; i--)
        {
            UIStackCeil stackCeil = _uiStack[i];
            if (stackCeil.Panel.GetPanelName() == uiName)
            {
                return stackCeil;
            }
        }

        return null;
    }
    
    void ResortPanel()
    {
        //遍历UI栈，如果界面显示着的，则对应的位置Index，并计数加1
        int index = 0;
        for (int i = 0; i < _uiStack.Count; i++)
        {
            var uiStackCeil = _uiStack[i];
            if (uiStackCeil.IsShowing)
            {
                uiStackCeil.Panel.transform.SetSiblingIndex(index);
                index++;
            }
        }
    }

    public void ClosePanel(string uiName)
    {
        //从_uiStack中找到符合要求的最后一个界面，尝试关闭它，并维护上一个全屏界面引用
        for (int i = _uiStack.Count - 1; i >= 0; i--)
        {
            UIStackCeil stackCeil = _uiStack[i];
            if (stackCeil.Panel.GetPanelName() == uiName)
            {
                //这个ceil的关闭不一定真的会关闭Panel,如果这个界面引用数量大于1，则只隐藏，否则关闭
                int refCount = 0;
                if (_panelRefDic.TryGetValue(uiName, out refCount))
                {
                    refCount--;
                    if (refCount > 0)
                    {
                        _panelRefDic[uiName] = refCount;
                    }
                    else
                    {
                        _panelRefDic.Remove(uiName);
                    }
                }
                else
                {
                    Debug.LogError("ClosePanel Error: " + uiName);
                    return;
                }

                bool isShowing = stackCeil.IsShowing;
                if(refCount > 0)
                    stackCeil.Hide();
                else
                    stackCeil.Close();
                
                //如果这个界面是全屏界面，则需要把前面的全屏界面显示。
                if (stackCeil.Panel.IsFullPanel())
                {
                    //如果全屏界面时发现前面没有打开过全屏界面了，则报错
                    if(stackCeil.PreviousFullPanelCeil == null)
                        Debug.LogError("关闭一个全屏界面时，前面没有其他全屏界面了");
                    //更新前一个全屏界面的引用
                    for (int j = i + 1; j < _uiStack.Count; j++)
                    {
                        _uiStack[j].PreviousFullPanelCeil = stackCeil.PreviousFullPanelCeil;
                    }
                    
                    //如果关闭的这个全屏界面是显示状态，则把它前一个全屏界面显示出来。
                    if (isShowing && stackCeil.PreviousFullPanelCeil != null)
                        stackCeil.PreviousFullPanelCeil.ReShow();
                }
                else
                {
                    if(stackCeil.IsCloseSameNamePanel)
                    // if (refCount > 0)
                    {
                        //找到上一个同名弹窗界面，重新显示
                        FindPreviousSameNameCeil(uiName, i - 1)?.ReShow();
                    }
                }
                
                //回收
                _uiStack.RemoveAt(i);
                ObjectPoolFactory.Instance.RecycleItem(stackCeil);
                if (refCount <= 0)
                {
                    UnityObjectPoolFactory.Instance.RecycleItem(GetUIPathByUIName(uiName), _panelDic[uiName].gameObject);
                    _panelDic.Remove(uiName);
                }
                break;
            }
        }
        
        //如果UI栈没有显示的UI了，则打开默认UI
        if (_uiStack.Count == 0)
        {
            OpenPanel(DefaultUIName);
        }
        
        ResortPanel();
        OnStackInfoRefresh?.Invoke();
    }

    string GetUIPathByUIName(string uiName)
    {
        return "Prefabs/UIPanel/" + uiName;
    }

    /// <summary>
    /// 如果界面已经打开，直接返回这个界面，否则创建一个新界面
    /// </summary>
    /// <param name="uiName"></param>
    /// <returns></returns>
    private UIBasePanel GetPanel(string uiName)
    {
        if (_panelDic.TryGetValue(uiName, out var panel))
            return panel;
        GameObject objPanel = UnityObjectPoolFactory.Instance.GetItem<GameObject>(GetUIPathByUIName(uiName));
        if (objPanel == null)
        {
            Debug.LogError("Load Panel Error: " + uiName);
        }
        else
        {
            objPanel.name = uiName;
            objPanel.transform.SetParent(GetCanvasTransform(), false);
            objPanel.transform.localScale = Vector3.one;
            UIBasePanel basePanel = objPanel.GetComponent<UIBasePanel>();
            if(basePanel == null)
                Debug.LogError("Panel not has UIBasePanel Component: " + uiName);
            else
                _panelDic.Add(uiName, basePanel);
            return basePanel;
        }

        return null;
    }
}