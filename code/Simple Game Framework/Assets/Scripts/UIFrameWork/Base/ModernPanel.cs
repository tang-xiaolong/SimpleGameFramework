using System;
using UnityEngine;
using UnityEngine.UI;

public class ModernPanel : MonoBehaviour, IDisposable
{
    private Button btnClose;
    private UIBasePanel _panel;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        btnClose = GetComponent<Button>();
    }

    public void BindPanel(UIBasePanel panel)
    {
        _panel = panel;
        if(btnClose)
            btnClose.onClick.RemoveAllListeners();
        if (panel)
        {
            _transform.SetParent(_panel.transform, false);
            _transform.localPosition = Vector3.zero;
            _transform.localScale = Vector3.one;
            _transform.SetSiblingIndex(0);
            if(panel.ClickModernNeedClosePanel())
                btnClose.onClick.AddListener(panel.Close);
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }
    
    void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_panel)
            {
                _panel = null;
            }
        }
        if(btnClose)
            btnClose.onClick.RemoveAllListeners();
    }
    
    ~ModernPanel()
    {
        Dispose(false);
    }
}