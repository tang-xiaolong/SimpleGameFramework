using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBtnGroup : MonoBehaviour
{
    public Button btnPrefab;
    private List<string> _btnNames = new List<string>()
    {
        UIDefine.FullPanel1,
        UIDefine.FullPanel2,
        UIDefine.PopWindow1,
        UIDefine.PopWindow2,
    };

    private void Awake()
    {
        string ownerPanelName = transform.parent.GetComponent<UIBasePanel>().GetPanelName();
        foreach (var btnName in _btnNames)
        {
            if (btnName == ownerPanelName)
                continue;
            Button btn = Instantiate(btnPrefab, transform);
            btn.GetComponentInChildren<Text>().text = btnName;
            btn.transform.Find("text").GetComponent<Text>().text = btnName;
            btn.onClick.AddListener(() =>
            {
                UIManager.Instance.OpenPanel(btnName);
            });
        }
    }
}