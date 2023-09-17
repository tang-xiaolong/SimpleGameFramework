using System;
using UnityEngine;

public class EnterFirstUI : MonoBehaviour
{
    private void Awake()
    {
        UIManager.Instance.OpenPanel(UIManager.DefaultUIName);
    }
}