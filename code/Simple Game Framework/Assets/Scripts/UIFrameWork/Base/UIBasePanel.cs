using System.Collections;
using System.Collections.Generic;
using PoolModule;
using UnityEngine;

public abstract class UIBasePanel : MonoBehaviour
{
    private ModernPanel _modernPanel;

    public abstract string GetPanelName();
    
    public void OnInit(ScreenParam param)
    {
        if(IsModernPanel() && _modernPanel == null)
        {
            GameObject objModernPanel = UnityObjectPoolFactory.Instance.GetItem<GameObject>("ModernPanel");
            if (objModernPanel)
            {
                _modernPanel = objModernPanel.GetComponent<ModernPanel>();
                if(_modernPanel)
                    _modernPanel.BindPanel(this);
            }
        }
        InitHandle(param);
    }
    
    public virtual void InitHandle(ScreenParam param)
    {
        
    }

    /// <summary>
    /// 界面可以被循环打开，被循环打开时如果需要保留参数，通过这个方法返回
    /// </summary>
    /// <returns></returns>
    public virtual Dictionary<int, object> GetPanelRuntimeParam()
    {
        return null;
    }
    
    /// <summary>
    /// 通过这个方法可以得到界面之前保存的参数
    /// </summary>
    /// <param name="param"></param>
    public virtual void ResetPanelRuntimeParam(Dictionary<int, object> param)
    {
    }

    public abstract bool IsFullPanel();
    /// <summary>
    /// 是否为模态窗口
    /// </summary>
    /// <returns></returns>
    public virtual bool IsModernPanel()
    {
        return false;
    }

    /// <summary>
    /// 点击模态窗口时是否要关闭界面
    /// </summary>
    /// <returns></returns>
    public virtual bool ClickModernNeedClosePanel()
    {
        return false;
    }
    
    public void OnShow()
    {
        this.gameObject.SetActive(true);
        OnShowHandle();
    }

    public void OnHide()
    {
        gameObject.SetActive(false);
        OnHideHandle();
    }
    
    /// <summary>
    /// 真正被关闭时调用
    /// </summary>
    public void OnClose()
    {
        OnHide();
        if (_modernPanel)
        {
            _modernPanel.Dispose();
            UnityObjectPoolFactory.Instance.RecycleItem("ModernPanel", _modernPanel.gameObject);
            _modernPanel = null;
        }
        OnCloseHandle();
    }

    public void Close()
    {
        UIManager.Instance.ClosePanel(GetPanelName());
    }
    
    protected virtual void OnShowHandle()
    {
    }

    protected virtual void OnHideHandle()
    {
    }
    
    protected virtual void OnCloseHandle()
    {
    }
}
