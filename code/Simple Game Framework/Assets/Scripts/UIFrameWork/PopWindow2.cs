using System;
using UnityEngine.UI;

public class PopWindow2 : UIBasePanel
{
    public Button btnClose;

    private void Awake()
    {
        btnClose.onClick.AddListener(Close);
    }

    public override string GetPanelName()
    {
        return "PopWindow2";
    }

    public override bool IsFullPanel()
    {
        return false;
    }
}