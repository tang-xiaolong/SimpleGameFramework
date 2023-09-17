using UnityEngine.UI;

public class FullPanel2 : UIBasePanel
{
    public Button btnClose;

    private void Awake()
    {
        btnClose.onClick.AddListener(Close);
    }

    public override string GetPanelName()
    {
        return "FullPanel2";
    }

    public override bool IsFullPanel()
    {
        return true;
    }
}