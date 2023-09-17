using System.Collections.Generic;
using UnityEngine.UI;

public class FullPanel1 : UIBasePanel
{
    enum TestParamEnum
    {
        ToggleData,
        TextData,
    }
    
    public Button btnClose;
    public Toggle toggle;
    public InputField inputField;

    private void Awake()
    {
        btnClose.onClick.AddListener(Close);
    }

    public override Dictionary<int, object> GetPanelRuntimeParam()
    {
        return new Dictionary<int, object>()
        {
            {(int)TestParamEnum.ToggleData, toggle.isOn},
            {(int)TestParamEnum.TextData, inputField.text},
        };
    }

    public override void ResetPanelRuntimeParam(Dictionary<int, object> param)
    {
        if (param == null)
            return;
        if(param.TryGetValue((int)TestParamEnum.ToggleData, out object toggleData))
            toggle.isOn = (bool)toggleData;
        if(param.TryGetValue((int)TestParamEnum.TextData, out object textData))
            inputField.text = (string)textData;
    }

    public override string GetPanelName()
    {
        return "FullPanel1";
    }

    public override bool IsFullPanel()
    {
        return true;
    }
}