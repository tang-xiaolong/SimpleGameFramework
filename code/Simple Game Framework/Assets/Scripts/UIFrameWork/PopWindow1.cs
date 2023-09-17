using System;
using UnityEngine.UI;

public class PopWindow1 : UIBasePanel
{
    public Button btnClose;

    private void Awake()
    {
        btnClose.onClick.AddListener(Close);
    }
    
    public override string GetPanelName()
    {
        return "PopWindow1";
    }

    public override bool IsFullPanel()
    {
        return false;
    }
    
    public override bool IsModernPanel()
    {
        return true;
    }
    
    public override bool ClickModernNeedClosePanel()
    {
        return true;
    }
}