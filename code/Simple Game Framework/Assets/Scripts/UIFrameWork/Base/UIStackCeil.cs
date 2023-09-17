using System;
using System.Collections.Generic;
using PoolModule;
using UnityEngine;

public class UIStackCeil : IPoolObjectItem
{
    public Action OnClose;
    public bool IsShowing = false; //可能被两个东西影响，一个是全屏界面打开，会把前面界面关了，一个是打开了同名的界面，界面复用了，这个时候一定要记录下来，等这个界面关闭时务必再把它打开。
    public bool IsCloseSameNamePanel = false;
    public UIBasePanel Panel;
    public UIStackCeil PreviousFullPanelCeil;
    Dictionary<int, object> _panelParam = new Dictionary<int, object>();
    private ScreenParam _param;
    

    public override string ToString()
    {
        return "界面名：" + Panel.GetPanelName() + 
               "\n前一个全屏界面名：" +
               (PreviousFullPanelCeil != null ? PreviousFullPanelCeil.Panel.GetPanelName() : "空") + 
               "\n是否显示:" + IsShowing + 
               "\n是否关闭过同名界面:" + IsCloseSameNamePanel;
    }


    public void Show(ScreenParam param, bool isFirstShow = true)
    {
        IsShowing = true;
        if(isFirstShow)
        {
            _param = param;
            Panel.OnInit(param);
        }
        else
            ResetPanelParam();
        Panel.OnShow();
    }
    
    public void ReShow()
    {
        Show(_param, false);
    }
    
    public void Hide()
    {
        IsShowing = false;
        SavePanelParam();
        Panel.OnHide();
    }
    
    public void Close()
    {
        IsShowing = false;
        IsCloseSameNamePanel = false;
        Panel.OnClose();
    }

    void SavePanelParam()
    {
        _panelParam = Panel.GetPanelRuntimeParam();
    }

    void ResetPanelParam()
    {
        Panel.ResetPanelRuntimeParam(_panelParam);
    }
    

    public void OnGetHandle()
    {
        IsCloseSameNamePanel = false;
        IsShowing = false;
        _panelParam = null;
        Panel = null;
        _param = null;
    }

    public void OnRecycleHandle()
    {
        IsCloseSameNamePanel = false;
        IsShowing = false;
        _panelParam = null;
        Panel = null;
        _param = null;
    }
}