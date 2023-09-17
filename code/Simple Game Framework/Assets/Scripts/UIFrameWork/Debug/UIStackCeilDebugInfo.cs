using UnityEngine;
using UnityEngine.UI;

public class UIStackCeilDebugInfo : MonoBehaviour
{
    public Text text;
    public void SetInfo(UIStackCeil uiStackCeil)
    {
        text.text = uiStackCeil.ToString();
    }
    
}